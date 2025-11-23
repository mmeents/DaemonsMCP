import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TreeSelectModule } from 'primeng/treeselect';
import { TreeNode } from 'primeng/api';
import { ItemsTreeComponent } from '../../shared/components/items-tree/items-tree.component';
import { ItemsService } from '../../core/services/items.service';
import { ItemDto, AddUpdateItemRequest, ItemTypeDto } from '../../shared/models/api.models';

@Component({
  selector: 'app-items-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ItemsTreeComponent, TreeSelectModule],
  templateUrl: './items-page.component.html',
  styleUrl: './items-page.component.scss',
})
export class ItemsPageComponent implements OnInit {
  items: ItemDto[] = [];
  selectedItem?: ItemDto;
  loading = false;

  isEditing = false;
  isSaving = false;
  editForm: AddUpdateItemRequest = this.createEmptyForm();
    
  itemTypes: ItemTypeDto[] = [];
  statusTypes: ItemTypeDto[] = [];

  parentTreeOptions: TreeNode[] = [];
  selectedParentNode: TreeNode | null = null;

  constructor(private itemsService: ItemsService) {}

  ngOnInit() {
    this.loadItems();
    this.loadTypes();
  }

  private createEmptyForm(parentId?: number): AddUpdateItemRequest {
    return {
      id: 0,
      parentId: parentId,
      itemTypeId: 0,
      statusTypeId: 0,
      rank: 0,
      name: '',
      details: ''
    };
  }

  private loadTypes() {
    this.itemsService.getItemTypes().subscribe({
      next: (types) => this.itemTypes = types,
      error: (err) => console.error('Failed to load item types:', err)
    });
    
    this.itemsService.getStatusTypes().subscribe({
      next: (types) => this.statusTypes = types,
      error: (err) => console.error('Failed to load status types:', err)
    });
  }

  loadItems() {
    this.loading = true;
    // Load root items with 3 levels deep for the tree
    this.itemsService.searchItems({ maxDepth: 3 }).subscribe({
      next: (items) => {
        this.items = items;
        this.loading = false;
        console.log('Loaded items:', items);
      },
      error: (err) => {
        console.error('Failed to load items:', err);
        this.loading = false;
      }
    });
  }

  onItemSelected(item: ItemDto) {
    this.selectedItem = item;
    console.log('Selected item:', item);
  }

  onItemMoved(event: { item: ItemDto; newParentId?: number }) {
    console.log('Moving item:', event.item.name, 'to parent:', event.newParentId);
    
    // Create update request with new parent
    const updateRequest: AddUpdateItemRequest = {
      id: event.item.id,
      parentId: event.newParentId,
      itemTypeId: event.item.itemTypeId,
      statusTypeId: event.item.statusTypeId,
      rank: event.item.rank,
      name: event.item.name,
      details: event.item.details,
      referenceFileSystemId: event.item.referenceFileSystemId,
      referenceObjectHierarchyId: event.item.referenceObjectHierarchyId
    };

    this.itemsService.addUpdateItem(updateRequest).subscribe({
      next: (updatedItem) => {
        console.log('Item moved successfully:', updatedItem);
        this.loadItems(); // Refresh tree to show new structure
      },
      error: (err) => {
        console.error('Failed to move item:', err);
        // TODO: Show error notification to user
        // For now, just reload to restore original state
        this.loadItems();
      }
    });
  }

  onNewItem() {
    // Pre-select parent if an item is selected, otherwise root
    const parentId = this.selectedItem?.id;
    this.editForm = this.createEmptyForm(parentId);
    
    // Default to first available type/status if available
    if (this.itemTypes.length > 0) {
      this.editForm.itemTypeId = this.itemTypes[0].id;
    }
    if (this.statusTypes.length > 0) {
      this.editForm.statusTypeId = this.statusTypes[0].id;
    }

    this.buildParentTreeOptions();
    this.isEditing = true;
  }

  onEditItem() {
    if (!this.selectedItem) return;
    
    // Populate form from selected item
    this.editForm = {
      id: this.selectedItem.id,
      parentId: this.selectedItem.parentId,
      itemTypeId: this.selectedItem.itemTypeId,
      statusTypeId: this.selectedItem.statusTypeId,
      rank: this.selectedItem.rank,
      name: this.selectedItem.name,
      details: this.selectedItem.details,
      referenceFileSystemId: this.selectedItem.referenceFileSystemId,
      referenceObjectHierarchyId: this.selectedItem.referenceObjectHierarchyId
    };

    this.buildParentTreeOptions();
    this.isEditing = true;
  }

  onSaveItem() {
    if (!this.editForm.name.trim()) {
      alert('Name is required');
      return;
    }
    
    let selectedParentId = this.selectedParentNode?.data?.id ?? undefined;
    if (selectedParentId != null)
    {
      this.editForm.parentId = selectedParentId;
    }    
    this.isSaving = true;
    this.itemsService.addUpdateItem(this.editForm).subscribe({
      next: (savedItem) => {
        console.log('Item saved:', savedItem);
        this.isSaving = false;
        this.isEditing = false;
        this.selectedItem = savedItem;
        this.loadItems(); // Refresh tree
      },
      error: (err) => {
        console.error('Failed to save item:', err);
        this.isSaving = false;
        alert('Failed to save item. Check console for details.');
      }
    });
  }

  onCancelEdit() {
    this.isEditing = false;
    // If it was a new item (id=0), clear selection
    if (this.editForm.id === 0) {
      this.selectedItem = undefined;
    }
  }

  private buildParentTreeOptions(excludeId?: number): void {
    const noneOption: TreeNode = {
      key: 'none',
      label: '(None - Root Level)',
      data: null,
      selectable: true
    };    
    const buildNodes = (items: ItemDto[]): TreeNode[] => {
      return items
        .filter(item => item.id !== excludeId) // Exclude self (children excluded implicitly)
        .map(item => ({
          key: item.id.toString(),
          label: item.name,
          data: item,
          children: item.children ? buildNodes(item.children) : [],
          selectable: true
        }));
    };
    this.parentTreeOptions = [noneOption, ...buildNodes(this.items)];
    if (this.editForm.parentId) {
      this.selectedParentNode = this.findNodeByItemId(this.parentTreeOptions, this.editForm.parentId);
    } else {
      this.selectedParentNode = noneOption;
    }
  }

  private findNodeByItemId(nodes: TreeNode[], itemId: number): TreeNode | null {
    for (const node of nodes) {
      if (node.data?.id === itemId) {
        return node;
      }
      if (node.children?.length) {
        const found = this.findNodeByItemId(node.children, itemId);
        if (found) return found;
      }
    }
    return null;
  }

}
