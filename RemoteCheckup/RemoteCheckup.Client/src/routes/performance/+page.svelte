<svelte:head>
    <title>Performance</title>
</svelte:head>

<script lang="ts">
    import { HttpTransportType, HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
    import { getContext, onDestroy, onMount, type Snippet } from "svelte";
    import GraphBox from "../../components/GraphBox.svelte";
    import type { PerformanceInfo } from "../../types/PerformanceInfo";
    import GraphBoxList from "../../components/GraphBoxList.svelte";
    import { getNetType, formatPrefix } from "../../utils/utils"
    import Icon from "@iconify/svelte";
    import NetworkGraphBox from "../../components/NetworkGraphBox.svelte";
  import CapacityGraphBox from "../../components/CapacityGraphBox.svelte";
    
    let info: PerformanceInfo = $state({}) as PerformanceInfo;
    let connection: HubConnection | null = $state(null);

    let graphValues = $state({
        cpuTotal: [] as number[][],
        cpuCore: [] as number[][][],
        mem: undefined as number[] | undefined,
        swap: undefined as number[] | undefined,
        driveRead: [] as number[][],
        driveWrite: [] as number[][],
        netUp: [] as number[][],
        netDown: [] as number[][],
    })

    onMount(async () => {
        connection = new HubConnectionBuilder()
            .withUrl("/api/hubs/performance", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .build();
        connection.on("update", (inf : PerformanceInfo) => {
            info = inf;
            console.log(inf);

            for (let a = 0; a < info.processors.length; a++) {

                if (graphValues.cpuTotal[a] == undefined) {
                    graphValues.cpuTotal[a] = new Array(50).fill(0);
                }
                graphValues.cpuTotal[a].push(info.processors[a].totalUsage);
                graphValues.cpuTotal[a].shift();

                if (graphValues.cpuCore[a] == undefined) {
                    graphValues.cpuCore[a] = new Array();
                }
                for (let b = 0; b < info.processors[a].usage.length; b++) {
                    if (graphValues.cpuCore[a][b] == undefined) {
                        graphValues.cpuCore[a][b] = new Array(50).fill(0);
                    }
                    graphValues.cpuCore[a][b].push(info.processors[a].usage[b]);
                    graphValues.cpuCore[a][b].shift();
                }
            }

            if (graphValues.mem == undefined) {
                graphValues.mem = new Array(50).fill(0);
            }
            graphValues.mem.push(info.memory.usedPhys / info.memory.totalPhys);
            graphValues.mem.shift();

            if (graphValues.swap == undefined) {
                graphValues.swap = new Array(50).fill(0);
            }
            graphValues.swap.push(info.memory.usedSwap / info.memory.totalSwap);
            graphValues.swap.shift();
            
            for (let a = 0; a < info.drives.length; a++) {
                if (graphValues.driveRead[a] == undefined) {
                    graphValues.driveRead[a] = new Array(50).fill(0);
                }
                if (graphValues.driveWrite[a] == undefined) {
                    graphValues.driveWrite[a] = new Array(50).fill(0);
                }
                graphValues.driveRead[a].push(info.drives[a].readSpeed);
                graphValues.driveRead[a].shift();
                graphValues.driveWrite[a].push(info.drives[a].writeSpeed);
                graphValues.driveWrite[a].shift();
            }
            
            for (let a = 0; a < info.networks.length; a++) {
                if (graphValues.netUp[a] == undefined) {
                    graphValues.netUp[a] = new Array(50).fill(0);
                }
                if (graphValues.netDown[a] == undefined) {
                    graphValues.netDown[a] = new Array(50).fill(0);
                }
                graphValues.netUp[a].push(info.networks[a].uploadSpeed);
                graphValues.netUp[a].shift();
                graphValues.netDown[a].push(info.networks[a].downloadSpeed);
                graphValues.netDown[a].shift();
            }

            graphValues = graphValues;
        });
        await connection.start();
    })

    onDestroy(async () => {
        await connection?.stop();
    })

    let slots = getContext("layout-slots") as {
        subnav?: Snippet
    };
    Object.assign(slots, { subnav });
</script>

{#snippet subnav()}
    {#each graphValues.cpuTotal as cpu, i}
        <button class="item perf-item cpu">
            <div class="section">
                <GraphBox values={cpu} />
                <div class="info">
                    <h2><b>CPU</b></h2>
                    <p>{(info.processors[i].totalUsage * 100).toFixed(1)}%</p>
                </div>
            </div>
        </button>
    {/each}

    {#if info?.memory}
        <hr/>
        <button class="item perf-item mem">
            <div class="section">
                <GraphBox values={graphValues.mem} />
                <div class="info">
                    <h2><b>MEMORY</b></h2>
                    <p>
                        {(info?.memory.usedPhys / info?.memory.totalPhys * 100).toFixed(1)}%
                    </p>
                    <p>
                        {(info?.memory.usedPhys / 2 ** 30 || 0).toFixed(2)} / 
                        {(info?.memory.totalPhys / 2 ** 30 || 0).toFixed(2)} GiB
                    </p>
                </div>
            </div>
            {#if info.memory.totalSwap > 0}
                <div class="section">
                    <GraphBox values={graphValues.swap} />
                    <div class="info">
                        <h2><b>SWAP</b></h2>
                        <p>
                            {(info?.memory.usedSwap / info?.memory.totalSwap * 100).toFixed(1)}%
                        </p>
                        <p>
                            {(info?.memory.usedSwap / 2 ** 30 || 0).toFixed(2)} / 
                            {(info?.memory.totalSwap / 2 ** 30 || 0).toFixed(2)} GiB
                        </p>
                    </div>
                </div>
            {/if}
        </button>
    {/if}

    {#if info?.drives?.length}
        <hr/>
    {/if}
    {#each info?.drives as drv, i}
        <button class="item perf-item drv">
            <div class="section">
                <NetworkGraphBox upValues={graphValues.driveRead[i]} downValues={graphValues.driveWrite[i]} />
                <div class="info">
                    <h2><b>{drv.isHDD ? "HDD" : "SSD"}</b> {drv.name}</h2>
                    <p>
                        <Icon icon="fluent:arrow-up-12-regular" inline />
                        {formatPrefix(drv.readSpeed, true)}B/s
                    </p>
                    <p>
                        <Icon icon="fluent:arrow-down-12-regular" inline />
                        {formatPrefix(drv.writeSpeed, true)}B/s
                    </p>
                </div>
            </div>
            {#each drv.partitions as part, j}
                <div class="section">
                    <CapacityGraphBox value={part.usedBytes / part.totalBytes} />
                    <div class="info">
                        <h2><b>{part.name}</b></h2>
                        {formatPrefix(part.usedBytes || 0, true)}B / 
                        {formatPrefix(part.totalBytes || 0, true)}B 
                    </div>
                </div>
            {/each}
        </button>
    {/each}

    {#if info?.networks?.length}
        <hr/>
    {/if}
    {#each info?.networks as net, i}
        <button class="item perf-item net">
            <div class="section">
                <NetworkGraphBox upValues={graphValues.netUp[i]} downValues={graphValues.netDown[i]} />
                <div class="info">
                    <h2><b>{getNetType(net.type).toUpperCase()}</b> {net.name}</h2>
                    <p>
                        <Icon icon="fluent:arrow-up-12-regular" inline />
                        {formatPrefix(net.uploadSpeed * 8)}bps
                    </p>
                    <p>
                        <Icon icon="fluent:arrow-down-12-regular" inline />
                        {formatPrefix(net.downloadSpeed * 8)}bps
                    </p>
                </div>
            </div>
        </button>
    {/each}
{/snippet}

{#each graphValues.cpuCore as cpu, i}
    <article class="perf-object cpu">
        <h2><b>CPU {i}</b></h2>
        <GraphBoxList>
            {#each cpu as core, j}
                <GraphBox values={core} width={60} height={30} />
            {/each}
        </GraphBoxList>
    </article>
{/each}

<style>

    .perf-object {
        border: 1px solid var(--accent-color);
        padding: 10px;
    }
    .perf-object h2 {
        font-size: 1em;
        margin-block: 0 10px;
    }
    .perf-object :global(.graph-box) {
        --graph-line: var(--accent-color);
        --graph-border: unset;
        --graph-fill: unset;
    }

    .perf-object.cpu, .perf-item.cpu {
        --accent-color: #9fa;
        --graph-border: #9fa7;
        --graph-fill: #9fa3;
    }
    .perf-object.mem, .perf-item.mem {
        --accent-color: #7cf;
        --graph-border: #7cf7;
        --graph-fill: #7cf3;
    }
    .perf-object.drv, .perf-item.drv {
        --accent-color: #ee9;
        --graph-border: #ee97;
        --graph-fill: #ee93;
    }
    .perf-object.net, .perf-item.net {
        --accent-color: #e7f;
        --graph-border: #e7f7;
        --graph-fill: #e7f3;
    }
</style>