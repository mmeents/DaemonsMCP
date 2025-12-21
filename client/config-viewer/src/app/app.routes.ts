import { Routes } from '@angular/router';
import { FilesPageComponent } from './features/files/files-page.component';
import { ItemsPageComponent } from './features/items/items-page.component';
import { ProjectsPageComponent } from './features/projects/projects-page.component';
import { AccessTokensPageComponent } from './features/access-tokens/access-tokens-page.component';
import { authGuard } from './core/guards/auth.guard';
import { loginGuard } from './core/guards/login.guard';
import { UsersPageComponent } from './features/users/users-page.component';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/landing/landing.component').then(m => m.LandingComponent) },
  { path: 'login', canActivate:[loginGuard],
    loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', canActivate:[loginGuard],
    loadComponent: () => import('./features/register/register.component').then(m => m.RegisterComponent) },

  { path: '', canActivate:[authGuard], children:[
    { path: '', redirectTo: '/files', pathMatch: 'full' },
    { path: 'files', component: FilesPageComponent },
    { path: 'items', component: ItemsPageComponent },
    { path: 'projects', component: ProjectsPageComponent },
    { path: 'access-tokens',  component: AccessTokensPageComponent },  
    { path: 'users', component: UsersPageComponent }
  ]},
  { path: '**', redirectTo: '' }
];