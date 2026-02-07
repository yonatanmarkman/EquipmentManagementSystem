import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Equipment,
  CreateEquipment,
  UpdateEquipment,
  PagedResult,
  EquipmentSearch,
  Category,
  Location
} from '../types';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Equipment endpoints
  getAllEquipment(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResult<Equipment>>
  {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Equipment>>(`${this.apiUrl}/equipment`, { params });
  }

  getEquipmentById(id: number): Observable<Equipment>
  {
    return this.http.get<Equipment>(`${this.apiUrl}/equipment/${id}`);
  }

  searchEquipment(searchDto: EquipmentSearch): Observable<PagedResult<Equipment>>
  {
    return this.http.post<PagedResult<Equipment>>(`${this.apiUrl}/equipment/search`, searchDto);
  }

  createEquipment(equipment: CreateEquipment): Observable<Equipment>
  {
    return this.http.post<Equipment>(`${this.apiUrl}/equipment`, equipment);
  }

  updateEquipment(id: number, equipment: UpdateEquipment): Observable<Equipment>
  {
    return this.http.put<Equipment>(`${this.apiUrl}/equipment/${id}`, equipment);
  }

  deleteEquipment(id: number): Observable<void>
  {
    return this.http.delete<void>(`${this.apiUrl}/equipment/${id}`);
  }

  getEquipmentByCategory(categoryId: number, pageNumber: number = 1, pageSize: number = 10): Observable<PagedResult<Equipment>> 
  {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Equipment>>(`${this.apiUrl}/equipment/category/${categoryId}`, { params });
  }

  // Category endpoints
  getAllCategories(): Observable<Category[]>
  {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }

  // Location endpoints
  getAllLocations(): Observable<Location[]>
  {
    return this.http.get<Location[]>(`${this.apiUrl}/locations`);
  }
}