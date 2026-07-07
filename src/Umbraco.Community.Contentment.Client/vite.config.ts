import { defineConfig } from 'vite';

export default defineConfig({
	build: {
		lib: {
			entry: { index: 'src/index.ts', manifests: 'src/manifests.ts' },
			formats: ['es'],
		},
		outDir: '../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment',
		emptyOutDir: true,
		sourcemap: true,
		rollupOptions: {
			external: [/^@umbraco/],
			output: {
				entryFileNames: '[name].js',
				chunkFileNames: '[name].js',
			},
		},
	},
});
