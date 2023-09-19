using GQIDataSources.Utility;
using Skyline.DataMiner.Analytics.GenericInterface;
using SkylineAPI.Connector.Connectors.SharePoint.DCP.Base.Lists.AttachmentLists;
using SkylineAPI.Model.SharePoint.DCP.Base.Lists;
using SkylineAPI.Model.SharePoint.DCP.Base.Lists.AttachmentLists;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GQIDataSources.Dynamic
{
    public class DCPTasks : CSVDataCacher, IGQIInputArguments, IGQIOnPrepareFetch
    {
        private readonly Dictionary<string, string> _userToDomain = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string _assignees;

        protected override TimeSpan CacheDuration => TimeSpan.FromMinutes(30);

        protected override GQIColumn[] GetColumnsInner()
        {
            return new GQIColumn[]
            {
                new GQIStringColumn("Name"),
                new GQIStringColumn("Description"),
                new GQIDateTimeColumn("Start"),
                new GQIDateTimeColumn("Stop"),
                new GQIStringColumn("State"),
                new GQIStringColumn("Assignee"),
                new GQIIntColumn("Task ID"),
                new GQIStringColumn("Task state"),
            };
        }

        public GQIArgument[] GetInputArguments()
        {
            return new[]
            {
                new GQIStringArgument("Assignees") { IsRequired = false }
            };
        }

        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            _assignees = args.GetArgumentValue<string>("Assignees");

            return new OnArgumentsProcessedOutputArgs();
        }

        public OnPrepareFetchOutputArgs OnPrepareFetch(OnPrepareFetchInputArgs args)
        {
            InitializeDomainCache();
            return new OnPrepareFetchOutputArgs();
        }

        protected override GQIPage GetNextPageInner(GetNextPageInputArgs args)
        {
            InitializeDomainCache();
            var tasks = TasksConnector.ByLabel(new[] { "sprint goal" }).Where(x =>
                !String.Equals(x.Status, "Rejected", StringComparison.OrdinalIgnoreCase)
                && !String.Equals(x.Status, "Closed - Rejected", StringComparison.OrdinalIgnoreCase));

            if (!String.IsNullOrEmpty(_assignees))
            {
                var domains = _assignees.Split(',').ToHashSet(StringComparer.OrdinalIgnoreCase);
                tasks = tasks.Where(x => _assignees.Contains(x.Assignee.Name));
            }

            var rows = new List<GQIRow>();
            foreach (var task in tasks)
            {
                if (!int.TryParse(task.ID, out var taskID))
                    taskID = -1;

                if (!_userToDomain.TryGetValue(task.Assignee.Name, out var assignee))
                    assignee = task.Assignee.Name;

                rows.Add(new GQIRow(new[]
                {
                    new GQICell()
                    {
                        Value = task.Title
                    },
                    new GQICell()
                    {
                        Value = task.Description
                    },
                    new GQICell()
                    {
                        Value = task.StartDate
                    },
                    new GQICell()
                    {
                        Value = task.DueDate
                    },
                    new GQICell()
                    {
                        Value = task.Status
                    },
                    new GQICell()
                    {
                        Value = assignee
                    },
                    new GQICell()
                    {
                        Value = taskID
                    },
                    new GQICell()
                    {
                        Value = task.Status
                    }
                }));
            }

            return new GQIPage(rows.ToArray())
            {
                HasNextPage = false
            };
        }

        protected override void FillInCache()
        {
            SkylineAPI.Connector.CallExecutor.SetAuthentication("SKYLINE2\\svc_collab-poller", @"QPv7r0HLVW-*GfLe!!$FxF6CaYr7xtYmyaH");
            base.FillInCache();
        }

        private void InitializeDomainCache()
        {
            foreach (var domainID in Enumerable.Range(65, 7))
                foreach (var membership in GetMemberships(domainID.ToString()))
                    _userToDomain[membership.User.Name] = membership.Group.Name.StartsWith("domain.create - ", StringComparison.Ordinal) ? membership.Group.Name.Substring("domain.create - ".Length) : membership.Group.Name;
        }

        private DCPGroupMembership[] GetMemberships(String domainID)
        {
            return SkylineAPI.Connector.Connectors.SharePoint.DCP.Base.Lists.GroupMembershipsConnector.ByGroup(new[] { domainID }, new[] { "Member", "Agile Coach", "QX Coach", "Product Owner", "Group User" });
        }
    }
}