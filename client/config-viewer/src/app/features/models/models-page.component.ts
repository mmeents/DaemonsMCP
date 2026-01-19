import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TabsModule } from 'primeng/tabs';
import { TreeSelectModule } from 'primeng/treeselect';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { TreeNode } from 'primeng/api';
import { CheckboxModule } from 'primeng/checkbox';
import { DatePickerModule } from 'primeng/datepicker';

import { ModelsTreeComponent } from '../../shared/components/models-tree/models-tree.component';
import { ProjectsService } from '../../core/services/projects.service';
import { ModelsService } from '../../core/services/models.service';
import { ModelsPageStateService } from '../../core/services/models-page-state.service';
import { ModelTypeService, Mte } from '../../core/services/model-type.service';
import { FileViewerComponent } from '../../shared/components/file-viewer/file-viewer.component';
import { 
  Project, 
  ModelDto, 
  ModelTypeDto,
  AddUpdateModelRequest, 
  ModelPropertyDto,
  AddUpdateModelPropertyRequest
} from '../../shared/models/api.models';

@Component({
  selector: 'app-models-page',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ModelsTreeComponent, 
    TreeSelectModule, 
    DialogModule, 
    SelectModule,    
    CheckboxModule,
    DatePickerModule,
    FileViewerComponent,
    TabsModule
  ],
  templateUrl: './models-page.component.html',
  styleUrl: './models-page.component.scss',
})
export class ModelsPageComponent implements OnInit {
  // Services
  private projectsService = inject(ProjectsService);
  private modelsService = inject(ModelsService);
  private stateService = inject(ModelsPageStateService);
  private modelTypeService = inject(ModelTypeService);

  // Signals for state
  protected projects = signal<Project[]>([]);
  protected selectedProject = signal<Project | null>(null);
  protected models = signal<ModelDto[]>([]);
  protected selectedModel = signal<ModelDto | null>(null);
  protected loading = signal(true);
  protected modelsLoading = signal(false);
  protected activeTab = signal('0');
  protected modelProperties = signal<ModelPropertyDto[]>([]);
  protected isAddingProperty = signal(false);
  protected newProperty = {
    propertyKey: '',
    propertyValue: '',
    propertyValueTypeId: undefined as number | undefined,
    propertyEditorTypeId: undefined as number | undefined
  };
  protected modelTypeChildren = signal<Map<number, ModelTypeDto[]>>(new Map());

  // Edit form state
  protected isEditing = signal(false);
  protected isSaving = signal(false);
  protected editForm: AddUpdateModelRequest = this.createEmptyForm();
  
  protected modelTypes = signal<ModelTypeDto[]>([]);
  protected parentTreeOptions: TreeNode[] = [];
  protected selectedParentNode: TreeNode | null = null;
  protected get Mte() { return this.modelTypeService.Mte; }
  protected propertiesExpanded = signal(true); // Start expanded by default
  protected previewLoading = signal(false);
  protected previewError = signal<string | null>(null);
  protected showImportTableDialog = signal(false);
  protected importTableSql = signal('');
  protected importTargetParent?: ModelDto;

  // Delete dialog
  protected showDeleteDialog = signal(false);
  protected modelToDelete?: ModelDto;

  ngOnInit() {
    this.loadProjects();
    this.loadModelTypes();
    
    // Restore properties panel state
    const savedExpanded = this.stateService.getPropertiesExpanded();
    this.propertiesExpanded.set(savedExpanded);
  }

  protected togglePropertiesPanel() {
    this.propertiesExpanded.update(v => !v);
  }

  private createEmptyForm(projectId?: number, parentId?: number): AddUpdateModelRequest {
    return {
      id: 0,
      projectId: projectId ?? 0,
      parentId: parentId,
      modelTypeId: 0,
      name: '',
      rank: 0,
      code: undefined
    };
  }

  private loadProjects() {
    this.loading.set(true);
    
    this.projectsService.getProjects().subscribe({
        next: (projects) => {
        // Sort projects by name for consistent ordering
        const sortedProjects = projects.sort((a, b) => a.name.localeCompare(b.name));
        this.projects.set(sortedProjects);
        this.loading.set(false);
        
        // Try to restore saved project first
        const savedProjectId = this.stateService.getProjectId();
        if (savedProjectId) {
            const savedProject = sortedProjects.find(p => p.id === savedProjectId);
            if (savedProject) {
            this.selectProject(savedProject);
            return;
            }
        }
        
        // Auto-select first project (now guaranteed to match dropdown order)
        if (sortedProjects.length > 0) {
            this.selectProject(sortedProjects[0]);
        }
        },
        error: (err) => {
        console.error('Error loading projects:', err);
        this.loading.set(false);
        }
    });
  }

  // Use helper methods
protected getEditorForProperty(prop: ModelPropertyDto): number {
  return this.modelTypeService.getEditorType(prop.propertyEditorTypeId ?? prop.propertyValueTypeId);
}
  
protected isReferenceType(typeId: number | null | undefined): boolean {
  return this.modelTypeService.isReferenceType(typeId);
}

protected getTypeChildren(parentTypeId: number): ModelTypeDto[] {
  return this.modelTypeChildren().get(parentTypeId) || [];
}

  onProjectChange(projectIdStr: string) {
    const projectId = parseInt(projectIdStr, 10);
    const project = this.projects().find(p => p.id === projectId);
    if (project) {
        this.selectProject(project);
    }
  }

  private loadModelTypes() {
    this.modelsService.getProjectTemplates().subscribe({
      next: (types) => {
        this.modelTypes.set(types);
      },
      error: (err) => console.error('Failed to load model types:', err)
    });
  }

  selectProject(project: Project) {
    this.selectedProject.set(project);
    this.stateService.setProjectId(project.id);
    this.selectedModel.set(null);
    this.stateService.setSelectedModelId(null);
    this.loadModels();
  }

  private loadModels() {
    const project = this.selectedProject();
    if (!project) return;

    this.modelsLoading.set(true);
    
    this.modelsService.searchModels({
      projectId: project.id,
      maxDepth: 3,
      includeProperties: false
    }).subscribe({
      next: (models) => {
        this.models.set(models);
        this.modelsLoading.set(false);
        
        // Try to restore selected model
        const savedModelId = this.stateService.getSelectedModelId();
        if (savedModelId) {
          const savedModel = this.findModelById(models, savedModelId);
          if (savedModel) {
            this.selectedModel.set(savedModel);
          }
        }
      },
      error: (err) => {
        console.error('Failed to load models:', err);
        this.modelsLoading.set(false);
      }
    });
  }

  private findModelById(models: ModelDto[], id: number): ModelDto | null {
    for (const model of models) {
      if (model.id === id) return model;
      if (model.children?.length) {
        const found = this.findModelById(model.children, id);
        if (found) return found;
      }
    }
    return null;
  }

  onModelSelected(model: ModelDto) {
    this.selectedModel.set(model);
    this.stateService.setSelectedModelId(model.id);
    this.loadModelProperties();
    if (this.isTemplateType(model.modelTypeId)&& !this.isEditing() && model.code != null) {
      this.onExecuteTemplate(model, true)
    } else if (this.isTableModelType(model.modelTypeId) && !this.isEditing()) {
      this.onExecuteTemplate(model, true)
    }
    console.log('MODEL SELECTED EVENT RECEIVED:', model);
  }

  onNewModel(event?: { parent: ModelDto | null; modelTypeId?: number }) {
    console.log('NEW MODEL EVENT RECEIVED:', event);
    const project = this.selectedProject();
    if (!project) return;   

    const parentId = event?.parent?.id ?? this.selectedModel()?.id;
    this.editForm = this.createEmptyForm(project.id, parentId);
    
    const parent = event?.parent ?? this.selectedModel();
    if (parent?.children?.length) {
      this.editForm.rank = parent.children.length + 1;
    } else {
      this.editForm.rank = 1;
    }
    
    // If a specific modelTypeId was provided (from root context menu), use it
    if (event?.modelTypeId) {
        this.editForm.modelTypeId = event.modelTypeId;
    } else {
        // Default to first available type
        const types = this.modelTypes();
        if (types.length > 0) {
        this.editForm.modelTypeId = types[0].id;
        }
    }

    this.buildParentTreeOptions();
    this.isEditing.set(true);
  }

  onEditModel(model?: ModelDto) {
    console.log('EDIT MODEL EVENT RECEIVED:', model);
    const modelToEdit = model ?? this.selectedModel();
    if (!modelToEdit) return;
    
    this.editForm = {
      id: modelToEdit.id,
      projectId: modelToEdit.projectId,
      parentId: modelToEdit.parentId,
      modelTypeId: modelToEdit.modelTypeId,
      name: modelToEdit.name,
      rank: modelToEdit.rank,
      code: modelToEdit.code
    };

    this.buildParentTreeOptions(this.editForm.id);
    this.isEditing.set(true);
  }

  onDeleteModel(model: ModelDto) {
    this.modelToDelete = model;
    this.showDeleteDialog.set(true);
  }

  confirmDelete() {
    if (!this.modelToDelete) return;
    const parentId = this.modelToDelete.parentId;
    this.modelsService.deleteModel(this.modelToDelete.id).subscribe({
      next: () => {
        console.log('Model deleted successfully');

        if (parentId) {
          const parent = this.findModelById(this.models(), parentId);
          this.selectedModel.set(parent);
          this.stateService.setSelectedModelId(parentId);
        } else {
          this.selectedModel.set(null);
          this.stateService.setSelectedModelId(null);
        }

        this.showDeleteDialog.set(false);
        this.loadModels();
      },
      error: (err) => {
        console.error('Failed to delete model:', err);
        alert('Failed to delete model. ' + (err.error || 'Check console for details.'));
        this.showDeleteDialog.set(false);
      }
    });
  }

  cancelDelete() {
    this.showDeleteDialog.set(false);
    this.modelToDelete = undefined;
  }

  onSaveModel() {
    if (!this.editForm.name.trim()) {
      alert('Name is required');
      return;
    }
    
    if (this.selectedParentNode?.data?.id != null) {
      this.editForm.parentId = this.selectedParentNode.data.id;
    }
    
    this.isSaving.set(true);
    this.modelsService.addUpdateModel(this.editForm).subscribe({
      next: (savedModel) => {
        console.log('Model saved:', savedModel);
        this.isSaving.set(false);
        this.isEditing.set(false);
        this.selectedModel.set(savedModel);
        this.stateService.setSelectedModelId(savedModel.id);
        this.loadModelProperties();
        this.loadModels();
      },
      error: (err) => {
        console.error('Failed to save model:', err);
        this.isSaving.set(false);
        alert('Failed to save model. Check console for details.');
      }
    });
  }

  onCancelEdit() {
    this.isEditing.set(false);
    if (this.editForm.id === 0) {
      this.selectedModel.set(null);
    }
  }

  private buildParentTreeOptions(excludeId?: number): void {
    const noneOption: TreeNode = {
      key: 'none',
      label: '(None - Root Level)',
      data: null,
      selectable: true
    };
    
    const buildNodes = (models: ModelDto[]): TreeNode[] => {
      return models
        .filter(model => model.id !== excludeId)
        .map(model => ({
          key: model.id.toString(),
          label: model.name,
          data: model,
          children: model.children ? buildNodes(model.children) : [],
          selectable: true
        }));
    };
    
    this.parentTreeOptions = [noneOption, ...buildNodes(this.models())];
    
    if (this.editForm.parentId) {
      this.selectedParentNode = this.findNodeByModelId(this.parentTreeOptions, this.editForm.parentId);
    } else {
      this.selectedParentNode = noneOption;
    }
  }

  private findNodeByModelId(nodes: TreeNode[], modelId: number): TreeNode | null {
    for (const node of nodes) {
      if (node.data?.id === modelId) {
        return node;
      }
      if (node.children?.length) {
        const found = this.findNodeByModelId(node.children, modelId);
        if (found) return found;
      }
    }
    return null;
  }

  onTabChange(event: any) {
    this.activeTab.set(event.index.toString());
    this.stateService.setActiveTab(event.index.toString());
    
    // Load properties when switching to properties tab
    if (event.index === 1 ) {
      this.loadModelProperties();
    }
  }

  private loadModelProperties() {
    const model = this.selectedModel();
    if (!model) {
      this.modelProperties.set([]);
      return;
    }

    this.modelsService.getModelProperties(model.id).subscribe({
      next: (properties) => {
        this.modelProperties.set(properties);
        const referenceTypeIds = properties
          .filter(p => this.isReferenceType(p.propertyValueTypeId))
          .map(p => p.propertyValueTypeId!)
          .filter((id, idx, arr) => arr.indexOf(id) === idx); // unique
        
        referenceTypeIds.forEach(typeId => this.loadModelTypeChildren(typeId));
      },
      error: (err) => {
        console.error('Failed to load properties:', err);
      }
    });
  }

  async loadModelTypeChildren(parentTypeId: number) {
    if (this.modelTypeChildren().has(parentTypeId)) {
      return; // Already loaded
    }
    
    this.modelsService.getValidChildTypes(parentTypeId).subscribe({
      next: (children) => {
        this.modelTypeChildren.update(map => {
          map.set(parentTypeId, children);
          return new Map(map); // Create new map to trigger signal
        });
      },
      error: (err) => console.error('Failed to load model type children:', err)
    });
  }

  onAddProperty() {
    const model = this.selectedModel();
    if (!model) return;

    if (!this.newProperty.propertyKey.trim()) {
      alert('Property key is required');
      return;
    }

    const request: AddUpdateModelPropertyRequest = {
      id: 0,
      modelId: model.id,
      propertyKey: this.newProperty.propertyKey,
      propertyValue: this.newProperty.propertyValue?.toString() ?? undefined,
      propertyValueTypeId: this.newProperty.propertyValueTypeId,
      propertyEditorTypeId: this.newProperty.propertyEditorTypeId,
    };

    this.modelsService.addUpdateModelProperty(request).subscribe({
      next: (property) => {
        console.log('Property added:', property);
        this.modelProperties.update(props => [...props, property]);        
        this.newProperty = { propertyKey: '', propertyValue: '', propertyValueTypeId: undefined, propertyEditorTypeId: undefined };
        this.isAddingProperty.set(false);
      },
      error: (err) => {
        console.error('Failed to add property:', err);
        alert('Failed to add property. Check console for details.');
      }
    });
  }

  onUpdateProperty(property: ModelPropertyDto) {
    const request: AddUpdateModelPropertyRequest = {
      id: property.id,
      modelId: property.modelId,
      propertyKey: property.propertyKey,
      propertyValue: property.propertyValue?.toString() ?? undefined,
      propertyValueTypeId: property.propertyValueTypeId,
      propertyEditorTypeId: property.propertyEditorTypeId
    };

    this.modelsService.addUpdateModelProperty(request).subscribe({
      next: (updated) => {
        console.log('Property updated:', updated);
        this.modelProperties.update(props => 
          props.map(p => p.id === updated.id ? updated : p)
        );
        // 🔥 Auto-refresh the preview
        const model = this.selectedModel();
        if (model && this.isTemplateType(model.modelTypeId) && model.code) {
          this.onExecuteTemplate(model, true);
        } else if (model && this.isTableModelType(model.modelTypeId)) {
          this.onExecuteTemplate(model, true);
        }
      },
      error: (err) => {
        console.error('Failed to update property:', err);
        alert('Failed to update property. Check console for details.');
      }
    });
  }

  onDeleteProperty(property: ModelPropertyDto) {
    if (!confirm(`Delete property "${property.propertyKey}"?`)) return;

    // You'll need to add this to ModelsService 
    
    this.modelsService.deleteModelProperty(property.id).subscribe({
      next: () => {
        console.log('Property deleted');
        this.modelProperties.update(props => props.filter(p => p.id !== property.id));
      },
      error: (err) => {
        console.error('Failed to delete property:', err);
        alert('Failed to delete property. Check console for details.');
      }
    });  
  }

  protected getTargetModelOptions(templateTypeId: number): ModelDto[] {
    const targetTypeId = this.modelTypeService.getTargetModelType(templateTypeId);
    if (!targetTypeId) return [];
    return this.findModelsByType(this.models(), targetTypeId);
  }

  // Recursive search through the model tree
  private findModelsByType(models: ModelDto[], targetTypeId: number): ModelDto[] {
    const results: ModelDto[] = [];
    
    for (const model of models) {
      // Check if this model matches
      if (model.modelTypeId === targetTypeId) {
        results.push(model);
      }
      
      // Recursively check children
      if (model.children?.length) {
        if (targetTypeId == this.modelTypeService.Mte.TableColumnModel && model.modelTypeId == this.modelTypeService.Mte.TableModel) {
          const childMatches = this.findModelsByType(model.children, targetTypeId);
          for (const match of childMatches) {
            const newName = `${model.name}.${match.name}`;
            const renamedMatch: ModelDto = { ...match, name: newName };
            results.push(renamedMatch);
          }
        } else {
          const childMatches = this.findModelsByType(model.children, targetTypeId);
          results.push(...childMatches);
        }       
        
      }
    }
    
    return results;
  }

  protected isTemplateType(modelTypeId: number): boolean {
    return this.modelTypeService.isTemplateType(modelTypeId);
  }

  protected isTableModelType(modelTypeId: number): boolean {
    return this.modelTypeService.isTableModelType(modelTypeId);
  }

  protected showPreviewDialog = signal(false);
  protected previewCode = signal('');
  protected previewFileName = signal('');
  protected onExecuteTemplate(template: ModelDto, preview: boolean = true) {
    this.modelsService.executeTemplate({ templateId: template.id, saveToFile: !preview }).subscribe({
      next: (result) => {
        if (result.success) {
          if (preview) {
            this.previewCode.set(result.renderedCode);
            this.previewFileName.set(result.relativeFileName);            
          } else {
            alert(`Generated: ${result.relativeFileName}`);
          }
        } else {
          alert(`Execution failed: ${result.errorMessage}`);
        }
      },
      error: (err) => {
        console.error('Template execution error:', err);
        alert('Template execution failed. Check console.');
      }
    });
  }

  onImportTable(parent: ModelDto) {
    this.importTargetParent = parent;
    this.importTableSql.set('');
    this.showImportTableDialog.set(true);
  }

  confirmImportTable() {
    if (!this.importTableSql().trim() || !this.importTargetParent) return;    
    
    this.modelsService.importTable({
      parentId: this.importTargetParent.id,
      sqlStatement: this.importTableSql()
    }).subscribe({
      next: (result) => {
        console.log('Table imported.');
        this.showImportTableDialog.set(false);
        this.loadModels();
      },
      error: (err) => {
        console.error('Import failed:', err);
        alert('Import failed. Check console for details.');
      }
    });
  }

  cancelImportTable() {
    this.showImportTableDialog.set(false);
    this.importTableSql.set('');
  }

}