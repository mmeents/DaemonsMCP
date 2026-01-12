import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserCredentialsService } from '../../../../core/services/user-credentials.service';
import { AuthService } from '../../../../core/services/auth.service';
import { 
  UserCredentialDto,
  CreateUserCredentialCommand,
  UpdateUserCredentialCommand,
  ProviderType,
  CredentialType
} from '../../../../shared/models/api.models';

// PrimeNG v20 Imports
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-git-credentials',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
    TableModule,
    TagModule,
    DialogModule,
    InputTextModule,
    SelectModule,
    CheckboxModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './git-credentials.component.html',
  styleUrl: './git-credentials.component.scss'
})
export class GitCredentialsComponent implements OnInit {
  credentials: UserCredentialDto[] = [];
  loading = false;
  
  // Dialog state
  showEditDialog = false;
  isEditing = false;
  isSaving = false;
  editForm!: FormGroup;
  
  showDeleteDialog = false;
  credentialToDelete?: UserCredentialDto;

  // Dropdown options
  providerOptions = [
    { label: 'GitHub', value: ProviderType.GitHub, icon: 'pi pi-github' },
    { label: 'Azure DevOps', value: ProviderType.AzureDevOps, icon: 'pi pi-microsoft' },
    { label: 'Bitbucket', value: ProviderType.Bitbucket, icon: 'pi pi-code' },
    { label: 'GitLab', value: ProviderType.GitLab, icon: 'pi pi-gitlab' }
  ];

  credentialTypeOptions = [
    { label: 'Personal Access Token', value: CredentialType.PersonalAccessToken },
    { label: 'Username/Password', value: CredentialType.UsernamePassword },
    { label: 'SSH Key', value: CredentialType.SshKey }
  ];

  constructor(
    private fb: FormBuilder,
    private credentialsService: UserCredentialsService,
    private authService: AuthService,
    private messageService: MessageService
  ) {
    this.initializeForm();
  }

  ngOnInit() {
    this.loadCredentials();
  }

  private initializeForm() {
    this.editForm = this.fb.group({
      id: [0],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      providerType: [ProviderType.GitHub, Validators.required],
      credentialType: [CredentialType.PersonalAccessToken, Validators.required],
      username: ['', Validators.required],
      secret: ['', Validators.required],
      isActive: [true]
    });
  }

  loadCredentials() {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) {
      console.error('No current user');
      return;
    }

    this.loading = true;
    this.credentialsService.getUserCredentials(currentUser.id).subscribe({
      next: (credentials) => {
        this.credentials = credentials;
        this.loading = false;
        console.log('Loaded credentials:', credentials);
      },
      error: (err) => {
        console.error('Failed to load credentials:', err);
        this.loading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load credentials'
        });
      }
    });
  }

  onAddCredential() {
    this.isEditing = false;
    this.editForm.reset({
      id: 0,
      name: '',
      providerType: ProviderType.GitHub,
      credentialType: CredentialType.PersonalAccessToken,
      username: '',
      secret: '',
      isActive: true
    });
    this.showEditDialog = true;
  }

  onEditCredential(credential: UserCredentialDto) {
    this.isEditing = true;
    this.editForm.patchValue({
      id: credential.id,
      name: credential.name,
      providerType: credential.providerType,
      credentialType: credential.credentialType,
      username: '',  // Don't populate encrypted values
      secret: '',    // Don't populate encrypted values
      isActive: credential.isActive
    });
    
    // Make username/secret optional for updates
    this.editForm.get('username')?.clearValidators();
    this.editForm.get('secret')?.clearValidators();
    this.editForm.get('username')?.updateValueAndValidity();
    this.editForm.get('secret')?.updateValueAndValidity();
    
    this.showEditDialog = true;
  }

  onSaveCredential() {
    this.editForm.markAllAsTouched();

    if (!this.editForm.valid) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Validation Error',
        detail: 'Please fix the errors in the form'
      });
      return;
    }

    const currentUser = this.authService.currentUserValue;
    if (!currentUser) return;

    const formValue = this.editForm.value;
    this.isSaving = true;

    if (this.isEditing && formValue.id > 0) {
      // Update existing credential
      const command: UpdateUserCredentialCommand = {
        id: formValue.id,
        name: formValue.name || undefined,
        username: formValue.username || undefined,
        secret: formValue.secret || undefined,
        isActive: formValue.isActive
      };

      this.credentialsService.updateCredential(formValue.id, command).subscribe({
        next: () => {
          this.handleSaveSuccess('Credential updated successfully');
        },
        error: (err) => {
          this.handleSaveError(err);
        }
      });
    } else {
      // Create new credential
      const command: CreateUserCredentialCommand = {
        userId: currentUser.id,
        name: formValue.name,
        providerType: formValue.providerType,
        credentialType: formValue.credentialType,
        username: formValue.username,
        secret: formValue.secret
      };

      this.credentialsService.createCredential(command).subscribe({
        next: () => {
          this.handleSaveSuccess('Credential created successfully');
        },
        error: (err) => {
          this.handleSaveError(err);
        }
      });
    }
  }

  private handleSaveSuccess(message: string) {
    console.log(message);
    this.isSaving = false;
    this.showEditDialog = false;
    this.loadCredentials();
    this.messageService.add({
      severity: 'success',
      summary: 'Success',
      detail: message
    });
  }

  private handleSaveError(err: any) {
    console.error('Failed to save credential:', err);
    this.isSaving = false;
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: err.error?.errorMessage || 'Failed to save credential'
    });
  }

  onCancelEdit() {
    this.showEditDialog = false;
    // Restore validators for next add
    this.editForm.get('username')?.setValidators(Validators.required);
    this.editForm.get('secret')?.setValidators(Validators.required);
  }

  onDeleteCredential(credential: UserCredentialDto) {
    this.credentialToDelete = credential;
    this.showDeleteDialog = true;
  }

  confirmDelete() {
    if (!this.credentialToDelete) return;

    this.credentialsService.deleteCredential(this.credentialToDelete.id).subscribe({
      next: () => {
        console.log('Credential deleted successfully');
        this.showDeleteDialog = false;
        this.credentialToDelete = undefined;
        this.loadCredentials();
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Credential deleted successfully'
        });
      },
      error: (err) => {
        console.error('Failed to delete credential:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to delete credential'
        });
      }
    });
  }

  cancelDelete() {
    this.showDeleteDialog = false;
    this.credentialToDelete = undefined;
  }

  // Helper methods for template
  getProviderName(providerType: ProviderType): string {
    const provider = this.providerOptions.find(p => p.value === providerType);
    return provider?.label || 'Unknown';
  }

  getProviderIcon(providerType: ProviderType): string {
    const provider = this.providerOptions.find(p => p.value === providerType);
    return provider?.icon || 'pi pi-code';
  }

  getCredentialTypeName(credentialType: CredentialType): string {
    const type = this.credentialTypeOptions.find(t => t.value === credentialType);
    return type?.label || 'Unknown';
  }

  get nameControl() {
    return this.editForm.get('name');
  }

  get usernameControl() {
    return this.editForm.get('username');
  }

  get secretControl() {
    return this.editForm.get('secret');
  }
}