<script lang="ts">
    let {
        upValues = [],
        downValues = [],
        width = 60,
        height = 30
    } = $props();

    let max = $derived(2 ** Math.floor(Math.log2(Math.max(...upValues, ...downValues, 128)) + 1))

    function getUpLinePath() {
        let path = "";
        let x = 0;
        let y = height / 2 * (1 - upValues[0] / max);
        path += `M ${x} ${y}`;
        for (let i = 1; i < upValues.length; i++) {
            x += width / (upValues.length - 1);
            y = height / 2 * (1 - upValues[i] / max);
            path += ` L ${x} ${y}`;
        }
        return path;
    }
    function getDownLinePath() {
        let path = "";
        let x = 0;
        let y = height / 2 * (1 + downValues[0] / max);
        path += `M ${x} ${y}`;
        for (let i = 1; i < downValues.length; i++) {
            x += width / (downValues.length - 1);
            y = height / 2 * (1 + downValues[i] / max);
            path += ` L ${x} ${y}`;
        }
        return path;
    }

    let upLinePath = $derived(getUpLinePath());
    let downLinePath = $derived(getDownLinePath());
</script>

<div class="graph-box">
    <svg class="graph-box-svg" viewBox={`0 0 ${width} ${height}`}>
        <rect class="graph-box-border" x={0} y={0} width={width} height={height} />
        <path class="graph-box-fill up" d={upLinePath + ` L ${width} ${height / 2} L 0 ${height / 2}`} />
        <path class="graph-box-fill down" d={downLinePath + ` L ${width} ${height / 2} L 0 ${height / 2}`} />
        <path class="graph-box-line" d={`M 0 ${height / 2} L ${width} ${height / 2}`} stroke-dasharray="3 3" opacity="0.25" />
        <path class="graph-box-line up" d={upLinePath} />
        <path class="graph-box-line down" d={downLinePath} />
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
    .down {
        --graph-line: #88f;
        --graph-border: #88f7;
        --graph-fill: #88f3;
    }
    .up {
        --graph-line: #f66;
        --graph-border: #f667;
        --graph-fill: #f663;
    }
</style>