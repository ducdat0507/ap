<script lang="ts">
    import { onMount } from "svelte";
    import { getDatabaseType } from "../../utils/utils";
    import EditDatabaseTargetModal from "../modals/EditDatabaseTargetModal.svelte";

    import "../../common/css/settings.css";

    let dbTargets: any[] = [];

    let editDbModal: EditDatabaseTargetModal;

    onMount(() => {
        updateDbTargets();
    });

    function updateDbTargets() {
        fetch("api/db/targets").then(x => x.json()).then(data => {
            dbTargets = data.data;
        })
    }
</script>

<h2>Probe Settings</h2>
<section class="setting-section">
    <h3>Database Targets</h3>
    <div class="db-target-list">
        {#each dbTargets as target}
            <section class="db-target">
                <h4><b>{getDatabaseType(target.type).toUpperCase()}</b> {target.name}</h4>
                <button onclick={() => editDbModal.show(target)}>edit</button> 
            </section>
        {/each}
        <button onclick={() => editDbModal.show(null)}>add new</button>
    </div>
</section>
<EditDatabaseTargetModal bind:this={editDbModal} onedit={updateDbTargets} />

<style>
    .db-target-list {
        display: flex;
        flex-direction: column;
        gap: 5px;
    }
    .db-target {
        padding: 5px;
        border: 1px solid #aaa;
        border-radius: 5px;
        display: flex;
        flex-direction: row-reverse;
        justify-content: start;
        align-items: baseline;
        gap: 1ch;
    }
    .db-target h4 {
        margin: 0;
        font-weight: normal;
    }
</style>
