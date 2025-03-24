<script lang="ts">
  import { getContext, type Snippet } from "svelte";

    let slots = getContext("layout-slots") as {
        subnav?: Snippet
    };
    Object.assign(slots, { subnav: null });
</script>

<h1>Welcome</h1>
<p>
    Insert dashboard here
</p>