import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
	debug: true,
	input: 'http://localhost:21187/umbraco/swagger/contentment/swagger.json',
	output: {
		path: 'src/api',
		format: 'prettier',
		lint: 'eslint',
	},
	plugins: [
		{
			name: '@hey-api/client-fetch',
			baseUrl: false,
			bundle: false,
			exportFromIndex: false,
			throwOnError: true,
		},
		{
			name: '@hey-api/typescript',
			enums: 'typescript',
		},
		{
			name: '@hey-api/sdk',
			asClass: true,
		},
	],
});
