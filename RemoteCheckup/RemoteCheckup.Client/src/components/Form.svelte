
<script lang="ts">
    let { 
        children,
        busy,
        ...formArgs
    } = $props();
</script>


<form class="form" class:busy={!!busy} {...formArgs}>
    {@render children()}
</form>

<style>
    .form {
        display: flex;
        flex-direction: column;
        gap: 5px;
    }
    .form.busy {
        opacity: 0.5;
        transition: opacity 1s;
    }
    .form :global(> div) {
        position: relative;
        display: flex;
        align-items: baseline;
    }
    .form :global(> hr) {
        width: 100%;
        border: none;
        border-bottom: 1px solid #777;
    }
    .form :global(:is(> div > label, .form .flex-space)) {
        flex: 1;
    }
    .form :global(:is(button, input)) {
        background: #0007;
        border: 1px solid #ccc;
        font-size: 1em;
        color: inherit;
        padding: 5px 10px;
        border-radius: 5px;
        transition: box-shadow .2s, border-color .2s;
    }
    .form :global(input) {
        width: 100%;
        box-shadow: inset 0 5px 10px black;
    }
    .form :global(:is(button, input):focus-visible) {
        outline: none;
        border-color: white;
        box-shadow: 0 0 0 2px #acf;
    }
    .form :global(input:focus-visible) {
        box-shadow: inset 0 5px 10px black, 0 0 0 2px #acf;
    }
    .form :global(input[type="checkbox"]) {
        position: relative;
        appearance: none;
        margin-inline: 0;
        width: calc(1.3em + 12px);
        aspect-ratio: 1;
        padding: 5px;
    }
    .form :global(input[type="checkbox"]::before) {
        content: " "; /* Non-breaking space */
        opacity: 0;
    }
    .form :global(input[type="checkbox"]:checked::after) {
        content: "";
        width: calc(0.4em);
        height: calc(0.8em);
        position: absolute;
        border-bottom: 4px solid white;
        border-right: 4px solid white;
        transform: rotate(45deg) translate(-3px, 3px);
    }
    .form :global(input[type="checkbox"] + label) {
        display: block;
        padding-inline: 5px 0;
    }

    .form :global(input:not([type="checkbox"]) + label) {
        position: absolute;
        left: 11px;
        top: 6px;
        opacity: 0;
        font-style: italic;
        user-select: none;
        -webkit-user-select: none;
        pointer-events: none;
    }
    .form :global(input:not([type="checkbox"]):placeholder-shown + label) {
        opacity: 0.4;
    }

    .form :global(> .error) {
        background: #4007;
        border: 1px solid #fcc;
        padding: 5px 10px;
        border-radius: 5px;
    }
</style>