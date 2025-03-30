<script lang="ts">
    import Form from "../Form.svelte";
    import ChangePasswordModal from "../modals/ChangePasswordModal.svelte";
    import Popup from "../Popup.svelte";

    import "../../common/css/settings.css";

    let changePwModal: ChangePasswordModal;
</script>

<h2>Account Settings</h2>
<section class="setting-section">
    <h3>Account Actions</h3>
    <div>
        <button onclick={() => changePwModal.getPopup().show()}>change password</button>
        <ChangePasswordModal bind:this={changePwModal} />
    </div>
</section>