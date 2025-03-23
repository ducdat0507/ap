export function getNetType(typeNo: number) {
    switch (typeNo) {
        case 1:
            return "unknown"
        case 6: case 26: case 62: case 69:
            return "ethernet"
        case 71:
            return "wi-fi"
        default:
            return "other"
    }
}

export function getServiceStatus(statusNo: number) {
    switch (statusNo) {
        case 12:
            return "inactive"
        case 13:
            return "running"
        case 11:
            return "completed"
        case 666666:
            return "faulted"
        default:
            return "unknown"
    }
}

export function formatPrefix(num: number, binary: boolean = false) {
    const base = binary 
        ? 1024 
        : 1000;
    const prefixes = binary 
        ? ["", "Ki", "Mi", "Gi", "Ti", "Pi", "Ei", "Zi", "Yi"]
        : ["", "k", "M", "G", "T", "P", "E", "Z", "Y"];
    let index = Math.max(0, Math.floor(Math.log(num) / Math.log(base)));
    if (num / base ** index >= 1000) index++;
    return index <= 0
        ? num.toFixed(0) + " "
        : (num / base ** index).toPrecision(3) + " " + prefixes[index];
}