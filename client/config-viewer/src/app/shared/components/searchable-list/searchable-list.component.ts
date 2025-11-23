import { Component, Input, Output, EventEmitter, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-searchable-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './searchable-list.component.html',
  styleUrl: './searchable-list.component.scss'
})
export class SearchableListComponent<T> {
  // Inputs with defaults
  @Input() items: WritableSignal<T[]> = signal([]);
  @Input() loading: WritableSignal<boolean> = signal(false);
  @Input() searchPlaceholder = 'Search files...';
  @Input() emptyMessage = 'Type to search for files';
  @Input() noResultsMessage = 'No files found';
  
  // Column configuration
  @Input() columns: ColumnConfig<T>[] = [];
  
  // Outputs
  @Output() itemSelected = new EventEmitter<T>();
  @Output() searchChanged = new EventEmitter<string>();

  // Internal state
  protected searchTerm = signal('');
  protected selectedItem = signal<T | null>(null);
  
  // Search debouncer
  private searchSubject = new Subject<string>();

  constructor() {
    // Set up debounced search
    this.searchSubject.pipe(
      debounceTime(1000),
      distinctUntilChanged(),
      takeUntilDestroyed()
    ).subscribe(term => {
      this.searchChanged.emit(term);
    });
  }

  protected onSearchInput(value: string): void {
    this.searchTerm.set(value);
    this.searchSubject.next(value);
  }

  protected clearSearch(): void {
    this.searchTerm.set('');
    this.searchSubject.next('');
  }

  protected selectItem(item: T): void {
    this.selectedItem.set(item);
    this.itemSelected.emit(item);
  }

  protected isSelected(item: T): boolean {
    return this.selectedItem() === item;
  }

  protected trackByIndex(index: number): number {
    return index;
  }

  protected getCellValue(item: T, column: ColumnConfig<T>): string {
    return column.getValue(item);
  }
}

// Column configuration interface
export interface ColumnConfig<T> {
  header: string;
  width?: string;
  getValue: (item: T) => string;
  align?: 'left' | 'center' | 'right';
}