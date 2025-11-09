import { Routes } from '@angular/router';
import { FilesPageComponent } from './features/files/files-page.component';
import { ItemsPageComponent } from './features/items/items-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/files', pathMatch: 'full' },
  { path: 'files', component: FilesPageComponent },
  { path: 'items', component: ItemsPageComponent },
  // Future: projects page
  // { path: 'projects', component: ProjectsPageComponent },
];