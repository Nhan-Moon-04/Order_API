export interface Dish {
  dishId: string;
  dishName: string;
  basePrice: number;
  kitchenId: string;
  groupId: string;
  isActive: boolean;
  createdAt: string;
  kitchenName?: string;
  groupName?: string;
  description?: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalRecords: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
}
