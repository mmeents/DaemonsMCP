import { Routes } from '@angular/router';
import { FilesPageComponent } from './features/files/files-page.component';
import { ItemsPageComponent } from './features/items/items-page.component';
import { ProjectsPageComponent } from './features/projects/projects-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/files', pathMatch: 'full' },
  { path: 'files', component: FilesPageComponent },
  { path: 'items', component: ItemsPageComponent },
  { path: 'projects', component: ProjectsPageComponent }  
];