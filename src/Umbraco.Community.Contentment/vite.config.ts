import { defineConfig } from "vite";

export default defineConfig({
    build: {
        lib: {
            "entry": [
                "DataEditors/Notes/notes.element.ts"
            ],
            "formats": ["es"]
        },
        outDir: "Web/UI/App_Plugins/Contentment/extensions",
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },
    },
});
