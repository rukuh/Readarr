﻿using FluentValidation;
using NzbDrone.Core.Configuration;
using Lidarr.Http.Validation;

namespace Lidarr.Api.V3.Config
{
    public class IndexerConfigModule : SonarrConfigModule<IndexerConfigResource>
    {

        public IndexerConfigModule(IConfigService configService)
            : base(configService)
        {
            SharedValidator.RuleFor(c => c.MinimumAge)
                           .GreaterThanOrEqualTo(0);

            SharedValidator.RuleFor(c => c.Retention)
                           .GreaterThanOrEqualTo(0);

            SharedValidator.RuleFor(c => c.RssSyncInterval)
                           .IsValidRssSyncInterval();
        }

        protected override IndexerConfigResource ToResource(IConfigService model)
        {
            return IndexerConfigResourceMapper.ToResource(model);
        }
    }
}