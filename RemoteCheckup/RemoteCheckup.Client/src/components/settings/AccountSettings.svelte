<script lang="ts">
    import Form from "../Form.svelte";
    import ChangePasswordModal from "../modals/ChangePasswordModal.svelte";
    import Popup from "../Popup.svelte";

    let changePwModal: ChangePasswordModal;
</script>

<h2>account settings</h2>
<button onclick={() => changePwModal.getPopup().show()}>change password</button>
<ChangePasswordModal bind:this={changePwModal} />