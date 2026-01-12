import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../core/services/user.service';
import { InvitationService } from '../../core/services/invitation.service';
import { RegisterWithInvitationCommand } from '../../shared/models/api.models';

import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CardModule,
    InputTextModule,
    ButtonModule,
    MessageModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  loading = false;
  validatingToken = false;
  invitationToken = '';
  tokenIsValid = false;
  invitedEmail? : string;

  constructor(
    private fb: FormBuilder,
    private invitationService: InvitationService,    
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
    this.registerForm.disable();
  }

  ngOnInit() {
    // Extract invitation token from URL query params (?token=xyz)
    this.route.queryParams.subscribe(params => {
      this.invitationToken = params['token'] || '';
      
      if (!this.invitationToken) {
        this.errorMessage = 'Invalid registration link. Please request a new invitation.';
      }
    });
    this.validateToken();
  }

  validateToken() {
    this.validatingToken = true;
    this.errorMessage = '';

    this.invitationService.validateInvitationToken(this.invitationToken).subscribe({
      next: (result) => {
        this.validatingToken = false;
        if (result.isUsed) {
          this.errorMessage = 'This invitation has already been used.';
          this.tokenIsValid = false;
          return;
        }    
        
        const now = new Date();
        const expiresAt = new Date(result.expiresAt);
        if (now > expiresAt) {
          this.errorMessage = 'This invitation has expired. Please request a new one.';
          this.tokenIsValid = false;
          return;
        }

        // Token is valid - enable form
        this.tokenIsValid = true;
        this.registerForm.enable();
        
        // Pre-fill email if invitation is for specific email
        if (result.invitedEmail) {
          this.invitedEmail = result.invitedEmail;
          this.registerForm.patchValue({ email: result.invitedEmail });
          this.registerForm.get('email')?.disable();  // Lock the email field
        }
      },
      error: (error) => {
        this.validatingToken = false;
        this.errorMessage = 'Failed to validate invitation. Please try again or request a new invitation.';
        console.error('Token validation error:', error);
      }
    });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  onSubmit(): void {
    if (!this.invitationToken || !this.registerForm.valid) {
      this.errorMessage = 'No invitation token found. Please use a valid registration link.';
      return;
    }
  
    this.loading = true;
    this.errorMessage = '';
    
    const formValue = this.registerForm.getRawValue();
    const command: RegisterWithInvitationCommand = {
      invitationToken: this.invitationToken,
      email: formValue.email,
      displayName: formValue.displayName,
      password: formValue.password
    };
    
    this.invitationService.registerWithInvitation(command).subscribe({
      next: (user) => {
        console.log('Registration successful:', user);
        this.successMessage = 'Registration successful! Redirecting to login...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        console.error('Registration failed:', error);
        this.errorMessage = error.error?.errorMessage || 'Registration failed. Please check your invitation link.';
        this.loading = false;
      }
    });
 
  }
}