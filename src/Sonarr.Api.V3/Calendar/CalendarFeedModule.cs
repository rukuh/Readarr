using System;
using System.Collections.Generic;
using System.Linq;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Interfaces.Serialization;
using Ical.Net.Serialization;
using Ical.Net.Serialization.iCalendar.Factory;
using Nancy;
using Nancy.Responses;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Tags;
using NzbDrone.Core.Music;
using Lidarr.Http.Extensions;

namespace Lidarr.Api.V3.Calendar
{
    public class CalendarFeedModule : SonarrV3FeedModule
    {
        private readonly IAlbumService _albumService;
        private readonly ITagService _tagService;

        public CalendarFeedModule(IAlbumService albumService, ITagService tagService)
            : base("calendar")
        {
            _albumService = albumService;
            _tagService = tagService;

            Get["/Sonarr.ics"] = options => GetCalendarFeed();
        }

        private Response GetCalendarFeed()
        {
            var pastDays = 7;
            var futureDays = 28;            
            var start = DateTime.Today.AddDays(-pastDays);
            var end = DateTime.Today.AddDays(futureDays);
            var unmonitored = Request.GetBooleanQueryParameter("unmonitored");
            var premiersOnly = Request.GetBooleanQueryParameter("premiersOnly");
            var asAllDay = Request.GetBooleanQueryParameter("asAllDay");
            var tags = new List<int>();

            var queryPastDays = Request.Query.PastDays;
            var queryFutureDays = Request.Query.FutureDays;
            var queryTags = Request.Query.Tags;

            if (queryPastDays.HasValue)
            {
                pastDays = int.Parse(queryPastDays.Value);
                start = DateTime.Today.AddDays(-pastDays);
            }

            if (queryFutureDays.HasValue)
            {
                futureDays = int.Parse(queryFutureDays.Value);
                end = DateTime.Today.AddDays(futureDays);
            }

            if (queryTags.HasValue)
            {
                var tagInput = (string)queryTags.Value.ToString();
                tags.AddRange(tagInput.Split(',').Select(_tagService.GetTag).Select(t => t.Id));
            }

            var episodes = _albumService.AlbumsBetweenDates(start, end, unmonitored);
            var calendar = new Ical.Net.Calendar
            {
                ProductId = "-//sonarr.tv//Sonarr//EN"
            };


            foreach (var album in episodes.OrderBy(v => v.ReleaseDate.Value))
            {
                //if (premiersOnly && (album.SeasonNumber == 0 || album.EpisodeNumber != 1))
                //{
                //    continue;
                //}

                if (tags.Any() && tags.None(album.Artist.Tags.Contains))
                {
                    continue;
                }

                var occurrence = calendar.Create<Event>();
                occurrence.Uid = "NzbDrone_album_" + album.Id;
                //occurrence.Status = album.HasFile ? EventStatus.Confirmed : EventStatus.Tentative;
                //occurrence.Description = album.Overview;
                //occurrence.Categories = new List<string>() { album.Series.Network };

                occurrence.Start = new CalDateTime(album.ReleaseDate.Value) { HasTime = false };

                occurrence.Summary = $"{album.Artist.Name} - {album.Title}";
            }

            var serializer = (IStringSerializer)new SerializerFactory().Build(calendar.GetType(), new SerializationContext());
            var icalendar = serializer.SerializeToString(calendar);

            return new TextResponse(icalendar, "text/calendar");
        }
    }
}
