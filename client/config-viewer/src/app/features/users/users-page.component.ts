import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { InvitationService } from '../../core/services/invitation.service';
import {
  InvitationTokenDto,
  InvitationTokenSearchParams,
  CreateInvitationCommand, UserDto
} from '../../shared/models/api.models';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { PaginatorModule } from 'primeng/paginator';
import { MessageService, MenuItem } from 'primeng/api';
import { SplitButtonModule } from 'primeng/splitbutton';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    PaginatorModule,
    SplitButtonModule,
    TableModule,
    TagModule,
    ToastModule,
    InputNumberModule,
    ReactiveFormsModule
  ],
  templateUrl: './users-page.component.html',
  styleUrl: './users-page.component.scss',
  providers: [MessageService]
})
export class UsersPageComponent implements OnInit {
  // Users Section
  users: UserDto[] = [];
  selectedUser?: UserDto;
  loadingUsers = false;
  userSearchEmail = '';

  // Invitations Section
  invitations: InvitationTokenDto[] = [];
  selectedInvitation?: InvitationTokenDto;
  loadingInvitations = false;
  copyMenuItems: MenuItem[] = [];

  invitationSearchParams: InvitationTokenSearchParams = {
    invitedEmail: '',
    includeExpired: false,
    includeUsed: false,
    pageNo: 1,
    pageSize: 20
  };

  totalInvitationCount = 0;
  totalInvitationPages = 0;

  // Create Invitation Dialog
  showCreateDialog = false;
  isCreating = false;
  createInvitationForm: FormGroup;
  

  // Invitation Detail Dialog
  showDetailDialog = false;
  invitationToShow?: InvitationTokenDto;

  // Revoke Dialog
  showRevokeDialog = false;
  invitationToRevoke?: InvitationTokenDto;  

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private invitationService: InvitationService,
    private messageService: MessageService
  ) {
    this.createInvitationForm = this.fb.group({
      invitedEmail: ['', [Validators.email]],
      expiresInHours: [168, [Validators.required, Validators.min(1), Validators.max(8760)]],
    });
  }

  ngOnInit() {
    this.loadUsers();
    this.loadInvitations();
    this.initializeCopyMenu();
  }

  private initializeCopyMenu() {
    this.copyMenuItems = [
      {
        label: 'Copy Invitation URL',
        icon: 'pi pi-link',
        command: () => {
          if (this.invitationToShow) {
            this.copyInvitationUrl(this.invitationToShow.token);
          }
        }
      }
    ];
  }

  // ============ USERS SECTION ============
  loadUsers() {
    this.loadingUsers = true;
    this.userService.getAllUsers().subscribe({
      next: (result) => {
        this.users = result.data;
        this.loadingUsers = false;
        console.log('Loaded users:', result);
      },
      error: (err) => {
        console.error('Failed to load users:', err);
        this.loadingUsers = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load users'
        });
      }
    });
  }

  onUserSearch() {
    // Filter users locally or call API with search param
    if (this.userSearchEmail.trim()) {
      // You could add a search endpoint or filter locally
      console.log('Searching for:', this.userSearchEmail);
    } else {
      this.loadUsers();
    }
  }

  onUserSelected(event: any) {
    const user = event.data as UserDto;
    if (user) {
      this.selectedUser = user;
      console.log('Selected user:', user);
    }
  }

  getUserStatusClass(user: UserDto): string {
    return user.isActive ? 'status-active' : 'status-inactive';
  }

  getUserStatus(user: UserDto): string {
    return user.isActive ? 'Active' : 'Inactive';
  }

  // ============ INVITATIONS SECTION ============
  loadInvitations() {
    this.loadingInvitations = true;
    this.invitationService.searchInvitations(this.invitationSearchParams).subscribe({
      next: (result) => {
        this.invitations = result.data;
        this.totalInvitationCount = result.totalCount;
        this.totalInvitationPages = result.totalPages;
        this.loadingInvitations = false;
        console.log('Loaded invitations:', result);
      },
      error: (err) => {
        console.error('Failed to load invitations:', err);
        this.loadingInvitations = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load invitations'
        });
      }
    });
  }

  onInvitationSearch() {
    this.invitationSearchParams.pageNo = 1;
    this.loadInvitations();
  }

  onInvitationPageChange(event: any) {
    this.invitationSearchParams.pageNo = (event.first / event.rows) + 1;
    this.loadInvitations();
  }

  onInvitationSelected(invitation: InvitationTokenDto) {
    this.selectedInvitation = invitation;
    this.showInvitationDetail(invitation);
    console.log('Selected invitation:', invitation);
  }

  // ============ CREATE INVITATION ============
  onCreateInvitation() {
    this.createInvitationForm.reset({
      invitedEmail: '',
      expiresInHours: 168
    });
    this.showCreateDialog = true;
  }

  onSaveInvitation() {
     this.createInvitationForm.markAllAsTouched();

    if (!this.createInvitationForm.valid) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Validation Error',
        detail: 'Please fix the errors in the form'
      });
      return;
    }

    const formValue = this.createInvitationForm.value;
    const command: CreateInvitationCommand = {
      createdByUserId: 1, // TODO: Get from current user context
      invitedEmail: formValue.invitedEmail || undefined,  // Convert empty string to undefined
      expiresInHours: formValue.expiresInHours
    };

    this.isCreating = true;
    this.invitationService.createInvitation(command).subscribe({
      next: (invitation) => {
        console.log('Invitation created:', invitation);
        this.isCreating = false;
        this.showCreateDialog = false;
        this.loadInvitations();
        
        this.showInvitationDetail(invitation);
        
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Invitation created successfully'
        });
      },
      error: (err) => {
        console.error('Failed to create invitation:', err);
        this.isCreating = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to create invitation'
        });
      }
    });
  }

  onCancelCreate() {
    this.showCreateDialog = false;
  }

  // ============ INVITATION DETAIL ============
  showInvitationDetail(invitation: InvitationTokenDto) {
    this.invitationToShow = invitation;
    this.showDetailDialog = true;
  }

  onCopyInvitationToken(invitation: InvitationTokenDto) {
    this.copyToClipboard(invitation.token);
  }

  copyToClipboard(text: string) {
    navigator.clipboard.writeText(text).then(() => {
      this.messageService.add({
        severity: 'success',
        summary: 'Copied',
        detail: 'Token copied to clipboard'
      });
    }).catch(err => {
      console.error('Failed to copy:', err);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to copy to clipboard'
      });
    });
  }

  getInvitationUrl(token: string): string {
    return `https://daemonsmcp.app/register?token=${token}`;
  }

  copyInvitationUrl(token: string) {
    const url = this.getInvitationUrl(token);
    navigator.clipboard.writeText(url).then(() => {
      this.messageService.add({
        severity: 'success',
        summary: 'Copied',
        detail: 'Invitation URL copied to clipboard'
      });
    }).catch(err => {
      console.error('Failed to copy:', err);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to copy to clipboard'
      });
    });
  }

  // Helper methods for template
  get invitedEmailControl() {
    return this.createInvitationForm.get('invitedEmail');
  }

  get expiresInHoursControl() {
    return this.createInvitationForm.get('expiresInHours');
  }

  // ============ REVOKE INVITATION ============
  onRevokeInvitation(invitation: InvitationTokenDto) {
    this.invitationToRevoke = invitation;
    this.showRevokeDialog = true;
  }

  confirmRevoke() {
    if (!this.invitationToRevoke) return;

    this.invitationService.revokeInvitation(this.invitationToRevoke.id).subscribe({
      next: () => {
        console.log('Invitation revoked successfully');
        this.showRevokeDialog = false;
        this.loadInvitations();
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Invitation revoked successfully'
        });
      },
      error: (err) => {
        console.error('Failed to revoke invitation:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to revoke invitation'
        });
      }
    });
  }

  cancelRevoke() {
    this.showRevokeDialog = false;
    this.invitationToRevoke = undefined;
  }

  // ============ INVITATION STATUS ============
  getInvitationStatus(invitation: InvitationTokenDto): string {
    const now = new Date();
    const expiresAt = new Date(invitation.expiresAt);
    
    if (invitation.isUsed) return 'Used';
    if (now >= expiresAt) return 'Expired';
    return 'Valid';
  }

  getInvitationStatusClass(invitation: InvitationTokenDto): string {
    const now = new Date();
    const expiresAt = new Date(invitation.expiresAt);
    
    if (invitation.isUsed) return 'status-used';
    if (now >= expiresAt) return 'status-expired';
    return 'status-valid';
  }
}