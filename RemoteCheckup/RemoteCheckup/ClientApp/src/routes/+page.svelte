<script lang="ts">
    import { HubConnectionBuilder } from "@microsoft/signalr";
    import { onMount } from "svelte";

    onMount(() => {
        let connection = new HubConnectionBuilder().withUrl("api/hub/performance").build();
        connection.on("update", () => {
            console.log("update called");
        })
    })
</script>

<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://svelte.dev/docs/kit">svelte.dev/docs/kit</a> to read the documentation</p>
