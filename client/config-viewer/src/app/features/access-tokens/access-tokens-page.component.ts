import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccessTokensTreeComponent } from '../../shared/components/access-tokens-tree/access-tokens-tree.component';
import { AccessTokensService } from '../../core/services/access-token.service';
import { 
  AccessTokenDto, 
  AccessTokenSearchParams, 
  CreateAccessTokenCommand 
} from '../../shared/models/api.models';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { PaginatorModule } from 'primeng/paginator';
import { MessageService, MenuItem } from 'primeng/api';
import { SplitButtonModule } from 'primeng/splitbutton';

@Component({
  selector: 'app-access-tokens-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AccessTokensTreeComponent,
    DialogModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    PaginatorModule,
    SplitButtonModule
  ],
  templateUrl: './access-tokens-page.component.html',
  styleUrl: './access-tokens-page.component.scss',
  providers: [MessageService]
})
export class AccessTokensPageComponent implements OnInit {
  tokens: AccessTokenDto[] = [];
  selectedToken?: AccessTokenDto;
  loading = false;
  copyMenuItems: MenuItem[] = [];

  searchParams: AccessTokenSearchParams = {
    issuedTo: '',
    includeExpired: false,
    includeUsed: false,
    pageNo: 1,
    pageSize: 20
  };

  totalCount = 0;
  totalPages = 0;

  isGenerating = false;
  isSaving = false;
  generatedToken: AccessTokenDto | null = null;
  generateForm: CreateAccessTokenCommand = {
    issuedTo: '',
    expiresInMinutes: 60
  };

  showRevokeDialog = false;
  tokenToRevoke?: AccessTokenDto;
  showDetailDialog = false;
  tokenToShow?: AccessTokenDto;

  constructor(
    private tokensService: AccessTokensService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.loadTokens();
   this.initializeCopyMenu();
  }

  private initializeCopyMenu() {
    this.copyMenuItems = [
      {
        label: 'Copy README URL',
        icon: 'pi pi-link',
        command: () => {
          if (this.tokenToShow) {
            this.copyReadmeUrl(this.tokenToShow.token);
          }
        }
      }
    ];
  }

  loadTokens() {
    this.loading = true;
    this.tokensService.searchTokens(this.searchParams).subscribe({
      next: (result) => {
        this.tokens = result.data;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.loading = false;
        console.log('Loaded tokens:', result);
      },
      error: (err) => {
        console.error('Failed to load tokens:', err);
        this.loading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load tokens'
        });
      }
    });
  }

  onSearch() {
    this.searchParams.pageNo = 1;
    this.loadTokens();
  }

  onPageChange(event: any) {
    this.searchParams.pageNo = (event.first / event.rows) + 1;
    this.loadTokens();
  }

  onTokenSelected(token: AccessTokenDto) {
    this.selectedToken = token;
    this.showTokenDetail(token);
    console.log('Selected token:', token);
  }

  onGenerateToken() {
    this.generateForm = {
      issuedTo: '',
      expiresInMinutes: 60
    };
    this.generatedToken = null;
    this.isGenerating = true;
  }

  onSaveToken() {
    if (!this.generateForm.issuedTo.trim()) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Validation',
        detail: 'Issued To is required'
      });
      return;
    }

    if (this.generateForm.expiresInMinutes <= 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Validation',
        detail: 'Expires In must be greater than 0'
      });
      return;
    }

    this.isSaving = true;
      this.tokensService.createToken(this.generateForm).subscribe({
        next: (token) => {
        console.log('Token generated:', token);
        this.isSaving = false;
        this.isGenerating = false; // CLOSE THE DIALOG
        this.generatedToken = token; // Store for detail view
        this.selectedToken = token;  // Select it in the table
        this.loadTokens();
        
        // Show the token in a detail dialog instead
        this.showTokenDetail(token);
      },
      error: (err) => {
        console.error('Failed to generate token:', err);
        this.isSaving = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to generate token'
        });
      }
    });
  }

  showTokenDetail(token: AccessTokenDto) {
    this.tokenToShow = token;
    this.showDetailDialog = true;
  }

  onCancelGenerate() {
    this.isGenerating = false;
    this.generatedToken = null;
  }

  onCopyToken(token: AccessTokenDto) {
    this.copyToClipboard(token.token);
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

  onRevokeToken(token: AccessTokenDto) {
    this.tokenToRevoke = token;
    this.showRevokeDialog = true;
  }

  confirmRevoke() {
    if (!this.tokenToRevoke) return;

    this.tokensService.revokeToken(this.tokenToRevoke.id).subscribe({
      next: () => {
        console.log('Token revoked successfully');
        this.showRevokeDialog = false;
        this.loadTokens();
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Token revoked successfully'
        });
      },
      error: (err) => {
        console.error('Failed to revoke token:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to revoke token'
        });
      }
    });
  }

  cancelRevoke() {
    this.showRevokeDialog = false;
    this.tokenToRevoke = undefined;
  }

  getStatus(token: AccessTokenDto): string {
    if (token.isExpired) return 'Expired';
    if (token.used) return 'Used';
    return 'Valid';
  }

  getStatusClass(token: AccessTokenDto): string {
    if (token.isExpired) return 'status-expired';
    if (token.used) return 'status-used';
    return 'status-valid';
  }

  getReadmeUrl(token: string): string {
    return `https://daemonsmcp.app/api/readme?token=${token}`;
  }

  copyReadmeUrl(token: string) {
    const url = this.getReadmeUrl(token);
    navigator.clipboard.writeText(url).then(() => {
      this.messageService.add({
        severity: 'success',
        summary: 'Copied',
        detail: 'README URL copied to clipboard'
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
}