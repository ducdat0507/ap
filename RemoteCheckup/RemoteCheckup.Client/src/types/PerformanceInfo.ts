

export interface PerformanceInfo {
    processors: CPUInfo[];
    memory: MemoryInfo;
    drives: DriveInfo[];
    networks: NetworkInfo[];
}

export interface CPUInfo {
    name: string;
    totalUsage: number;
    usage: number[];
}

export interface MemoryInfo {
    totalPhys: number;
    usedPhys: number;
    committedPhys: number;
    totalSwap: number;
    usedSwap: number;
}

export interface DriveInfo {
    name: string;
    isHDD?: boolean;
    readSpeed: number;
    writeSpeed: number;
    partitions: PartitionInfo[];
}

export interface PartitionInfo {
    name: string;
    totalBytes: number;
    usedBytes: number;
}

export interface NetworkInfo {
    name: string;
    type: number;
    uploadSpeed: number;
    downloadSpeed: number;
}