import { Component, Input, signal, effect, ElementRef, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

declare const monaco: any;

@Component({
  selector: 'app-file-viewer',
  standalone: true,
  imports: [CommonModule],
  styleUrl: './file-viewer.component.scss',
  templateUrl: 'file-viewer.component.html' 
})
export class FileViewerComponent implements AfterViewInit, OnDestroy {
  @ViewChild('editorContainer', { static: false }) editorContainer?: ElementRef;
  @Input() fileSystemId = signal<number | null>(null);
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
    