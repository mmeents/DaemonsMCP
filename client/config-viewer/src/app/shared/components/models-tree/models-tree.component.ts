import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { ModelDto } from '../../models/api.models';
import { ContextMenu, ContextMenuModule } from 'primeng/contextmenu';
import { MenuItem } from 'primeng/api';
import { ModelsService } from '../../../core/services/models.service';
import { Mte } from '../../../core/services/model-type.service';

@Component({
  selector: 'app-models-tree',
  standalone: true,
  imports: [CommonModule, TreeModule, ContextMenuModule],
  styleUrl: './models-tree.component.scss',
  templateUrl: './models-tree.component.html'  
})
export class ModelsTreeComponent implements OnInit, OnChanges {
  @Input() models: ModelDto[] = [];
  @Input() loading = false;
  @Input() selectedModelId?: number;
  @Output() modelSelected = new EventEmitter<ModelDto>();
  @Output() modelMoved = new EventEmitter<{ model: ModelDto; newParentId?: number }>();
  @Output() modelAdd = new EventEmitter<{ parent: ModelDto | null; modelTypeId?: number }>();
  @Output() modelEdit = new EventEmitter<ModelDto>();
  @Output() modelDelete = new EventEmitter<ModelDto>();

  @ViewChild('cm') contextMenu!: ContextMenu;
  private modelsService = inject(ModelsService);

  treeNodes: TreeNode[] = [];
  selectedNode?: TreeNode;
  expandedKeys: {[key: string]: boolean } = {};
  nodeContextMenuItems: MenuItem[] = [];  // For nodes
  rootContextMenuItems: MenuItem[] = [];  // For empty space
  selectedNodeForContext?: TreeNode;

  ngOnInit() {
    this.buildTree();
    this.initializeContextMenus();    
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['models']) {
      this.buildTree();
    } else if (changes['selectedModelId'] && this.selectedModelId) {
      this.selectedNode = this.findNodeById(this.treeNodes, this.selectedModelId);
    }
  }

  private readonly TYPE_HIERARCHY: { [parentTypeId: number]: { label: string, typeId: number }[] } = {
    201: [  // Database parent
      { label: 'Add Tables Folder', typeId: 202 }  // Tables folder
    ],
    202: [  // Tables folder parent
      { label: 'Add Table', typeId: 203 }  // Table
    ],
    203: [  // Table parent
      { label: 'Add Column', typeId: 204 }  // TableColumn
    ],
    301: [  // API Project parent
      { label: 'Add Endpoints Folder', typeId: 302 }  // Endpoints folder
    ],
    302: [  // Endpoints folder parent
      { label: 'Add Endpoint', typeId: 303 }  // Endpoint
    ]
    // Add more mappings as needed
  };

  private expandAll() {
    this.expandedKeys = {};
    this.expandRecursive(this.treeNodes, true);
  }

  private expandRecursive(nodes: TreeNode[], isExpand: boolean) {
    nodes.forEach(node => {
      if (node.key) {
        this.expandedKeys[node.key] = isExpand;
      }
      if (node.children) {
        this.expandRecursive(node.children, isExpand);
      }
    });
  }

  private initializeContextMenus() {
    // Root context menu (for empty space) - Note: adjust ModelTypeIds to match your seed data
    this.rootContextMenuItems = [
      {
        label: 'Add Database Project',
        icon: 'pi pi-database',
        command: () => this.onAddRootModel(Mte.DatabaseModel) // Adjust ID as needed
      },
      {
        label: 'Add API Project',
        icon: 'pi pi-server',
        command: () => this.onAddRootModel(Mte.ApiModel) // Adjust ID as needed
      },
      {
        label: 'Add Template Root',
        icon: 'pi pi-file',
        command: () => this.onAddRootModel(Mte.RootTemplate) // Adjust ID as needed
      }
    ];

    // Node context menu (for clicking on nodes) - will be updated per node
    this.nodeContextMenuItems = [
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

  private buildTree() {
    this.treeNodes = this.convertToTreeNodes(this.models);    
    if (this.selectedModelId) {
      this.selectedNode = this.findNodeById(this.treeNodes, this.selectedModelId);
    }
  }

  private findNodeById(nodes: TreeNode[], modelId: number): TreeNode | undefined {
    for (const node of nodes) {
      if (node.data?.id === modelId) {
        return node;
      }
      if (node.children?.length) {
        const found = this.findNodeById(node.children, modelId);
        if (found) {
          found.expanded = true; 
          node.expanded = true;
          return found;
        }
      }
    }
    return undefined;
  }

  private convertToTreeNodes(models: ModelDto[]): TreeNode[] {
    if (!models || models.length === 0) {
      return [];
    }
    
    return models.map((model) => ({
      key: model.id.toString(),
      label: model.name,
      data: model,
      children: model.children ? this.convertToTreeNodes(model.children) : [],
      icon: this.getIconForType(model.modelTypeName),
      draggable: false,
      droppable: false
    }));
  }

  onNodeContextMenu(event: any) {
    console.log('nNodeContextMenu CurretContext:', this.selectedNodeForContext);
    
  this.selectedNodeForContext = event.node;
  const model = event.node.data as ModelDto;
  console.log('model:', model);
  var modelTypeId = model.modelTypeId;
  if (modelTypeId >= Mte.RootTemplate && modelTypeId < 500) {
    modelTypeId = Mte.RootTemplate;  // Enable all templates to have any child templates.
  }
  
  // Fetch valid child types from backend
  this.modelsService.getValidChildTypes(modelTypeId).subscribe({
    next: (validTypes) => {
      this.nodeContextMenuItems = [];
      
      // Add "Add X" option for each valid child type
      validTypes.forEach(type => {
        this.nodeContextMenuItems.push({
          label: `Add ${type.name}`,
          icon: 'pi pi-plus',
          command: () => this.onAddChildWithType(type.id)
        });
      });
      
      // Always allow edit
      if (this.nodeContextMenuItems.length > 0) {
        this.nodeContextMenuItems.push({ separator: true });
      }
      
      this.nodeContextMenuItems.push({
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => this.onEdit()
      });
      
      this.nodeContextMenuItems.push({ separator: true });
      
      this.nodeContextMenuItems.push({
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.onDelete()
      });
    },
    error: (err) => {
      console.error('Failed to load valid child types:', err);
      // Fallback to generic "Add Child"      
    }
  });
}

  private onAddChildWithType(modelTypeId: number) {
    if (this.selectedNodeForContext) {
      const parentModel = this.selectedNodeForContext.data as ModelDto;
      this.modelAdd.emit({ parent: parentModel, modelTypeId });
    }
  }

 

  // Handle right-click on empty tree area
  onTreeBackgroundContextMenu(event: MouseEvent) {
    event.preventDefault();
    event.stopPropagation();
    
    // Show the context menu at the click position
    if (this.contextMenu) {
      this.contextMenu.show(event);
    }
  }

  private onAddRootModel(modelTypeId: number) {
    this.modelAdd.emit({ parent: null, modelTypeId });
  }

  private onAddChild() {
    console.log('Add child clicked, selectedNodeForContext:', this.selectedNodeForContext);
    if (this.selectedNodeForContext) {
      const parentModel = this.selectedNodeForContext.data as ModelDto;
      console.log('Emitting modelAdd event for parent:', parentModel);
      this.modelAdd.emit({ parent: parentModel });
    }
  }

 private onEdit() {
    console.log('Edit clicked, selectedNodeForContext:', this.selectedNodeForContext);
    if (this.selectedNodeForContext) {
      const model = this.selectedNodeForContext.data as ModelDto;
      console.log('Emitting modelEdit event for:', model);
      this.modelEdit.emit(model);
    }
  }

  private onDelete() {
    console.log('Delete clicked, selectedNodeForContext:', this.selectedNodeForContext);
    if (this.selectedNodeForContext) {
      const model = this.selectedNodeForContext.data as ModelDto;
      console.log('Emitting modelDelete event for:', model);
      this.modelDelete.emit(model);
    }
  }

  private getIconForType(typeName: string): string {
    const iconMap: { [key: string]: string } = {
      Database: 'pi pi-database',
      Table: 'pi pi-table',
      View: 'pi pi-eye',
      Column: 'pi pi-list',
      StoredProcedure: 'pi pi-cog',
      Function: 'pi pi-calculator',
      API: 'pi pi-server',
      Endpoint: 'pi pi-directions',
    };
    return iconMap[typeName] || 'pi pi-circle';
  }

  onSelect(event: any) {
    const model = event.node.data as ModelDto;
    this.selectedNodeForContext = event.node;
    this.onNodeContextMenu(event);
    this.modelSelected.emit(model);
  }

  onNodeDrop(event: any) {
    const droppedModel = event.dragNode.data as ModelDto;
    const newParentId = event.dropNode?.data?.id;
    
    this.modelMoved.emit({ model: droppedModel, newParentId });
  }
}