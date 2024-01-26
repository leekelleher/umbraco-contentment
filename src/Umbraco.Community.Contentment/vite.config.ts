import { defineConfig } from "vite";

export default defineConfig({
    build: {
        lib: {
            "entry": ["entry-point.ts"],
            "formats": ["es"]
        },
        outDir: "wwwroot/App_Plugins/Contentment/extensions",
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },
    },
});
