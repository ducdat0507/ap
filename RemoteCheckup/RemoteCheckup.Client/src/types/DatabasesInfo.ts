export interface DatabasesInfo {
    servers: DatabaseServerInfo[];
}

export interface DatabaseServerInfo {
    name: string;
    type: string;
    databases: DatabaseInfo[];
}

export interface DatabaseInfo {
    name: string;
    tables: TableInfo[];
}

export interface TableInfo {
    name: string;
}