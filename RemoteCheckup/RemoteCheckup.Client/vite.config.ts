import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [sveltekit()],
	server: {
		proxy: {
			'/api/': {
				target: 'http://localhost:5039',
				ws: true,
			}
		}
	}
});
