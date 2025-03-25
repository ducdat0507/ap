
<script lang="ts">
  import Icon from "@iconify/svelte";

    let { 
        children,
        busy,
        actionName,
        ...buttonProps
    } = $props();
</script>

<button disabled={!!busy} {...buttonProps}>
    {#if busy == actionName} 
        <Icon icon="svg-spinners:ring-resize" inline />
    {:else}
        {@render children()}
    {/if}
</button>