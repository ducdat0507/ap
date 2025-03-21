<script lang="ts">
    import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
    import { onMount } from "svelte";
    import GraphBox from "../../components/GraphBox.svelte";
    import type { PerformanceInfo } from "../../types/PerformanceInfo";
    import GraphBoxList from "../../components/GraphBoxList.svelte";
    
    let info: PerformanceInfo | null = null;

    let cpuGraphTotalValues: number[][] = [];
    let cpuGraphValues: number[][][] = [];

    let memGraphValues: number[] | undefined = undefined;
    let swapGraphValues: number[] | undefined = undefined;
    
    let netGraphUpValues: number[][] = [];
    let netGraphDownValues: number[][] = [];

    onMount(async () => {
        let connection = new HubConnectionBuilder()
            .withUrl("/api/hubs/performance", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .build();
        connection.on("update", (inf : PerformanceInfo) => {
            info = inf;

            for (let a = 0; a < info.processors.length; a++) {

                if (cpuGraphTotalValues[a] == undefined) {
                    cpuGraphTotalValues[a] = new Array(50).fill(0);
                }
                cpuGraphTotalValues[a].push(info.processors[a].totalUsage);
                cpuGraphTotalValues[a].shift();

                if (cpuGraphValues[a] == undefined) {
                    cpuGraphValues[a] = new Array();
                }
                for (let b = 0; b < info.processors[a].usage.length; b++) {
                    if (cpuGraphValues[a][b] == undefined) {
                        cpuGraphValues[a][b] = new Array(50).fill(0);
                    }
                    cpuGraphValues[a][b].push(info.processors[a].usage[b]);
                    cpuGraphValues[a][b].shift();
                }
            }
            cpuGraphValues = cpuGraphValues;
            cpuGraphTotalValues = cpuGraphTotalValues;


            if (memGraphValues == undefined) {
                memGraphValues= new Array(50).fill(0);
            }
            memGraphValues.push(info.memory.usedPhys / info.memory.totalPhys);
            memGraphValues.shift();
            memGraphValues = memGraphValues;

            if (swapGraphValues == undefined) {
                swapGraphValues = new Array(50).fill(0);
            }
            swapGraphValues.push(info.memory.usedSwap / info.memory.totalSwap);
            swapGraphValues.shift();
            swapGraphValues = swapGraphValues;
            
            for (let a = 0; a < info.networks.length; a++) {
                if (netGraphUpValues[a] == undefined) {
                    netGraphUpValues[a] = new Array(50).fill(0);
                }
                if (netGraphDownValues[a] == undefined) {
                    netGraphDownValues[a] = new Array(50).fill(0);
                }
                netGraphUpValues.push(info.networks[a].uploadSpeed);
                netGraphUpValues.shift();
                netGraphDownValues.push(info.networks[a].downloadSpeed);
                netGraphDownValues.shift();
            }
        });
        await connection.start();
    })
</script>

<div class="perf-container">
    <aside>
        {#each cpuGraphTotalValues as cpu, i}
            <button class="perf-item cpu">
                <GraphBox values={cpu} />
                <div class="info">
                    <h2><b>CPU {i}</b></h2>
                    <p>{(info.processors[i].totalUsage * 100).toFixed(1)}%</p>
                </div>
            </button>
        {/each}

        {#if info?.memory}
            <hr/>
            <button class="perf-item mem">
                <GraphBox values={memGraphValues} />
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
            </button>
            {#if info.memory.totalSwap > 0}
                <button class="perf-item mem">
                    <GraphBox values={swapGraphValues} />
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
                </button>
            {/if}
        {/if}
        
        {#if info?.networks?.length}
            <hr/>
        {/if}
        {#each info?.networks as net, i}
            <button class="perf-item mem">
                <GraphBox values={netGraphUpValues[i]} />
                <div class="info">
                    <h2><b>NET</b></h2>
                    <p>
                        UP {(net.uploadSpeed).toFixed(0)}
                    </p>
                    <p>
                        DOWN {(net.downloadSpeed).toFixed(0)}
                    </p>
                </div>
            </button>
        {/each}
    </aside>
    <main>
        {#each cpuGraphValues as cpu, i}
            <article class="perf-object cpu">
                <h2><b>CPU {i}</b></h2>
                <GraphBoxList>
                    {#each cpu as core, j}
                        <GraphBox values={core} width={60} height={30} />
                    {/each}
                </GraphBoxList>
            </article>
        {/each}
    </main>
</div>

<style>
    .perf-container {
        display: flex;
        flex-direction: row;
        position: fixed;
        inset: 0;
    }
    .perf-container aside {
        width: 300px;
        flex: 0 0 300px;
        padding: 5px;
        display: flex;
        flex-direction: column;
        align-items: stretch;
    }
    .perf-container main {
        padding: 10px;
        flex: 1;
    }
    .perf-container aside hr {
        border: none;
        border-bottom: 1px solid #7777;
        margin: 5px;
    }

    .perf-item {
        display: flex;
        flex-direction: row;
        font-size: small;
        gap: 5px;
        background: transparent;
        border: none;
        padding: 5px;
        text-align: left;
        transition: background 0.1s, box-shadow 0.1s;
    }
    .perf-item:hover {
        box-shadow: inset 0 0 0 1px var(--graph-border);
    }
    .perf-item:active {
        box-shadow: inset 0 0 0 1px var(--graph-border);
        background: var(--graph-fill);
        transition: none;
    }
    .perf-item h2, p {
        font-size: 1em;
        margin: 0;
    }
    .perf-item :global(.graph-box) {
        width: 100px;
        --graph-line: var(--accent-color);
        --graph-border: unset;
        --graph-fill: unset;
    }

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
        --accent-color: #592;
        --graph-border: #5927;
        --graph-fill: #5923;
    }
    .perf-object.mem, .perf-item.mem {
        --accent-color: #27c;
        --graph-border: #27c7;
        --graph-fill: #27c3;
    }
</style>