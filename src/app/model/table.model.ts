export interface Table {
  tableId: string;
  tableCode: string;
  tableName: string;
  capacity: number;
  isActive: boolean;
  status: number;     
  openAt?: string;
  closeAt?: string;
  areaId: string;
}
