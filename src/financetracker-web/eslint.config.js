import js from '@eslint/js'
import globals from 'globals'
import tseslint from 'typescript-eslint'
import pluginVue from 'eslint-plugin-vue'
import { globalIgnores } from 'eslint/config'

export default tseslint.config([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,vue}'],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      ...pluginVue.configs['flat/recommended'],
    ],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
      parserOptions: {
        parser: tseslint.parser,
      },
    },
  },
])
