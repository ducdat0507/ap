<script lang="ts">
    import { getContext, type Snippet } from "svelte";
    import PopupBody from "./PopupBody.svelte";

    let { children } = $props();

    const popups = getContext("popups") as { popups: Snippet[] };

    export function show() {
        if (!popups.popups.includes(modal))
            popups.popups.push(modal);
    }
    export function hide() {
        const index = popups.popups.indexOf(modal);
        if (index >= 0) popups.popups.splice(index, 1);
    }
</script>

{#snippet modal()}
    <PopupBody>
        <button class="popup-hide-btn" onclick={hide}>x</button>
        {@render children()}
    </PopupBody>
{/snippet}
