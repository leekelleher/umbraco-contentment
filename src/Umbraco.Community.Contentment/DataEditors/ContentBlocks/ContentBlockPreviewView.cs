/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.Views;
using Umbraco.Community.Contentment.DataEditors;

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
        public override ViewContext ViewContext
        {
            get => base.ViewContext;
            set => base.ViewContext = SetViewData(value);
        }

        protected ViewContext SetViewData(ViewContext viewCtx)
        {
            void setProperty<T>(string key, Action<T> action)
            {
                if (viewCtx.ViewData.TryGetValueAs(key, out T value) == true)
                {
                    action(value);
                }
            }

            var model = new ContentBlockPreviewModel<TPublishedContent, TPublishedElement>();

            setProperty<TPublishedContent>("content", (x) => model.Content = x);
            setProperty<TPublishedElement>("element", (x) => model.Element = x);
            setProperty<int>("elementIndex", (x) => model.ElementIndex = x);
            setProperty<string>("contentIcon", (x) => model.ContentTypeIcon = x);
            setProperty<string>("elementIcon", (x) => model.ElementTypeIcon = x);

            if (model.Element == null && viewCtx.ViewData.Model is TPublishedElement element)
            {
                model.Element = element;
            }

            if (model.Content == null && UmbracoContext?.PublishedRequest?.PublishedContent is TPublishedContent content)
            {
                model.Content = content;
            }

            viewCtx.ViewData.Model = model;

            return viewCtx;
        }
    }
}
