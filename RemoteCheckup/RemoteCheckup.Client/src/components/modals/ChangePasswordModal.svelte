
<script lang="ts">
    import Button from "../Button.svelte";
    import Form from "../Form.svelte";
    import Popup from "../Popup.svelte";

    let popup: Popup;

    let oldPassword = $state("");
    let newPassword = $state("");
    let newPassword2 = $state("");
    
    let busy = $state("");
    let error = $state("");

    function onhide() {
        oldPassword = newPassword = newPassword2 = busy = error = "";
    }

    function changePassword(e: SubmitEvent) {
        e.preventDefault();
        if (newPassword != newPassword2) {
            error = "Passwords do not match";
            return;
        }
        busy = "change-password";
        fetch("/api/auth/change-password", {
            method: "POST",
            body: JSON.stringify({oldPassword, newPassword, newPassword2}),
            headers: { "Content-Type": "application/json" }
        }).then(res => {
            busy = "";
            if (res.status == 200) {
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
    export function getPopup() {
        return popup;
    }
</script>

<Popup bind:this={popup} {onhide}>
    <h2>Change password</h2>
    <Form {busy} onsubmit={changePassword}>
        <hr/>
        {#if !!error}
            <div class="error">
                <span>{error}</span>
            </div>
        {/if}
        <div>
            <input type="password" bind:value={oldPassword}
                name="old-password" id="old-password" autocomplete="off"
                placeholder="" disabled={!!busy} />
            <label for="old-password">
                old password
            </label>
        </div>
        <div>
            <input type="password" bind:value={newPassword}
                name="new-password" id="new-password" autocomplete="off"
                placeholder="" disabled={!!busy} />
            <label for="new-password">
                new password
            </label>
        </div>
        <div>
            <input type="password" bind:value={newPassword2}
                name="new-password2" id="new-password2" autocomplete="off"
                placeholder="" disabled={!!busy} />
            <label for="new-password2">
                new password again
            </label>
        </div>
        <hr/>
        <div>
            <span class="flex-space"></span>
            <Button actionName={"change-password"} {busy} type="submit">
                change password
            </Button>
        </div>
    </Form>
</Popup>