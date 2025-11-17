import { Component, Input, Output, EventEmitter, OnInit, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { ItemDto } from '../../models/api.models';

@Component({
  selector: 'app-items-tree',
  standalone: true,
  imports: [CommonModule, TreeModule],
  template: `
    <div class="tree-wrapper">
      <p-tree
        [value]="treeNodes"
        [draggableNodes]="enableDragDrop"
        [droppableNodes]="enableDragDrop"
        (onNodeDrop)="onNodeDrop($event)"
        selectionMode="single"
        [(selection)]="selectedNode"
        (onNodeSelect)="onSelect($event)"
        [loading]="loading"
        [style]="{ width: '100%' }"
      >
      </p-tree>
      <div *ngIf="treeNodes.length === 0 && !loading" class="empty-state">
        <p>No items found. Click "+ New Item" to create one.</p>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
    }
    
    .tree-wrapper {
      height: 100%;
      overflow: auto;
    }
    
    .empty-state {
      padding: 2rem;
      text-align: center;
      color: #6c757d;
    }
    
    /* Custom status styling */
    :host ::ng-deep .status-not-started {
      color: #6c757d;
    }
    
    :host ::ng-deep .status-in-progress {
      color: #0d6efd;
      font-weight: 500;
    }
    
    :host ::ng-deep .status-complete {
      color: #198754;
      text-decoration: line-through;
    }
    
    :host ::ng-deep .status-cancelled {
      color: #dc3545;
      text-decoration: line-through;
    }
  `]
})
export class ItemsTreeComponent implements OnInit, OnChanges {
  @Input() items: ItemDto[] = [];
  @Input() loading = false;
  @Output() itemSelected = new EventEmitter<ItemDto>();
  @Output() itemMoved = new EventEmitter<{ item: ItemDto; newParentId?: number }>();

  treeNodes: TreeNode[] = [];
  selectedNode?: TreeNode;
  enableDragDrop = false;

  ngOnInit() {
    this.buildTree();
  }

  ngOnChanges() {
    this.buildTree();
  }

  private buildTree() {
    this.treeNodes = this.convertToTreeNodes(this.items);
    
    // Enable drag-drop only after nodes are built and rendered
    // Short delay to let PrimeNG finish rendering
    if (this.treeNodes.length > 0) {
      setTimeout(() => {
        this.enableDragDrop = true;
      }, 100);
    } else {
      this.enableDragDrop = false;
    }
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
      draggable: true,
      droppable: true,
    }));
  }

  private getIconForType(typeName: string): string {
    // Map your item types to PrimeIcons
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
    // Add custom classes based on status - normalize to lowercase with hyphens
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
    
    // Emit the move event - parent component will handle the API call
    this.itemMoved.emit({ item: droppedItem, newParentId });
  }
}
