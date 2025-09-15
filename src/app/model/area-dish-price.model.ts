export interface AreaDishPrice {
  id: string;
  areaId: string;
  dishId: string;
  customPrice: number;
  effectiveDate: string;
  isActive: boolean;
  sortOrder: number;
  createdAt?: string;
  areaName?: string;
  dishName?: string;
}

// Extended interface for display purposes with dish information
export interface AreaDishPriceDisplay extends AreaDishPrice {
  basePrice?: number;
  kitchenId?: string;
  groupId?: string;
}

// Search request interface
export interface AreaDishPriceSearchRequest {
  searchString: string;
  pageIndex: number;
  pageSize: number;
  isActive: number;
  areaId?: string;
  dishId?: string;
  areaName?: string;
  dishName?: string;
  kitchenName?: string;
  groupName?: string;
  description?: string;
}

// Pagination response interface
export interface PagedResponse<T> {
  items: T[];
  totalRecords: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
}

// Add dishes to area request interface
export interface AddDishesToAreaRequest {
  areaId: string;
  dishIds: string[];
  customPrice: number;
}

// Delete area dish price request interface
export interface DeleteAreaDishPriceRequest {
  id: string;
}

// Delete area dish price response interface
export interface DeleteAreaDishPriceResponse {
  message: string;
}

// Get available dishes request interface
export interface GetAvailableDishesRequest {
  areaId: string;
  searchString?: string;
}

// Available dishes response interface - backend returns array directly
export type AvailableDishesResponse = AvailableDish[];

// Available dish interface for modal
export interface AvailableDish {
  dishId: string;
  dishName: string;
  basePrice: number;
  description?: string;
  kitchenName?: string;
  groupName?: string;
  isActive?: boolean;
  createdAt?: string;
  id?: string;
}
