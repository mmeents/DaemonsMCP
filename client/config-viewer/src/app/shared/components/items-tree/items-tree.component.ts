import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { ItemDto } from '../../models/api.models';
import { ContextMenuModule } from 'primeng/contextmenu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-items-tree',
  standalone: true,
  imports: [CommonModule, TreeModule, ContextMenuModule],
  styleUrl: './items-tree.component.scss',
  templateUrl: './items-tree.component.html'  
})
export class ItemsTreeComponent implements OnInit, OnChanges {
  @Input() items: ItemDto[] = [];
  @Input() loading = false;
  @Input() selectedItemId?: number;
  @Output() itemSelected = new EventEmitter<ItemDto>();
  @Output() itemMoved = new EventEmitter<{ item: ItemDto; newParentId?: number }>();
  @Output() itemAdd = new EventEmitter<ItemDto | null>();
  @Output() itemEdit = new EventEmitter<ItemDto>();
  @Output() itemDelete = new EventEmitter<ItemDto>();

  treeNodes: TreeNode[] = [];
  selectedNode?: TreeNode;
  expandedKeys: {[key: string]: boolean } = {};
  enableDragDrop = false;
  contextMenuItems: MenuItem[] = [];
  selectedNodeForContext?: TreeNode;

  ngOnInit() {
    this.buildTree();
  }

  ngOnChanges(changes: SimpleChanges) {
      if (changes['items']) {
      this.buildTree();
    } else if (changes['selectedItemId'] && this.selectedItemId) {
      this.selectedNode = this.findNodeById(this.treeNodes, this.selectedItemId);
    }
  }

  private buildTree() {
    this.treeNodes = this.convertToTreeNodes(this.items);
    this.enableDragDrop = false;

    if (this.selectedItemId) {
      this.selectedNode = this.findNodeById(this.treeNodes, this.selectedItemId);
    }
  }

  private restoreExpandedState(nodes: TreeNode[]) {
    nodes.forEach(node => {
      if (node.key && this.expandedKeys[node.key]) {
        node.expanded = true;
      }
      if (node.children?.length) {
        this.restoreExpandedState(node.children);
      }
    });
  }

  private findNodeById(nodes: TreeNode[], itemId: number): TreeNode | undefined {
    for (const node of nodes) {
      if (node.data?.id === itemId) {
        return node;
      }
      if (node.children?.length) {
        const found = this.findNodeById(node.children, itemId);
        if (found) return found;
      }
    }
    return undefined;
  }

  private convertToTreeNodes(items: ItemDto[]): TreeNode[] {
    if (!items || items.length === 0) {
      return [];
    }
    
    return items.map((item) => ({
      key: item.id.toString(),
      label: item.name,
      data: item,
      children: item.children ? this.convertToTreeNodes(item.children) : [],
      icon: this.getIconForType(item.itemTypeName),
      styleClass: this.getStyleClassForStatus(item.statusTypeName),
      draggable: false,
      droppable: false,
    }));
  }

  onContextMenu(event: any) {
    this.selectedNodeForContext = event.node;
    const item = event.node.data as ItemDto;
  
    this.contextMenuItems = [
      {
        label: 'Add Child',
        icon: 'pi pi-plus',
        command: () => this.onAddChild()
      },
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => this.onEdit()
      },
      {
        separator: true
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.onDelete()
      }
    ];
  }

  private onAddChild() {
    if (this.selectedNodeForContext) {
      const parentItem = this.selectedNodeForContext.data as ItemDto;
      this.itemAdd.emit(parentItem);
    }
  }

  private onEdit() {
    if (this.selectedNodeForContext) {
      const item = this.selectedNodeForContext.data as ItemDto;
      this.itemEdit.emit(item);
    }
  }

  private onDelete(){
    if (this.selectedNodeForContext) {
      const item = this.selectedNodeForContext.data as ItemDto;
      this.itemDelete.emit(item);
    }
  }

  private getIconForType(typeName: string): string {    
    const iconMap: { [key: string]: string } = {
      Task: 'pi pi-check-square',
      Todo: 'pi pi-list',
      Feature: 'pi pi-star',
      Bug: 'pi pi-exclamation-circle',
      Note: 'pi pi-file',
      Project: 'pi pi-folder',
      Epic: 'pi pi-sitemap',
    };
    return iconMap[typeName] || 'pi pi-circle';
  }

  private getStyleClassForStatus(statusName: string): string {    
    const normalized = statusName.toLowerCase().replace(/\s+/g, '-');
    return `status-${normalized}`;
  }

  onSelect(event: any) {
    const item = event.node.data as ItemDto;
    this.itemSelected.emit(item);
  }

  onNodeDrop(event: any) {
    console.log('Drop event:', event);
    const droppedItem = event.dragNode.data as ItemDto;
    const newParentId = event.dropNode?.data?.id;
    
    this.itemMoved.emit({ item: droppedItem, newParentId });
  }
}
