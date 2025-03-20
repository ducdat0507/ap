<div class="graph-box-list">
    <slot />
</div>

<style>
    .graph-box-list {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
        gap: 5px;
    }
</style>