import { Equipment, PagedResult, EquipmentSearch, Category, Location, EquipmentStatus } from '../types';
import { Injectable, signal, computed, inject } from '@angular/core';
import { catchError, finalize, of, tap } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class EquipmentStateService {
  private apiService = inject(ApiService);

  // Signals for state management
  equipment = signal<Equipment[]>([]);
  categories = signal<Category[]>([]);
  locations = signal<Location[]>([]);
  
  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  
  // Pagination state
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);
  totalCount = signal<number>(0);
  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));
  
  // Filter state
  selectedCategoryId = signal<number | null>(null);
  selectedStatus = signal<EquipmentStatus | null>(null);
  searchName = signal<string>('');
  searchDate = signal<Date | null>(null);

  // Load all equipment with pagination
  loadEquipment() {
    this.loading.set(true);
    this.error.set(null);

    this.apiService.getAllEquipment(this.currentPage(), this.pageSize())
      .pipe(
        tap((result: PagedResult<Equipment>) => {
          this.equipment.set(result.items);
          this.totalCount.set(result.totalCount);
        }),
        catchError((err) => {
          this.error.set('Failed to load equipment');
          console.error('Error loading equipment:', err);
          return of({ 
            items: [], 
            totalCount: 0, 
            pageNumber: 1, 
            pageSize: 10, 
            totalPages: 0, 
            hasPreviousPage: false, 
            hasNextPage: false 
            });
        }),
        finalize(() => this.loading.set(false))
      )
      .subscribe();
  }

  // Search/filter equipment
  searchEquipment() {
    this.loading.set(true);
    this.error.set(null);

    const searchDto: EquipmentSearch = {
      equipmentName: this.searchName() || undefined,
      purchaseDate: this.searchDate() || undefined,
      categoryId: this.selectedCategoryId() || undefined,
      pageNumber: this.currentPage(),
      pageSize: this.pageSize()
    };

    this.apiService.searchEquipment(searchDto)
      .pipe(
        tap((result: PagedResult<Equipment>) => {
          this.equipment.set(result.items);
          this.totalCount.set(result.totalCount);
        }),
        catchError((err) => {
          this.error.set('Failed to search equipment');
          console.error('Error searching equipment:', err);
          return of({ 
            items: [], 
            totalCount: 0, 
            pageNumber: 1, 
            pageSize: 10, 
            totalPages: 0, 
            hasPreviousPage: false, 
            hasNextPage: false 
            });
        }),
        finalize(() => this.loading.set(false))
      )
      .subscribe();
  }

  // Load categories
  loadCategories() {
    this.apiService.getAllCategories()
      .pipe(
        tap((categories: Category[]) => {
          this.categories.set(categories);
        }),
        catchError((err) => {
          console.error('Error loading categories:', err);
          return of([]);
        })
      )
      .subscribe();
  }

  // Load locations
  loadLocations() {
    this.apiService.getAllLocations()
      .pipe(
        tap((locations: Location[]) => {
          this.locations.set(locations);
        }),
        catchError((err) => {
          console.error('Error loading locations:', err);
          return of([]);
        })
      )
      .subscribe();
  }

  // Pagination methods
  goToPage(page: number) {
    this.currentPage.set(page);
    this.hasActiveFilters() ? this.searchEquipment() : this.loadEquipment();
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.goToPage(this.currentPage() + 1);
    }
  }

  previousPage() {
    if (this.currentPage() > 1) {
      this.goToPage(this.currentPage() - 1);
    }
  }

  // Filter methods
  setCategory(categoryId: number | null) {
    this.selectedCategoryId.set(categoryId);
    this.currentPage.set(1);
    this.searchEquipment();
  }

  setStatus(status: EquipmentStatus | null) {
    this.selectedStatus.set(status);
    this.currentPage.set(1);
    // Note: Status filter needs to be added to backend search if required
    this.searchEquipment();
  }

  setSearchName(name: string) {
    this.searchName.set(name);
    this.currentPage.set(1);
    this.searchEquipment();
  }

  setSearchDate(date: Date | null) {
    this.searchDate.set(date);
    this.currentPage.set(1);
    this.searchEquipment();
  }

  clearFilters() {
    this.selectedCategoryId.set(null);
    this.selectedStatus.set(null);
    this.searchName.set('');
    this.searchDate.set(null);
    this.currentPage.set(1);
    this.loadEquipment();
  }

  hasActiveFilters(): boolean {
    return !!(
      this.selectedCategoryId() ||
      this.selectedStatus() ||
      this.searchName() ||
      this.searchDate()
    );
  }

  // Delete equipment
  deleteEquipment(id: number) {
    return this.apiService.deleteEquipment(id).pipe(
      tap(() => {
        // Reload current page after deletion
        this.hasActiveFilters() ? this.searchEquipment() : this.loadEquipment();
      }),
      catchError((err) => {
        this.error.set('Failed to delete equipment');
        console.error('Error deleting equipment:', err);
        throw err;
      })
    );
  }
}