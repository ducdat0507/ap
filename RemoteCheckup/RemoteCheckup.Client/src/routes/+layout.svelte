<script lang="ts">
    import { setContext } from "svelte";
    import "../app.css";
    import type { Snippet } from "svelte";
    import Icon from "@iconify/svelte";
    import { page } from '$app/state';

    let { children } = $props();

    const slots: {
        subnav?: Snippet
    } = $state({});

    function isPathExact(path: string): boolean {
        return page.url.pathname == path;
    }
    function isPathIn(path: string): boolean {
        return page.url.pathname.startsWith(path);
    }
    setContext('layout-slots', slots);
</script>

<div class="container">
    <aside class={{
        "has-subnav": !!slots.subnav
    }}>
        <nav class="nav grainy-bg">
            <div>
                <span></span>
                <a class="item" class:active={isPathExact("/")} href="/">
                    <Icon icon="fluent:home-24-regular" />
                    <span>Overview</span>
                </a>
                <a class="item" class:active={isPathIn("/performance")} href="/performance">
                    <Icon icon="fluent:top-speed-24-regular" />
                    <span>Performance</span>
                </a>
                <a class="item" class:active={isPathIn("/processes")} href="/processes">
                    <Icon icon="fluent:apps-24-regular" />
                    <span>Processes</span>
                </a>
                <a class="item" class:active={isPathIn("/databases")} href="/databases">
                    <Icon icon="fluent:database-24-regular" />
                    <span>Databases</span>
                </a>
                <hr />
                <a class="item" class:active={isPathIn("/settings")} href="/settings">
                    <Icon icon="fluent:wrench-20-regular" />
                    <span>Settings</span>
                </a>
                <span></span>
                <button class="item danger">
                    <Icon icon="fluent:power-24-regular" />
                    <span>Log Out</span>
                </button>
            </div>
        </nav>
        {#if slots.subnav}
            <nav class="subnav grainy-bg">
                <div>
                    {@render slots.subnav?.()}
                </div>
            </nav>
        {/if}
    </aside>
    <main>
        {@render children()}
    </main>
</div>

<style>
    .container {
        display: flex;
        flex-direction: row;
        position: fixed;
        inset: 0;
    }
    .container aside {
        box-shadow: 0 0 0 1px black, 0 0 10px 5px black;
        display: flex;
        flex-direction: row;
        align-items: stretch;
        overflow-y: auto;
        z-index: 10;
        transition: all 0.3s;
    }
    .container aside > nav {
        --bg-color: #2226;
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: stretch;
        justify-content: stretch;
        transition: all 0.3s;
    }
    .container aside > nav > div {
        position: relative;
        flex: 1;
        max-height: 100%;
        display: flex;
        flex-direction: column;
        align-items: stretch;
        overflow: hidden auto;
    }
    .container aside .subnav {
        width: 320px;
        flex: 0 0 320px;
        --accent-color: #ccc;
        --graph-border: #ccc7;
        --graph-fill: #ccc3;
    }
    .container aside .subnav > div {
        padding: 15px;
    }
    .container aside > nav::before {
        content: "";
        position: absolute;
        inset: 0 0 0 auto;
        width: 1px;
        background: linear-gradient(#0000, #7777, #fffa, #7777, #0000);
    }

    .nav {
        z-index: 10;
    }
    .nav > div {
        padding: 10px;
        gap: 1px;
    }
    .nav > div > span {
        display: block;
        flex: 1;
    }
    .nav .item {
        position: relative;
        display: block;
        background-color: #0000;
        border: none;
        color: white;
        text-decoration: none;
        font-size: 24px;
        width: 44px;
        border-radius: 10px;
        padding: 10px;
        transition: width 0.3s, background 0.1s, box-shadow 0.1s;
    }
    .nav .item:hover {
        box-shadow: inset 0 0 0 1px #ccc;
        background-color: #ccc3;
    }
    .nav .item:active {
        box-shadow: inset 0 0 0 1px #ccc;
        background-color: #000c;
        transition: width 0.3s;
    }
    .nav .item.danger:hover {
        box-shadow: inset 0 0 0 1px #f99;
        background-color: #f993;
    }
    .nav .item.danger:active {
        box-shadow: inset 0 0 0 1px #f99;
        background-color: #000c;
    }
    .nav .item.active {
        box-shadow: inset 0 0 0 1px #acf;
        background-color: #acf3;
        transition: width 0.3s;
        overflow: hidden;
    }
    .nav .item :global(svg) {
        display: block;
    }
    .nav .item span {
        position: absolute;
        top: calc(22px - 0.4rem);
        left: 44px;
        white-space: nowrap;
        font-size: 0.8rem;
        line-height: 1;
        opacity: 0;
        transition: opacity 0.3s;
    }
    .nav .item.item.active span {
        opacity: 1;
    }
    .nav:hover .item span {
        opacity: 1;
    }

    .nav:hover {
        --bg-color: #3336;
        box-shadow: 0 0 0 1px black, 0 0 10px 5px black;
    }
    .nav:hover .item {
        width: 180px;
    }
    .nav:hover + .subnav {
        margin-left: -146px;
        filter: brightness(0.4);
    }
    .container aside:not(.has-subnav):hover {
        margin-right: -136px;
    }
    .container aside.has-subnav:has(.nav:hover):hover {
        box-shadow: 0 0 0 1px black, 0 0 4px 2px black;
        margin-right: 10px;
    }

    .subnav :global(.item) {
        display: flex;
        flex-direction: column;
        font-size: small;
        gap: 5px;
        background: transparent;
        border: none;
        padding: 5px;
        text-align: left;
        transition: background 0.1s, box-shadow 0.1s;
    }
    .subnav :global(.item:hover) {
        box-shadow: inset 0 0 0 1px var(--graph-border);
        background: var(--graph-fill);
    }
    .subnav :global(.item:active) {
        box-shadow: inset 0 0 0 1px var(--graph-border);
        background: #000a;
        transition: none;
    }
    .subnav :global(.item .section) {
        display: flex;
        flex-direction: row;
        gap: 5px;
    }
    .subnav :global(.item .section .info) {
        min-width: 0;
    }
    .subnav :global(:is(.item h2, .item h3, .item p)) {
        font-size: 1em;
        font-weight: normal;
        margin: 0;
        max-width: 100%;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }
    .subnav :global(.item .graph-box) {
        width: 100px;
        flex: 0 0 100px;
        --graph-line: var(--accent-color);
        --graph-border: unset;
        --graph-fill: unset;
    }

    .container main {
        padding: 20px;
        flex: 1;
        overflow-y: auto;
    }
    .container aside :global(hr) {
        border: none;
        border-bottom: 1px solid #7777;
        margin: 5px;
    }
</style>