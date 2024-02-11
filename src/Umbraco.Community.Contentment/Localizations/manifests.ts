import { ManifestLocalization } from "@umbraco-cms/backoffice/extension-registry";

export const manifests: Array<ManifestLocalization> = [
    {
        type: "localization",
        alias: "Umbraco.Community.Contentment.Language.EN_US",
        name: "English (US)",
        weight: 0,
        meta: {
            culture: "en-us",
        },
        js: () => import("./en-us.js")
    },
];
