import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ModelsPageStateService {
  private readonly STORAGE_KEY = 'models-page-state';
  private readonly PROPERTIES_EXPANDED_KEY = 'models_page_properties_expanded';

  private getState(): any {
    const stored = localStorage.getItem(this.STORAGE_KEY);
    return stored ? JSON.parse(stored) : {};
  }

  private setState(state: any): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(state));
  }

  getProjectId(): number | null {
    return this.getState().projectId ?? null;
  }

  setProjectId(projectId: number | null): void {
    const state = this.getState();
    state.projectId = projectId;
    this.setState(state);
  }

  getSelectedModelId(): number | null {
    return this.getState().selectedModelId ?? null;
  }

  setSelectedModelId(modelId: number | null): void {
    const state = this.getState();
    state.selectedModelId = modelId;
    this.setState(state);
  }

  getActiveTab(): string {
    return this.getState().activeTab ?? '0';
  }

  setActiveTab(tab: string): void {
    const state = this.getState();
    state.activeTab = tab;
    this.setState(state);
  }

  clear(): void {
    localStorage.removeItem(this.STORAGE_KEY);
  } 

  getPropertiesExpanded(): boolean {
    const saved = localStorage.getItem(this.PROPERTIES_EXPANDED_KEY);
    return saved !== null ? saved === 'true' : true; // Default to true
  }

  setPropertiesExpanded(expanded: boolean): void {
    localStorage.setItem(this.PROPERTIES_EXPANDED_KEY, String(expanded));
  }
}