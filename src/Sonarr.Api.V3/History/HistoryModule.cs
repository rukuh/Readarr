using System;
using Nancy;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.DecisionEngine;
using NzbDrone.Core.Download;
using NzbDrone.Core.History;
using Lidarr.Api.V3.Albums;
using Lidarr.Api.V3.Artist;
using Lidarr.Http;
using Lidarr.Http.Extensions;

namespace Lidarr.Api.V3.History
{
    public class HistoryModule : LidarrRestModule<HistoryResource>
    {
        private readonly IHistoryService _historyService;
        private readonly IUpgradableSpecification _upgradableSpecification;
        private readonly IFailedDownloadService _failedDownloadService;

        public HistoryModule(IHistoryService historyService,
                             IUpgradableSpecification upgradableSpecification,
                             IFailedDownloadService failedDownloadService)
        {
            _historyService = historyService;
            _upgradableSpecification = upgradableSpecification;
            _failedDownloadService = failedDownloadService;
            GetResourcePaged = GetHistory;

            Post["/failed"] = x => MarkAsFailed();
        }

        protected HistoryResource MapToResource(NzbDrone.Core.History.History model)
        {
            var resource = model.ToResource();

            resource.Artist = model.Artist.ToResource();
            resource.Album = model.Album.ToResource();

            if (model.Artist != null)
            {
                resource.QualityCutoffNotMet = _upgradableSpecification.CutoffNotMet(model.Artist.Profile.Value,
                                                                                     model.Artist.LanguageProfile,
                                                                                     model.Quality,
                                                                                     model.Language);
            }

            return resource;
        }

        private PagingResource<HistoryResource> GetHistory(PagingResource<HistoryResource> pagingResource)
        {
            var pagingSpec = pagingResource.MapToPagingSpec<HistoryResource, NzbDrone.Core.History.History>("date", SortDirection.Descending);

            if (pagingResource.FilterKey == "eventType")
            {
                var filterValue = (HistoryEventType)Convert.ToInt32(pagingResource.FilterValue);
                pagingSpec.FilterExpression = v => v.EventType == filterValue;
            }

            if (pagingResource.FilterKey == "albumId")
            {
                int albumId = Convert.ToInt32(pagingResource.FilterValue);
                pagingSpec.FilterExpression = h => h.AlbumId == albumId;
            }

            return ApplyToPage(_historyService.Paged, pagingSpec, MapToResource);
        }

        private Response MarkAsFailed()
        {
            var id = (int)Request.Form.Id;
            _failedDownloadService.MarkAsFailed(id);
            return new object().AsResponse();
        }
    }
}
