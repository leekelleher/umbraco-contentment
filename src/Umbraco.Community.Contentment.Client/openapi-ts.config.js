import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
	client: 'legacy/fetch',
	debug: true,
	input: 'http://localhost:21185/umbraco/swagger/contentment/swagger.json',
	output: {
		path: 'src/api',
		format: 'prettier',
		lint: 'eslint',
	},
	plugins: [
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
