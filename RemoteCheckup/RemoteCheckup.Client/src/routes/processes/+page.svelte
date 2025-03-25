<svelte:head>
    <title>Processes</title>
</svelte:head>

<script lang="ts">
    import { HttpTransportType, HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
    import { getContext, onDestroy, onMount, type Snippet } from "svelte";
    import GraphBox from "../../components/GraphBox.svelte";
    import type { ProcessesInfo, ProcessInfo } from "../../types/ProcessesInfo";
    import GraphBoxList from "../../components/GraphBoxList.svelte";
    import { getNetType, formatPrefix, getServiceStatus, getPortStatus } from "../../utils/utils"
    import Icon from "@iconify/svelte";
    import NetworkGraphBox from "../../components/NetworkGraphBox.svelte";
    import CapacityGraphBox from "../../components/CapacityGraphBox.svelte";
    
    let info: ProcessesInfo = $state({}) as ProcessesInfo;
    let connection: HubConnection | null = $state(null);

    let tab = $state("proc");
    let sortCriteria: string | null = $state(null);
    let sortDirection: 1 | -1 = $state(-1);

    onMount(async () => {
        connection = new HubConnectionBuilder()
            .withUrl("/api/hubs/processes", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets,
            })
            .build();
        connection.on("update", (inf : ProcessesInfo) => {
            info = inf;
            console.log(inf);
        });
        await connection.start();
    });

    onDestroy(async () => {
        await connection?.stop();
    })

    function sortProcs<T>(procs: T[] | undefined): T[] {
        if (!procs) return [];
        if (!sortCriteria) return procs;
        procs = [...procs];
        procs.sort((x, y) => {
            const xc = x[sortCriteria as keyof T];
            const yc = y[sortCriteria as keyof T];
            if (typeof xc == "string")
                return xc.localeCompare(yc as string) * sortDirection;
            if (typeof xc == "number")
                return (xc - (yc as number)) * sortDirection;
            return 0;
        });
        return procs;
    }

    function setTab(newTab: string)
    {
        tab = newTab;
        sortCriteria = null;
        sortDirection = -1;
    }

    function setSort(column: string)
    {
        if (sortCriteria == column) {
            sortDirection *= -1;
        } else {
            sortCriteria = column;
            sortDirection = -1;
        }
    }

    let slots = getContext("layout-slots") as {
        subnav?: Snippet
    };
    Object.assign(slots, { subnav });
</script>

{#snippet columnHead(column: string, name: string)}
    <th>
        <button onclick={() => setSort(column)}>
            {name}
            {#if sortCriteria == column}
                <Icon icon={`fluent:arrow-${sortDirection == 1 ? "up" : "down"}-12-regular`} inline/>
            {/if}
        </button>
    </th>
{/snippet}

{#snippet subnav()}
    <button class="item" onclick={() => setTab("proc")}>
        <div class="section">
            <div class="info">
                <h2><b>PROCESSES</b></h2>
                <p>{info?.processes?.length ?? 0}</p>
            </div>
        </div>
    </button>
    <button class="item" onclick={() => setTab("serv")}>
        <div class="section">
            <div class="info">
                <h2><b>SERVICES</b></h2>
                <p>{info?.services?.length ?? 0}</p>
            </div>
        </div>
    </button>
    <button class="item" onclick={() => setTab("port")}>
        <div class="section">
            <div class="info">
                <h2><b>ACTIVE PORTS</b></h2>
                <p>{info?.ports?.length ?? 0}</p>
            </div>
        </div>
    </button>
{/snippet}

<div class="proc-div">
    <div class="proc-filters">
        asdads
    </div>
    <div class="proc-container">
        <table class="proc-table">
            {#if tab == "proc"}
                <thead class="grainy-bg">
                    <tr>
                        {@render columnHead("pid", "PID")}
                        {@render columnHead("name", "NAME")}
                        {@render columnHead("cpuUsage", "CPU")}
                        {@render columnHead("memoryUsage", "MEM")}
                        <th class="fill"></th>
                    </tr>
                </thead>
                <tbody>
                    {#each sortProcs(info.processes) as proc}
                        <tr>
                            <td class="right">{proc.pid}</td>
                            <td>{proc.name}</td>
                            <td class="right">{(proc.cpuUsage * 100).toFixed(2)} %</td>
                            <td class="right">{formatPrefix(proc.memoryUsage, true)}B</td>
                            <td></td>
                        </tr>
                    {/each}
                </tbody>
            {:else if tab == "serv"}
                <thead class="grainy-bg">
                    <tr>
                        {@render columnHead("name", "NAME")}
                        {@render columnHead("status", "STATUS")}
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {#each sortProcs(info.services) as serv}
                        <tr>
                            <td>{serv.name}</td>
                            <td class="right">{getServiceStatus(serv.status)}</td>
                            <td></td>
                        </tr>
                    {/each}
                </tbody>
            {:else if tab == "port"}
                <thead class="grainy-bg">
                    <tr>
                        {@render columnHead("isTCP", "PROTO")}
                        {@render columnHead("status", "STATUS")}
                        {@render columnHead("port", "PORT")}
                        {@render columnHead("pid", "PID")}
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {#each sortProcs(info.ports) as port}
                        <tr>
                            <td>{port.isTCP ? "tcp" : "udp"}</td>
                            <td class="right">{getPortStatus(port.status)}</td>
                            <td class="right">{port.port}</td>
                            <td class="right">{port.pid ?? "---"}</td>
                            <td></td>
                        </tr>
                    {/each}
                </tbody>
            {/if}
        </table>
    </div>
</div>

<style>
    .proc-div {
        display: flex;
        flex-direction: column;
        margin: -20px;
        width: calc(100% + 40px);
        height: calc(100% + 40px);
    }
    .proc-filters {
        display: flex;
        padding: 10px 20px;
    }
    .proc-container {
        position: relative;
        width: 100%;
        height: 100%;
        overflow: auto;
    }
    .proc-table {
        position: absolute;
        inset: 0;
        width: 100%;
        max-height: 100%;
        border-collapse: collapse; 
        border: none;
        gap: 0;
        font-size: smaller;
    }
    .proc-table :is(th, td) {
        padding: 2px 5px;
        max-width: 250px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }
    .proc-table :is(th, td):first-child {
        padding-inline-start: 20px;
    }
    .proc-table :is(th, td):last-child {
        width: 100%;
        padding-inline-end: 20px;
    }
    .proc-table .right {
        text-align: right;
    }
    .proc-table thead {
        --bg-color: #222a;
        z-index: 5;
        position: sticky;
        top: 0px;
        padding-top: 10px;
        user-select: none;
        -webkit-user-select: none;
    }
    .proc-table thead button {
        width: calc(100% + 10px);
        height: calc(100% + 4px);
        margin: -2px -5px;
        border: none;
        background: #0000;
    }
    .proc-table thead button:hover {
        background: #fff7;
    }
    .proc-table thead button:active {
        background: #000a;
    }
    .proc-table :is(thead, tbody) {
        width: 100%;
    }
</style>