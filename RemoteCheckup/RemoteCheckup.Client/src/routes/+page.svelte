<script lang="ts">
  import { getContext, type Snippet } from "svelte";

    let slots = getContext("layout-slots") as {
        subnav?: Snippet
    };
    let loginInfo = getContext("login-info") as {
        name: string
    };
    Object.assign(slots, { subnav: null });
</script>

<h1>Welcome, {loginInfo.name}</h1>
<p>
    Insert dashboard here
</p>