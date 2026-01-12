import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GitCredentialsComponent } from './components/git-credentials/git-credentials.component';

// PrimeNG v20 Imports
import { TabsModule } from 'primeng/tabs';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [
    CommonModule,
    GitCredentialsComponent,
    TabsModule
  ],
  templateUrl: './settings-page.component.html',
  styleUrl: './settings-page.component.scss'
})
export class SettingsPageComponent {
  activeTab: number = 0;
}