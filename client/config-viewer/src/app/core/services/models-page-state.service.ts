import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ModelsPageStateService {
  private readonly STORAGE_KEY = 'models-page-state';

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
}