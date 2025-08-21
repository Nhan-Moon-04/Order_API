export interface TableSession {
  sessionId: string;
  tableId: string;
  openAt: string;
  closeAt?: string;
  openedBy?: string;
  closedBy?: string;
  status: number;     
}
