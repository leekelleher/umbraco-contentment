/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Examine.Search;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
using Umbraco.Examine;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Cms.Core.Composing.HideFromTypeFinder]
    public sealed class ExamineDataListSource : IDataListSource
    {
        private readonly IExamineManager _examineManager;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IIOHelper _ioHelper;
        private const string _defaultNameField = "nodeName";
        private const string _defaultValueField = UmbracoExamineIndex.NodeKeyFieldName;
        private const string _defaultIconField = UmbracoExamineIndex.IconFieldName;

        private readonly Dictionary<string, object> _examineFieldConfig = new Dictionary<string, object>
        {
            {
                Constants.Conventions.ConfigurationFieldAliases.Items,
                new[]
                {
                    LuceneIndex.CategoryFieldName,
                    LuceneIndex.ItemIdFieldName,
                    LuceneIndex.ItemTypeFieldName,
                    UmbracoExamineIndex.IconFieldName,
                    UmbracoExamineIndex.IndexPathFieldName,
                    UmbracoExamineIndex.NodeKeyFieldName,
                    UmbracoExamineIndex.PublishedFieldName,
                    UmbracoExamineIndex.UmbracoFileFieldName,
                    "createDate",
                    "creatorID",
                    "creatorName",
                    "icon",
                    "id",
                    "level",
                    _defaultNameField,
                    "nodeType",
                    "parentID",
                    "path",
                    "templateID",
                    "updateDate",
                    "urlName",
                    "writerID",
                    "writerName",
                }.Select(x => new DataListItem { Name = x, Value = x })
            },
        };

        public ExamineDataListSource(
            IExamineManager examineManager,
            IShortStringHelper shortStringHelper,
            IIOHelper ioHelper)
        {
            _examineManager = examineManager;
            _shortStringHelper = shortStringHelper;
            _ioHelper = ioHelper;
        }

        public string Name => "Examine Query";

        public string Description => "Populate the data source from an Examine query.";

        public string Icon => "icon-search";

        public string Group => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "examineIndex",
                Name = "Examine Index",
                Description = "Select the Examine index.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DropdownListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { DropdownListDataListEditor.AllowEmpty, Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, _examineManager.Indexes.OrderBy(x => x.Name).Select(x => new DataListItem { Name = x.Name.SplitPascalCasing(_shortStringHelper), Value = x.Name }) },
                }
            },
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>Do you need help with Lucene query?</strong></summary>
<p>If you need assistance with Lucene query syntax, please refer to this resource on <a href=""https://our.umbraco.com/documentation/reference/searching/examine/overview-explanation#power-searching-with-raw-lucene-queries"" target=""_blank""><strong>our.umbraco.com</strong></a>.</p>
</details>", true),
            new ConfigurationField
            {
                Key = "luceneQuery",
                Name = "Lucene query",
                Description = "Enter your raw Lucene expression to query Examine with.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "text" },
                    { CodeEditorConfigurationEditor.MinLines, 1 },
                    { CodeEditorConfigurationEditor.MaxLines, 5 },
                }
            },
            new ConfigurationField
            {
                Key = "nameField",
                Name = "Name Field",
                Description = "Enter the field name to select the name from the Examine record.",
                View =  IOHelper.ResolveUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
            new ConfigurationField
            {
                Key = "valueField",
                Name = "Value Field",
                Description = "Enter the field name to select the value (key) from the Examine record.",
                View =  IOHelper.ResolveUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
            new ConfigurationField
            {
                Key = "iconField",
                Name = "Icon Field",
                Description = "<em>(optional)</em> Enter the field name to select the icon from the Examine record.",
                View =  IOHelper.ResolveUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
            new ConfigurationField
            {
                Key = "descriptionField",
                Name = "Description Field",
                Description = "<em>(optional)</em> Enter the field name to select the description from the Examine record.",
                View =  IOHelper.ResolveUrl(TextInputDataEditor.DataEditorViewPath),
                Config = _examineFieldConfig
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "examineIndex", UmbConstants.UmbracoIndexes.ExternalIndexName },
            { "luceneQuery", "+__IndexType:content" },
            { "nameField", _defaultNameField },
            { "valueField", _defaultValueField },
            { "iconField", _defaultIconField },
            { "descriptionField", string.Empty },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var examineIndex = config.GetValueAs("examineIndex", UmbConstants.UmbracoIndexes.ExternalIndexName);
            if (_examineManager.TryGetIndex(examineIndex, out var index) == true)
            {
                var luceneQuery = config.GetValueAs("luceneQuery", string.Empty);
                if (string.IsNullOrWhiteSpace(luceneQuery) == false)
                {
                    var nameField = config.GetValueAs("nameField", _defaultNameField);
                    var valueField = config.GetValueAs("valueField", _defaultValueField);
                    var iconField = config.GetValueAs("iconField", _defaultIconField);
                    var descriptionField = config.GetValueAs("descriptionField", string.Empty);

                    var results = index
                        .GetSearcher()
                        .CreateQuery()
                        .NativeQuery(luceneQuery)
                        // NOTE: For any `OrderBy` complaints, refer to: https://github.com/Shazwazza/Examine/issues/126
                        .OrderBy(new SortableField(nameField, SortType.String))
                        .Execute();

                    if (results?.TotalItemCount > 0)
                    {
                        return results.Select(x => new DataListItem
                        {
                            Name = x.Values.ContainsKey(nameField) == true ? x.Values[nameField] : x.Values[_defaultNameField],
                            Value = x.Values.ContainsKey(valueField) == true ? x.Values[valueField] : x.Values[_defaultValueField],
                            Icon = x.Values.ContainsKey(iconField) == true ? x.Values[iconField] : x.Values[_defaultIconField],
                            Description = x.Values.ContainsKey(descriptionField) == true ? x.Values[descriptionField] : null,
                        });
                    }
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
