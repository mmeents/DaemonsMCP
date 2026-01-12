import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
})
export class LandingComponent {
  constructor(
    private router: Router, 
    private authService: AuthService,) {}

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  goToSwagger(): void {
    window.open('/swagger', '_blank');
  }
  
  get currentUser() {
    return this.authService.currentUserValue;
  }
}