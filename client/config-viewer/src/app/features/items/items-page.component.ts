import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemsTreeComponent } from '../../shared/components/items-tree/items-tree.component';
import { ItemsService } from '../../core/services/items.service';
import { ItemDto, AddUpdateItemRequest } from '../../shared/models/api.models';

@Component({
  selector: 'app-items-page',
  standalone: true,
  imports: [CommonModule, ItemsTreeComponent],
  templateUrl: './items-page.component.html',
  styleUrl: './items-page.component.scss',
})
export class ItemsPageComponent implements OnInit {
  items: ItemDto[] = [];
  selectedItem?: ItemDto;
  loading = false;

  constructor(private itemsService: ItemsService) {}

  ngOnInit() {
    this.loadItems();
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
    // TODO: Open modal or form to create new item
    // For now, just log that the button was clicked
    console.log('New item button clicked');
    console.log('Current selection:', this.selectedItem);
    
    // You could pre-fill the parent with selectedItem if one is selected
    alert('Item creation form coming soon!\n\nWill create item under: ' + 
          (this.selectedItem ? this.selectedItem.name : 'root'));
  }
}
