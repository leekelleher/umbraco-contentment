/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Web.Mvc;
using Umbraco.Community.Contentment.DataEditors;
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
            var model = new ContentBlockPreviewModel<TPublishedContent, TPublishedElement>();

            if (viewData.TryGetValue("content", out var tmp1) && tmp1 is TPublishedContent t1)
            {
                model.Content = t1;
            }

            if (viewData.TryGetValue("element", out var tmp2) && tmp2 is TPublishedElement t2)
            {
                model.Element = t2;
            }

            viewData.Model = model;

            base.SetViewData(viewData);
        }
    }
}
