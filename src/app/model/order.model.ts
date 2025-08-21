export interface Order {
  orderId: string;
  createdAt: string;
  closedAt?: string;
  isPaid: boolean;
  primaryAreaId: string;
  orderStatus: number; 
}