using System.Collections.Generic;
using Lidarr.Http.REST;
using NzbDrone.Core.Music;
using NzbDrone.Core.DecisionEngine;
using NzbDrone.SignalR;

namespace NzbDrone.Api.Tracks
{
    public class TrackModule : TrackModuleWithSignalR
    {
        public TrackModule(IArtistService artistService,
                             ITrackService trackService,
                             IUpgradableSpecification upgradableSpecification,
                             IBroadcastSignalRMessage signalRBroadcaster)
            : base(trackService, artistService, upgradableSpecification, signalRBroadcaster)
        {
            GetResourceAll = GetTracks;
            UpdateResource = SetMonitored;
        }

        private List<TrackResource> GetTracks()
        {
            if (!Request.Query.ArtistId.HasValue)
            {
                throw new BadRequestException("artistId is missing");
            }

            var artistId = (int)Request.Query.ArtistId;

            var resources = MapToResource(_trackService.GetTracksByArtist(artistId), false, true);

            return resources;
        }

        private void SetMonitored(TrackResource trackResource)
        {
            _trackService.SetTrackMonitored(trackResource.Id, trackResource.Monitored);
        }
    }
}
