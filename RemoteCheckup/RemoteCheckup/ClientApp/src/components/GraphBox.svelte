<script lang="ts">
    let {
        values = [],
        width = 60,
        height = 30
    } = $props();

    function getLinePath() {
        let path = "";
        let x = 0;
        let y = height * (1 - values[0]);
        path += `M ${x} ${y}`;
        for (let i = 1; i < values.length; i++) {
            x += width / (values.length - 1);
            y = height * (1 - values[i]);
            path += ` L ${x} ${y}`;
        }
        return path;
    }
    let linePath = $derived(getLinePath());
</script>

<div class="graph-box">
    <svg class="graph-box-svg" viewBox={`0 0 ${width} ${height}`}>
        <rect class="graph-box-border" x={0} y={0} width={width} height={height} />
        <path class="graph-box-fill" d={linePath + ` L ${width} ${height} L 0 ${height}`} />
        <path class="graph-box-line" d={linePath} />
    </svg>
</div>

<style>
    .graph-box {
        display: inline-block;
        --graph-line: #592;
        --graph-border: #5927;
        --graph-fill: #5923;
    }
    .graph-box-svg {
        display: block;
        width: 100%;
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