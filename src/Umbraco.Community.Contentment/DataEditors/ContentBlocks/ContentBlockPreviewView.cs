/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Web.Mvc;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.Web.Mvc
{
    public abstract class ContentBlockPreviewView
        : ContentBlockPreviewView<IPublishedContent, IPublishedElement>
    { }

    public abstract class ContentBlockPreviewView<TPublishedContent, TPublishedElement>
        : UmbracoViewPage<ContentBlockPreviewModel<TPublishedContent, TPublishedElement>>
        where TPublishedContent : IPublishedContent
        where TPublishedElement : IPublishedElement
    {
        protected override void SetViewData(ViewDataDictionary viewData)
        {
            void setProperty<T>(string key, Action<T> action)
            {
                if (viewData.TryGetValueAs(key, out T tmp))
                {
                    action(tmp);

                    // TODO: [LK:2020-12-29] Uncomment the removal in v2.0.
                    // I'm only keeping the value in the view-data for backwards-compat.
                    //viewData.Remove(key);
                }
            }

            var model = new ContentBlockPreviewModel<TPublishedContent, TPublishedElement>();

            setProperty<TPublishedContent>("content", (x) => model.Content = x);
            setProperty<TPublishedElement>("element", (x) => model.Element = x);
            setProperty<int>("elementIndex", (x) => model.ElementIndex = x);
            setProperty<string>("contentIcon", (x) => model.ContentTypeIcon = x);
            setProperty<string>("elementIcon", (x) => model.ElementTypeIcon = x);

            viewData.Model = model;

            base.SetViewData(viewData);
        }
    }
}
