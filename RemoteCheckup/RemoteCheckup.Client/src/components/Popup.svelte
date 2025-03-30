<script lang="ts">
    import { getContext, type Snippet } from "svelte";
    import PopupBody from "./PopupBody.svelte";
    import Icon from "@iconify/svelte";

    let { children, onshow = null, onhide = null } = $props();

    const popups = getContext("popups") as { popups: Snippet[] };

    export function show() {
        if (!popups.popups.includes(modal)) {
            popups.popups.push(modal);
            onshow?.();
        }
    }
    export function hide() {
        const index = popups.popups.indexOf(modal);
        if (index >= 0) {
            popups.popups.splice(index, 1);
            onhide?.();
        }
    }
</script>

{#snippet modal()}
    <PopupBody>
        <button class="popup-hide-btn" onclick={hide}>
            <Icon icon="akar-icons:cross" width="20" inline />
        </button>
        {@render children()}
    </PopupBody>
{/snippet}

<style>
    .popup-hide-btn {
        position: absolute;
        right: 20px;
        top: 15px;
        padding: 5px;
    }
    .popup-hide-btn :global(svg) {
        display: block;
    }
</style>
