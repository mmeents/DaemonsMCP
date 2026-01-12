import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';
import { Button, ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, ButtonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',    
})
export class App {
  protected readonly title = signal('DaemonsMCP Config Viewer');

  constructor(
    private authService: AuthService,    
    private router: Router
  ) {}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  get currentUser() {
    return this.authService.currentUserValue;
  }
}