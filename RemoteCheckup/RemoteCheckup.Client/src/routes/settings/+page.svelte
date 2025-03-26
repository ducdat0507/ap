<svelte:head>
    <title>Processes</title>
</svelte:head>

<script lang="ts">
  import Icon from '@iconify/svelte';
import { getContext, onDestroy, onMount, type Snippet } from 'svelte';

    let tab = $state("proc");

    function setTab(newTab: string)
    {
        tab = newTab;
    }

    let slots = getContext("layout-slots") as {
        subnav?: Snippet
    };
    Object.assign(slots, { subnav });
</script>

{#snippet tabButton(tab: string, icon: string, name: string)}
    <button class="item" onclick={() => setTab("tab")}>
        <div class="section">
            <div class="icon">
                <Icon {icon} width="24"/>
            </div>
            <div class="info">
                <h2>{name}</h2>
            </div>
        </div>
    </button>
{/snippet}

{#snippet subnav()}
    {@render tabButton("database", "fluent:database-24-regular", "Database")}
    {@render tabButton("account", "fluent:wrench-24-regular", "Account")}
{/snippet}

<style>
    .item .section {
        align-items: center;
    }
    .item .section :global(svg) {
        display: block;
        margin: 5px;
    }
</style>