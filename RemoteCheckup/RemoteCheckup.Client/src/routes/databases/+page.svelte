<svelte:head>
    <title>Databases</title>
</svelte:head>

<script lang="ts">
    import { HttpTransportType, HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
    import { getContext, onDestroy, onMount, type Snippet } from "svelte";
    import type { DatabasesInfo } from "../../types/DatabasesInfo";
    
    let info: DatabasesInfo = $state({}) as DatabasesInfo;
    let connection: HubConnection | null = $state(null)

    onMount(async () => {
        connection = new HubConnectionBuilder()
            .withUrl("/api/hubs/databases", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .build();
        connection.on("update", (inf : DatabasesInfo) => {
            info = inf;
            console.log(inf);
        });
        await connection.start();
    });

    onDestroy(async () => {
        await connection?.stop();
    })

    let slots = getContext("layout-slots") as {
        subnav?: Snippet
    };
    Object.assign(slots, { subnav });
</script>

{#snippet subnav()}
    {#each info?.servers as server}
        <hr />
        <h2 class="db-title">{(server.type ?? "").toUpperCase()}</h2>
        {#each server.databases as db}
            <button class="item">
                <div class="section">
                    <div class="info">
                        <h3><b>{db.name}</b></h3>
                    </div>
                </div>
            </button>
        {/each}
    {/each}
{/snippet}

<style>
    hr:first-child {
        display: none;
    }
    .db-title {
        padding: 5px;
        margin: 0;
        font-size: .8em;
        color: #cccc;
    }
</style>