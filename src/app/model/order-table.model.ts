export interface OrderTable {
  id: string;
  orderId: string;
  tableId: string;
  isPrimary: boolean;
  fromTime?: string;
  toTime?: string;
}
