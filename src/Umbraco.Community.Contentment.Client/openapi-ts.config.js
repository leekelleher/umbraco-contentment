import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
	debug: true,
	input: 'http://localhost:29918/umbraco/openapi/contentment.json',
	output: {
		path: 'src/api',
		postProcess: ['eslint', 'prettier'],
	},
	plugins: [
		{
			name: '@hey-api/client-fetch',
			baseUrl: false,
			bundle: true,
			exportFromIndex: false,
			throwOnError: true,
		},
		{
			name: '@hey-api/typescript',
			enums: 'typescript',
		},
		{
			name: '@hey-api/sdk',
			responseStyle: 'fields',
			operations: {
				strategy: 'byTags',
				container: 'class',
				containerName: { name: '{{name}}Service', casing: 'PascalCase' },
			},
		},
	],
});
