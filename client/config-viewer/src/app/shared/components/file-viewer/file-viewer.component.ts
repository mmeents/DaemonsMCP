import { Component, Input, signal, effect, ElementRef, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

declare const monaco: any;

@Component({
  selector: 'app-file-viewer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="file-viewer">
      @if (loading()) {
        <div class="loading-state">
          <div class="spinner"></div>
          <span>Loading file...</span>
        </div>
      } @else if (error()) {
        <div class="error-state">
          <span class="error-icon">⚠️</span>
          <p>{{ error() }}</p>
        </div>
      } @else if (!fileName()) {
        <div class="empty-state">
          <span class="empty-icon">📄</span>
          <p>Select a file to view its contents</p>
        </div>
      } @else {
        <div class="file-header">
          <span class="file-name">{{ fileName() }}</span>
          <span class="file-path">{{ filePath() }}</span>
        </div>
        <div #editorContainer class="editor-container"></div>
      }
    </div>
  `,
  styles: [`
    .file-viewer {
      display: flex;
      flex-direction: column;
      height: 100%;
      background: #1e1e1e;
      border: 1px solid #e0e0e0;
      border-radius: 4px;
      overflow: hidden;
    }

    .file-header {
      display: flex;
      flex-direction: column;
      padding: 12px 16px;
      background: #2d2d30;
      border-bottom: 1px solid #3e3e42;
      color: #cccccc;

      .file-name {
        font-size: 14px;
        font-weight: 600;
        margin-bottom: 4px;
      }

      .file-path {
        font-size: 12px;
        color: #858585;
      }
    }

    .editor-container {
      flex: 1;
      width: 100%;
      height: 100%;
    }

    .loading-state, .error-state, .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      color: #cccccc;
      gap: 12px;

      .spinner {
        width: 32px;
        height: 32px;
        border: 3px solid #3e3e42;
        border-top-color: #0e639c;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
      }

      .empty-icon, .error-icon {
        font-size: 48px;
        opacity: 0.5;
      }

      p {
        margin: 0;
        font-size: 14px;
      }
    }

    .error-state {
      color: #f48771;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class FileViewerComponent implements AfterViewInit, OnDestroy {
  @ViewChild('editorContainer', { static: false }) editorContainer?: ElementRef;
  
  @Input() fileName = signal<string>('');
  @Input() filePath = signal<string>('');
  @Input() fileContent = signal<string>('');
  @Input() loading = signal<boolean>(false);
  @Input() error = signal<string | null>(null);

  private editor: any = null;
  private monacoLoaded = false;

  constructor() {
    // Watch for content changes - this will trigger when a file is selected
    effect(() => {
      const content = this.fileContent();
      const name = this.fileName();
      
      console.log('Effect triggered - content length:', content?.length, 'fileName:', name);
      
      if (content && name) {
        // Content arrived - ensure Monaco is loaded and editor is ready
        this.ensureEditorReady().then(() => {
          console.log('Editor ready, updating content');
          this.updateEditorContent(content, name);
        });
      }
    });
  }

  ngAfterViewInit() {
    console.log('ngAfterViewInit called');
    // Start loading Monaco in the background
    this.loadMonaco();
  }

  ngOnDestroy() {
    if (this.editor) {
      this.editor.dispose();
    }
  }

  private async ensureEditorReady(): Promise<void> {
    console.log('ensureEditorReady called');
    
    // Wait for Monaco to load
    if (!this.monacoLoaded) {
      console.log('Waiting for Monaco to load...');
      await this.waitForMonaco();
    }
    
    // Wait for editor container to be available
    if (!this.editorContainer) {
      console.log('Waiting for editor container...');
      await this.waitForEditorContainer();
    }
    
    // Create editor if it doesn't exist
    if (!this.editor) {
      console.log('Creating editor...');
      this.createEditor();
    }
  }

  private waitForMonaco(): Promise<void> {
    return new Promise((resolve) => {
      const checkInterval = setInterval(() => {
        if (this.monacoLoaded) {
          clearInterval(checkInterval);
          console.log('Monaco is ready!');
          resolve();
        }
      }, 100);
    });
  }

  private waitForEditorContainer(): Promise<void> {
    return new Promise((resolve) => {
      const checkInterval = setInterval(() => {
        if (this.editorContainer) {
          clearInterval(checkInterval);
          console.log('Editor container is ready!');
          resolve();
        }
      }, 50);
      
      // Timeout after 5 seconds
      setTimeout(() => {
        clearInterval(checkInterval);
        console.error('Timeout waiting for editor container');
        resolve();
      }, 5000);
    });
  }

  private loadMonaco() {
    if (this.monacoLoaded) {
      console.log('Monaco already loaded');
      return;
    }

    console.log('Loading Monaco from CDN...');
    const loaderScript = document.createElement('script');
    loaderScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs/loader.min.js';
    loaderScript.onload = () => {
      console.log('Monaco loader script loaded');
      (window as any).require.config({ 
        paths: { 
          vs: 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs' 
        } 
      });
      (window as any).require(['vs/editor/editor.main'], () => {
        this.monacoLoaded = true;
        console.log('Monaco editor main loaded successfully!');
      });
    };
    loaderScript.onerror = (error) => {
      console.error('Failed to load Monaco:', error);
    };
    document.body.appendChild(loaderScript);
  }

  private createEditor() {
    if (!this.editorContainer) {
      console.error('Cannot create editor - container not available');
      return;
    }
    
    if (this.editor) {
      console.log('Editor already exists');
      return;
    }

    console.log('Creating Monaco editor instance...');
    try {
      this.editor = monaco.editor.create(this.editorContainer.nativeElement, {
        value: '',
        language: 'plaintext',
        theme: 'vs-dark',
        readOnly: true,
        automaticLayout: true,
        minimap: { enabled: true },
        scrollBeyondLastLine: false,
        fontSize: 13,
        lineNumbers: 'on',
        renderWhitespace: 'selection',
        scrollbar: {
          verticalScrollbarSize: 10,
          horizontalScrollbarSize: 10
        }
      });
      
      console.log('Monaco editor created successfully!');
    } catch (error) {
      console.error('Error creating Monaco editor:', error);
    }
  }

  private updateEditorContent(content: string, fileName: string) {
    console.log('Updating editor content, length:', content.length);

    // Check if editor exists and is still connected to a valid DOM element
    if (!this.editor || !this.editor.getDomNode()?.isConnected) {
        console.log('Editor invalid or disconnected, recreating...');
        if (this.editor) {
        try {
            this.editor.dispose();
        } catch (e) {
            console.warn('Error disposing old editor:', e);
        }
        this.editor = null;
        }
        this.createEditor();
    }

    if (!this.editor) {
        console.error('Cannot update content - editor not initialized');
        return;
    }

    const language = this.getLanguageFromFileName(fileName);
    console.log('Detected language:', language);

    // Dispose old model if it exists
    const oldModel = this.editor.getModel();
    if (oldModel) {
        oldModel.dispose();
    }

    const model = monaco.editor.createModel(content, language);
    this.editor.setModel(model);
    console.log('Content updated successfully');
  }

  private getLanguageFromFileName(fileName: string): string {
    const ext = fileName.split('.').pop()?.toLowerCase();
    const languageMap: { [key: string]: string } = {
      'ts': 'typescript',
      'js': 'javascript',
      'json': 'json',
      'html': 'html',
      'css': 'css',
      'scss': 'scss',
      'md': 'markdown',
      'cs': 'csharp',
      'py': 'python',
      'sql': 'sql',
      'xml': 'xml',
      'yaml': 'yaml',
      'yml': 'yaml',
      'sh': 'shell',
      'bat': 'bat',
      'ps1': 'powershell'
    };
    return languageMap[ext || ''] || 'plaintext';
  }
} 
    