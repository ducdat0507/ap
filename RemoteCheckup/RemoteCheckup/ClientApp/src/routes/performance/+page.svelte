<script lang="ts">
    import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
    import { onMount } from "svelte";
    import GraphBox from "../../components/GraphBox.svelte";
    import type { PerformanceInfo } from "../../types/PerformanceInfo";
    import GraphBoxList from "../../components/GraphBoxList.svelte";
    
    let info: PerformanceInfo | null = null;

    let cpuGraphValues: number[][][] = [];

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

            console.log(cpuGraphValues);
            cpuGraphValues = cpuGraphValues;
        });
        await connection.start();
    })
</script>

<h1>Welcome to SvelteKit</h1>
{#each cpuGraphValues as cpu, i}
    <article class="cpu">
        <h2>CPU {i}</h2>
        <GraphBoxList>
            {#each cpu as core, j}
                <GraphBox values={core} width={60} height={30} />
            {/each}
        </GraphBoxList>
    </article>
{/each}