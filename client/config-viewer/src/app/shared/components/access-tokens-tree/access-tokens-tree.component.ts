import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { AccessTokenDto } from '../../models/api.models';
import { ContextMenuModule } from 'primeng/contextmenu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-access-tokens-tree',
  standalone: true,
  imports: [CommonModule, TableModule, ContextMenuModule],
  styleUrl: './access-tokens-tree.component.scss',
  templateUrl: './access-tokens-tree.component.html'  
})
export class AccessTokensTreeComponent implements OnInit, OnChanges {
  @Input() tokens: AccessTokenDto[] = [];
  @Input() loading = false;
  @Input() selectedTokenId?: number;
  @Output() tokenSelected = new EventEmitter<AccessTokenDto>();
  @Output() tokenRevoke = new EventEmitter<AccessTokenDto>();
  @Output() tokenCopy = new EventEmitter<AccessTokenDto>();

  selectedToken?: AccessTokenDto;
  contextMenuItems: MenuItem[] = [];

  ngOnInit() {
    this.updateSelection();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['tokens'] || changes['selectedTokenId']) {
      this.updateSelection();
    }
  }

  private updateSelection() {
    if (this.selectedTokenId) {
      this.selectedToken = this.tokens.find(t => t.id === this.selectedTokenId);
    }
  }

  onRowSelect(event: any) {
    this.tokenSelected.emit(event.data);
  }

  onContextMenu(event: any, token: AccessTokenDto) {
    this.selectedToken = token;
    
    this.contextMenuItems = [
      {
        label: 'Copy Token',
        icon: 'pi pi-copy',
        command: () => this.onCopy()
      },
      {
        separator: true
      },
      {
        label: 'Revoke',
        icon: 'pi pi-ban',
        command: () => this.onRevoke(),
        disabled: token.isExpired
      }
    ];
  }

  private onCopy() {
    if (this.selectedToken) {
      this.tokenCopy.emit(this.selectedToken);
    }
  }

  private onRevoke() {
    if (this.selectedToken) {
      this.tokenRevoke.emit(this.selectedToken);
    }
  }

  getTokenShort(token: string): string {
    if (token.length >= 8) {
      return `${token.substring(0, 4)}...${token.substring(token.length - 4)}`;
    }
    return token;
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
}