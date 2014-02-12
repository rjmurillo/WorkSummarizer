using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkSummarizer
{
    public class WorkItemNode
    {
        public WorkItem WorkItem { get; private set; }
        public List<int> ChangesetIds = new List<int>();
        public int Id { get { return WorkItem.Id; } }
        public int ParentId { get; private set; }
        public WorkItemNode(WorkItem workItem)
        {
            // link to changesets
            foreach (ExternalLink l in workItem.Links.OfType<ExternalLink>()
                .Where(p => p.LinkedArtifactUri.StartsWith("vstfs:///VersionControl/Changeset", StringComparison.CurrentCulture)))
            {
                ChangesetIds.Add(int.Parse(l.LinkedArtifactUri.Split('/').Last()));
            }

            // link to parent
            ParentId = -1;
            foreach (WorkItemLink wil in workItem.WorkItemLinks.Cast<WorkItemLink>().Where(wil => wil.LinkTypeEnd.ImmutableName == "System.LinkTypes.Hierarchy-Reverse"))
            {
                // if (ParentId != -1) warning: multiple parents!
                ParentId = wil.TargetId;
            }
        }
        public bool ContainsField(string fieldName)
        {
            return WorkItem != null && WorkItem.Fields.Contains(fieldName);
        }
        public T GetFieldValue<T>(string fieldName)
        {
            var defaultValue = default(T);
            Func<T, bool> trueValidationCheck = p => true;
            return GetFieldValue(fieldName, defaultValue, trueValidationCheck);
        }

        public T GetFieldValue<T>(string fieldName, T defaultValue)
        {
            Func<T, bool> trueValidationCheck = p => true;
            return GetFieldValue(fieldName, defaultValue, trueValidationCheck);
        }

        public T GetFieldValue<T>(string fieldName, T defaultValue, Func<T, bool> validationCheck)
        {
            if (!WorkItem.Fields.Contains(fieldName)) return defaultValue;

            if (WorkItem.Fields[fieldName].Value is T && validationCheck((T)WorkItem.Fields[fieldName].Value))
            {
                return (T)WorkItem.Fields[fieldName].Value;
            }
            return defaultValue;
        }
    }
}
