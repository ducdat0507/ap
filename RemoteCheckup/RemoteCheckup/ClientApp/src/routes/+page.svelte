<script lang="ts">
    import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
    import { onMount } from "svelte";
    
    let info: {};

    onMount(async () => {
        let connection = new HubConnectionBuilder()
            .withUrl("/api/hubs/performance", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .build();
        connection.on("update", (inf) => {
            info = inf;
        });
        await connection.start();
    })
</script>

<h1>Welcome to SvelteKit</h1>
<p>{JSON.stringify(info)}</p>
