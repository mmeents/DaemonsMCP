import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { Project } from '../../models/api.models';
import { ContextMenuModule } from 'primeng/contextmenu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-projects-tree',
  standalone: true,
  imports: [CommonModule, TableModule, ContextMenuModule],
  styleUrl: './projects-tree.component.scss',
  templateUrl: './projects-tree.component.html'  
})
export class ProjectsTreeComponent implements OnInit, OnChanges {
  @Input() projects: Project[] = [];
  @Input() loading = false;
  @Input() selectedProjectId?: number;
  @Output() projectSelected = new EventEmitter<Project>();
  @Output() projectAdd = new EventEmitter<void>();
  @Output() projectEdit = new EventEmitter<Project>();
  @Output() projectDelete = new EventEmitter<Project>();

  selectedProject?: Project;
  contextMenuItems: MenuItem[] = [];

  ngOnInit() {
    this.updateSelection();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['projects'] || changes['selectedProjectId']) {
      this.updateSelection();
    }
  }

  private updateSelection() {
    if (this.selectedProjectId) {
      this.selectedProject = this.projects.find(p => p.id === this.selectedProjectId);
    }
  }

  onRowSelect(event: any) {
    this.projectSelected.emit(event.data);
  }

  onContextMenu(event: any, project: Project) {
    this.selectedProject = project;
    
    this.contextMenuItems = [
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

  private onEdit() {
    if (this.selectedProject) {
      this.projectEdit.emit(this.selectedProject);
    }
  }

  private onDelete() {
    if (this.selectedProject) {
      this.projectDelete.emit(this.selectedProject);
    }
  }
}