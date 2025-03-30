<script lang="ts">
    let { children } = $props();
</script>

<div class="popup-bg">
    <section class="popup grainy-bg">
        {@render children()}
    </section>
</div>

<style>
    .popup-bg {
        position: fixed;
        inset: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 20px;
    }
    .popup {
        position: relative;
        overflow: hidden auto;
        padding: 15px 20px;
        --bg-color: #2227;
        border-radius: 10px;
        box-shadow: 0 0 0 1px black, 0 10px 25px 5px black;
        min-width: 400px;
        max-width: 720px;
    }
    .popup > :global(*) {
        margin-block: 5px;
    }
</style>

