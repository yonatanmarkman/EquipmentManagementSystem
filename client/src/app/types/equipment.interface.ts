export enum EquipmentStatus {
  Active = 'Active',
  InMaintenance = 'InMaintenance',
  OutOfService = 'OutOfService',
  Retired = 'Retired'
}

// Base interface with common fields for create/update
export interface BaseEquipment {
  equipmentName: string;
  serialNumber: string;
  categoryId: number;
  locationId: number;
  purchaseDate: Date;
  status: EquipmentStatus;
}

// For API responses - includes ID and related entity names
export interface Equipment extends BaseEquipment {
  id: number;
  categoryName: string;
  locationName: string;
}

// For creating equipment - just uses base fields
export type CreateEquipment = BaseEquipment;

// For updating equipment - just uses base fields
export type UpdateEquipment = BaseEquipment;

export interface EquipmentSearch {
  equipmentName?: string;
  purchaseDate?: Date;
  categoryId?: number;
  pageNumber: number;
  pageSize: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}