// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as buttons } from './buttons/manifest.js';
import { manifests as bytes } from './bytes/manifests.js';
import { manifest as cascadingDropdownList } from './cascading-dropdown-list/manifest.js';
import { manifest as checkboxList } from './checkbox-list/manifest.js';
import { manifest as codeEditor } from './code-editor/manifest.js';
import { manifests as configurationEditor } from './configuration-editor/manifests.js';
import { manifests as contentBlocks } from './content-blocks/manifests.js';
import { manifest as contentSource } from './content-source/manifest.js';
import { manifest as dictionaryPicker } from './dictionary-picker/manifest.js';
import { manifests as dataList } from './data-list/manifests.js';
import { manifests as dataPicker } from './data-picker/manifests.js';
import { manifest as dropdownList } from './dropdown-list/manifest.js';
import { manifest as editorNotes } from './editor-notes/manifest.js';
import { manifests as iconPicker } from './icon-picker/manifests.js';
import { manifests as listItems } from './list-items/manifests.js';
import { manifest as memberTypePicker } from './member-type-picker/manifest.js';
import { manifests as notes } from './notes/manifests.js';
import { manifests as numberInput } from './number-input/manifests.js';
import { manifest as radioButtonList } from './radio-button-list/manifest.js';
import { manifest as readOnly } from './read-only/manifest.js';
import { manifests as renderMacro } from './render-macro/manifests.js';
import { manifests as socialLinks } from './social-links/manifests.js';
import { manifest as tags } from './tags/manifest.js';
import { manifests as templatedLabel } from './templated-label/manifests.js';
import { manifests as textboxList } from './textbox-list/manifests.js';
import { manifests as textInput } from './text-input/manifests.js';
import type {
	ManifestModal,
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestModal | ManifestPropertyEditorSchema | ManifestPropertyEditorUi> = [
	buttons,
	...bytes,
	cascadingDropdownList,
	checkboxList,
	codeEditor,
	...configurationEditor,
	...contentBlocks,
	contentSource,
	dictionaryPicker,
	...dataList,
	...dataPicker,
	dropdownList,
	editorNotes,
	...iconPicker,
	...listItems,
	memberTypePicker,
	...notes,
	...numberInput,
	radioButtonList,
	readOnly,
	...renderMacro,
	...socialLinks,
	tags,
	...templatedLabel,
	...textboxList,
	...textInput,
];
