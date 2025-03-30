<svelte:head>
    <title>Settings</title>
</svelte:head>

<script lang="ts">
  import Icon from '@iconify/svelte';
import { getContext, onDestroy, onMount, type Snippet } from 'svelte';
    import AccountSettings from '../../components/settings/AccountSettings.svelte';
  import ProbeSettings from '../../components/settings/ProbeSettings.svelte';

    let tab = $state("general");

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
    <button class="item" onclick={() => setTab(tab)}>
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
    {@render tabButton("general", "fluent:settings-24-regular", "General")}
    {@render tabButton("probes", "fluent:eye-24-regular", "Probes")}
    {@render tabButton("account", "fluent:person-24-regular", "Account")}
{/snippet}

{#if tab == "general"}
    <h2>General</h2>
{:else if tab == "probes"}
    <ProbeSettings />
{:else if tab == "account"}
    <AccountSettings />
{/if}

<style>
    .item .section {
        align-items: center;
    }
    .item .section :global(svg) {
        display: block;
        margin: 5px;
    }
</style>