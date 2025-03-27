<script lang="ts">
  import { getContext } from "svelte";
    import Button from "./Button.svelte";
  import Form from "./Form.svelte";
import PopupBody from "./PopupBody.svelte";

    let username = $state("");
    let password = $state("");
    let persist = $state(false);
    
    let busy = $state("");
    let error = $state("");

    let loginInfo = getContext("login-info") as object;

    function logIn(e: SubmitEvent) {
        e.preventDefault();
        busy = "auth";
        fetch("/api/auth/login", {
            method: "POST",
            body: JSON.stringify({username, password, persist}),
            headers: { "Content-Type": "application/json" }
        }).then(res => {
            busy = "";
            if (res.status == 200) {
                Object.assign(loginInfo, {
                    loggedIn: true,
                    name: username,
                });
            } else {
                res.json().then(t => {
                    error = t.message ?? "Something went wrong";
                }).catch(e => {
                    error = "Something went wrong";
                })
            }
        })
    }
</script>

<svelte:head>
    <title>Welcome</title>
</svelte:head>

<PopupBody>
    <h1>Welcome</h1>
    <Form {busy} onsubmit={logIn}>
        <hr/>
        {#if !!error}
            <div class="error">
                <span>{error}</span>
            </div>
        {/if}
        <div>
            <input type="text" bind:value={username}
                name="username" id="login-username" autocomplete="off"
                placeholder="" disabled={!!busy} />
                <label for="login-username">
                    username
                </label>
        </div>
        <div>
            <input type="password" bind:value={password}
                name="password" id="login-password" autocomplete="off"
                placeholder="" disabled={!!busy} />
                <label for="login-password">
                    password
                </label>
        </div>
        <div>
            <input type="checkbox" bind:checked={persist}
                name="persist" id="login-persist"
                disabled={!!busy}>
            <label for="login-persist">
                remember my login
            </label>
        </div>
        <hr/>
        <div>
            <span class="flex-space"></span>
            <Button actionName={"auth"} {busy} type="submit">
                authenticate
            </Button>
        </div>
    </Form>
</PopupBody>
