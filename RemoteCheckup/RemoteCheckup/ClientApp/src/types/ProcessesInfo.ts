

export interface ProcessesInfo {
    processes: ProcessInfo[];
    services: ServiceInfo[];
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