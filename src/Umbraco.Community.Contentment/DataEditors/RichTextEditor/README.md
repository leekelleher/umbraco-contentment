## Rich Text Editor

### Used interally by

This Rich Text Editor is intended to exclusively used by Notes and Editor Notes configuration editors.

Umbraco v13 introduced a breaking-change, due to the inclusion of the Inline Blocks feature (replacing inline Macros).
https://github.com/umbraco/Umbraco-CMS/pull/15029/files#diff-429a0a5b9fe236b792a692145f07d5aef201f4eaeae1b321f0dd5d698577f8ebR433

In order to workaround this breaking-change and maintain support for the Notes and Editor Notes editors,
this Rich Text Editor will only work with Umbraco v13 and does not support the Inline Blocks feature.
