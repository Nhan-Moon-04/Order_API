// Request models for order detail operations
export interface ChangeQuantityRequest {
  orderId: string;
  dishId: string;
  newQuantity: number;
}

export interface RemoveFoodRequest {
  orderId: string;
  dishId: string;
}

// Response models
export interface ChangeQuantityResponse {
  orderDetailId: string;
  orderId: string;
  dishId: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  dishName: string;
  kitchenName: string;
}

export interface RemoveFoodResponse {
  message: string;
  success: boolean;
}
