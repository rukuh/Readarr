using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NzbDrone.Common.TPL;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.ImportLists
{
    public interface IFetchAndParseImportList
    {
        List<ImportListItemInfo> Fetch();
        List<ImportListItemInfo> FetchSingleList(ImportListDefinition definition);
    }

    public class FetchAndParseImportListService : IFetchAndParseImportList
    {
        private readonly IImportListFactory _importListFactory;
        private readonly IImportListStatusService _importListStatusService;
        private readonly Logger _logger;

        public FetchAndParseImportListService(IImportListFactory importListFactory, IImportListStatusService importListStatusService, Logger logger)
        {
            _importListFactory = importListFactory;
            _importListStatusService = importListStatusService;
            _logger = logger;
        }

        public List<ImportListItemInfo> Fetch()
        {
            var result = new List<ImportListItemInfo>();

            var importLists = _importListFactory.AutomaticAddEnabled();

            if (!importLists.Any())
            {
                _logger.Debug("No enabled import lists, skipping.");
                return result;
            }

            _logger.Debug("Available import lists {0}", importLists.Count);

            var taskList = new List<Task>();
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

            foreach (var importList in importLists)
            {
                var importListLocal = importList;
                var importListStatus = _importListStatusService.GetLastSyncListInfo(importListLocal.Definition.Id);

                if (importListStatus.HasValue && DateTime.UtcNow < importListStatus + importListLocal.MinRefreshInterval)
                {
                    _logger.Trace("Skipping refresh of Import List {0} due to minimum refresh inverval", importListLocal.Definition.Name);
                    continue;
                }

                var task = taskFactory.StartNew(() =>
                     {
                         try
                         {
                             var importListReports = importListLocal.Fetch();

                             lock (result)
                             {
                                 _logger.Debug("Found {0} from {1}", importListReports.Count, importList.Name);

                                 result.AddRange(importListReports);
                             }

                             _importListStatusService.UpdateListSyncStatus(importList.Definition.Id);
                         }
                         catch (Exception e)
                         {
                             _logger.Error(e, "Error during Import List Sync");
                         }
                     }).LogExceptions();

                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            result = result.DistinctBy(r => new { r.Author, r.Book }).ToList();

            _logger.Debug("Found {0} reports", result.Count);

            return result;
        }

        public List<ImportListItemInfo> FetchSingleList(ImportListDefinition definition)
        {
            var result = new List<ImportListItemInfo>();

            var importList = _importListFactory.GetInstance(definition);

            if (importList == null || !definition.EnableAutomaticAdd)
            {
                _logger.Debug("Import list not enabled, skipping.");
                return result;
            }

            var importListStatus = _importListStatusService.GetLastSyncListInfo(importList.Definition.Id);

            if (importListStatus.HasValue && DateTime.UtcNow < importListStatus + importList.MinRefreshInterval)
            {
                _logger.Trace("Skipping refresh of Import List {0} due to minimum refresh inverval", importList.Definition.Name);
                return result;
            }

            var taskList = new List<Task>();
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

            var importListLocal = importList;

            var task = taskFactory.StartNew(() =>
            {
                try
                {
                    var importListReports = importListLocal.Fetch();

                    lock (result)
                    {
                        _logger.Debug("Found {0} from {1}", importListReports.Count, importList.Name);

                        result.AddRange(importListReports);
                    }

                    _importListStatusService.UpdateListSyncStatus(importList.Definition.Id);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error during Import List Sync");
                }
            }).LogExceptions();

            taskList.Add(task);

            Task.WaitAll(taskList.ToArray());

            result = result.DistinctBy(r => new { r.Author, r.Book }).ToList();

            return result;
        }
    }
}
