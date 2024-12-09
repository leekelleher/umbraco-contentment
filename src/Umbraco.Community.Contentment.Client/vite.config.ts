import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    lib: {
      entry: 'src/bundle.ts',
      formats: ['es'],
    },
    outDir: '../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment',
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      external: [/^@umbraco/],
      output: {
        chunkFileNames: '[name].js',
      }
    },
  },
});