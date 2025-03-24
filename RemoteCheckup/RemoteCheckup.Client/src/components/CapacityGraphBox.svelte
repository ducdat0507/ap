<script lang="ts">
    let {
        value = 0,
        width = 60,
        height = 20
    } = $props();
</script>

<div class="graph-box">
    <svg class="graph-box-svg" viewBox={`0 0 ${width} ${height}`}>
        <rect class="graph-box-border" x={0} y={0} width={width} height={height} />
        <rect class="graph-box-fill" x={0} y={0} width={width * value} height={height} />
        <path class="graph-box-line" d={` M ${width * value} 0 L ${width * value} ${height}`} />
    </svg>
</div>

<style>
    .graph-box {
        display: inline-block;
        --graph-line: #777;
        --graph-border: #7777;
        --graph-fill: #7773;
    }
    .graph-box-svg {
        display: block;
        width: 100%;
        background: #000c;
    }
    .graph-box-border {
        fill: none;
        stroke: var(--graph-border);
        stroke-width: 2;
        vector-effect: non-scaling-stroke;
    }
    .graph-box-line {
        fill: none;
        stroke: var(--graph-line);
        stroke-width: 1;
        stroke-linejoin: round;
        vector-effect: non-scaling-stroke;
    }
    .graph-box-fill {
        fill: var(--graph-fill); 
    }
</style>