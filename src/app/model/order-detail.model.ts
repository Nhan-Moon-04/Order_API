export interface OrderDetail {
  id: string;
  orderDetailId: string;
  orderId: string;
  dishId: string;
  quantity: number;
  unitPrice: number;
  tableId?: string;
  areaId?: string;
  priceSource: number; // enum trong .NET
}
