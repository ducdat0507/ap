
<script lang="ts">
    import Button from "../Button.svelte";
    import Form from "../Form.svelte";
    import Popup from "../Popup.svelte";

    let popup: Popup;

    let {
        onedit = null,
    } = $props();
    
    let data = $state({
        id: 0,
        type: 0,
        name: "",
        connString: "",
        connSecret: "",
    });

    let busy = $state("");
    let error = $state("");

    function onhide() {
        data = {
            id: 0,
            type: 0,
            name: "",
            connString: "",
            connSecret: "",
        }
        busy = error = "";
    }

    function submit(e: SubmitEvent) {
        e.preventDefault();
        busy = "submit";
        fetch("/api/db/targets", {
            method: data.id ? "PUT" : "POST",
            body: JSON.stringify(data),
            headers: { "Content-Type": "application/json" }
        }).then(res => {
            busy = "";
            if (res.status == 200) {
                onedit?.();
                popup.hide();
            } else {
                res.json().then(t => {
                    error = t.message ?? "Something went wrong";
                }).catch(e => {
                    error = "Something went wrong";
                })
            }
        })
    }

    function deleteTarget() {
        busy = "delete";
        fetch("/api/db/targets/" + data.id, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" }
        }).then(res => {
            busy = "";
            if (res.status == 200) {
                onedit?.();
                popup.hide();
            } else {
                res.json().then(t => {
                    error = t.message ?? "Something went wrong";
                }).catch(e => {
                    error = "Something went wrong";
                })
            }
        })
    }

    export function show(obj: typeof data | null) {
        if (obj) {
            data = {
                ...obj,
                connSecret: "",
            }
        } else {
            data = {
                id: 0,
                type: 0,
                name: "",
                connString: "",
                connSecret: "",
            }
        }
        popup.show();
    }
</script>

<Popup bind:this={popup} {onhide}>
    <h2>{data.id ? "Edit" : "Add"} database target</h2>
    <Form {busy} class="detailed" onsubmit={submit}>
        <hr/>
        {#if !!error}
            <div class="error">
                <span>{error}</span>
            </div>
        {/if}
        <div>
            <label for="db-target-type">
                database type
            </label>
            <select bind:value={data.type} placeholder="" disabled={!!busy}
                id="db-target-type">
                <option value={0}>MySQL</option>
            </select>
        </div>
        <div>
            <label for="db-target-name">
                label
            </label>
            <input type="text" bind:value={data.name} 
                id="db-target-name" autocomplete="off"
                placeholder="" disabled={!!busy}/>
        </div>
        <div>
            <label for="db-target-conn-string">
                connection string
            </label>
            <input type="text" bind:value={data.connString} 
                id="db-target-conn-string" autocomplete="off"
                placeholder="" disabled={!!busy}/>
        </div>
        <div>
            <label for="db-target-conn-secret">
                connection secret
            </label>
            <input type="password" bind:value={data.connSecret} 
                id="db-target-conn-secret" autocomplete="off"
                placeholder={data.id ? "(set to override)" : ""} disabled={!!busy}/>
        </div>
        <hr/>
        <div>
            {#if data.id}
                <Button actionName={"delete"} {busy} type="button" 
                    onclick={deleteTarget}>
                    delete
                </Button>
            {/if}
            <span class="flex-space"></span>
            <Button actionName={"submit"} {busy} type="submit">
                submit
            </Button>
        </div>
    </Form>
</Popup>