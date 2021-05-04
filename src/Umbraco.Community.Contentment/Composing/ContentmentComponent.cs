/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Community.Contentment.Migrations;

namespace Umbraco.Community.Contentment.Composing
{
    internal sealed class ContentmentComponent : IComponent
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger<Upgrader> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public ContentmentComponent(
            IScopeProvider scopeProvider,
            IMigrationBuilder migrationBuilder,
            IKeyValueService keyValueService,
            ILogger<Upgrader> logger,
            ILoggerFactory loggerFactory)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public void Initialize()
        {
            var upgrader = new Upgrader(new ContentmentPlan());
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger, _loggerFactory);
        }

        public void Terminate()
        { }
    }
}
