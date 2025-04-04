

export interface ProcessesInfo {
    processes: ProcessInfo[];
    services: ServiceInfo[];
    ports: ActivePortInfo[];
}

export interface ProcessInfo {
    pid: number;
    name: string;
    cpuUsage: number;
    memoryUsage: number;
}
export interface ServiceInfo {
    name: string;
    status: number;
}
export interface ActivePortInfo {
    isTCP: boolean;
    status: number;
    port: number;
    pid?: number;
}