/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.Contentment.Services
{
    public interface IContentmentContentContext
    {
        /// <summary>
        /// Gets the identifier of the current content, or its parent's identifier when the current content is new and unsaved.
        /// </summary>
        /// <param name="isParent"><see langword="true"/> if the returned identifier is the parent's, rather than the current content's.</param>
        /// <returns>The content identifier, or <see langword="null"/> if it could not be resolved.</returns>
        public int? GetCurrentContentId(out bool isParent);

        /// <summary>
        /// Gets the current content.
        /// </summary>
        /// <param name="isParent"><see langword="true"/> if the returned content is the parent, rather than the current content.</param>
        /// <returns>The current content, or <see langword="null"/> if it could not be resolved.</returns>
        public IPublishedContent? GetCurrentContent(out bool isParent);
    }

    // NOTE: Added as a separate interface, so not to break binary backwards-compatibility. [LK]
    [Obsolete("To be combined with `IContentmentContentContext`. This interface will be removed in Contentment 8.0.")]
    public interface IContentmentContentContext2 : IContentmentContentContext
    {
        /// <summary>
        /// Gets the identifier of the current content, or its parent's identifier when the current content is new and unsaved, converted to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="isParent"><see langword="true"/> if the returned identifier is the parent's, rather than the current content's.</param>
        /// <returns>The content identifier, or <see langword="default"/> if it could not be resolved or converted.</returns>
        public T? GetCurrentContentId<T>(out bool isParent);
    }

    // NOTE: Added as a separate interface, so not to break binary backwards-compatibility. [LK]
    [Obsolete("To be combined with `IContentmentContentContext`. This interface will be removed in Contentment 8.0.")]
    public interface IContentmentContentContext3 : IContentmentContentContext2
    {
        /// <summary>
        /// Gets the variant identifier (e.g. culture and/or segment) of the current content.
        /// </summary>
        /// <returns>The variant identifier, or <see langword="null"/> if it could not be resolved.</returns>
        public string? GetCurrentVariantId();
    }

    // NOTE: Added as a separate interface, so not to break binary backwards-compatibility. [EW]
    [Obsolete("To be combined with `IContentmentContentContext`. This interface will be removed in Contentment 8.0.")]
    public interface IContentmentContentContext4 : IContentmentContentContext3
    {
        /// <summary>
        /// Gets the content type key of the current content, e.g. a document type, media type or member type.
        /// </summary>
        /// <returns>The content type key, or <see langword="null"/> if it could not be resolved.</returns>
        public Guid? GetCurrentContentTypeKey();
    }
}
