export interface TableFilter {
    areaId: string;
    isActive?: boolean;
}

export interface TableQueryParams {
    areaId: string;
    isActive?: string; // Query params are strings
}
