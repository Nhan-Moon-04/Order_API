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
