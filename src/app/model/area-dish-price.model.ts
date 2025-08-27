export interface AreaDishPrice {
  id: string;
  areaId: string;
  dishId: string;
  customPrice: number;
  effectiveDate: string;
  isActive: boolean;
}

// Extended interface for display purposes with dish information
export interface AreaDishPriceDisplay extends AreaDishPrice {
  dishName?: string;
  basePrice?: number;
  kitchenId?: string;
  groupId?: string;
}
