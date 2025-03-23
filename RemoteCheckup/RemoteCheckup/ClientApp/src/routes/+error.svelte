<script lang="ts">
    import { page } from "$app/state";
</script>

<h1>Whoops</h1>
<p>{page.status}: {page.error?.message}</p>