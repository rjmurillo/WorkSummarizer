using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace TFSToCodeSwarm
{
    class Program
    {
        public static void GetChanges(string _tfsServer, string _tfsRepository, string _versionFromString)
        {
            //connection to TFS Version Control
            TeamFoundationServer tfs = TeamFoundationServerFactory.GetServer(_tfsServer);
            VersionControlServer vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));

            // Null means *all*
            VersionSpec versionSpecFrom = null;
            if (null != _versionFromString && "" != _versionFromString)
            {
                versionSpecFrom = VersionSpec.ParseSingleSpec(_versionFromString, null);
            }

            System.Collections.IEnumerable enumerable = vcs.QueryHistory(_tfsRepository,
                  VersionSpec.Latest,
                  0,
                  RecursionType.Full,
                  "",
                  versionSpecFrom,
                  VersionSpec.Latest,
                  Int32.MaxValue,
                  true,
                  true);

            //Console.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
            //Console.WriteLine("<File_Events>");

            //var dudes = new[] { "jocarrie", "cdockter", "krisrai", "edkotows", "olivialu", "stephpic", "rcook", "ryanvog", "ericgu", "cshi", "urosv", "paveg", "a-alfral", "a-beelle", "a-kesemz", "a-hoyap", "a-ricg", "a-machaf", "a-paveg", "a-raaraj", "a-xiani" };
            foreach (Changeset changeset in enumerable.Cast<Changeset>().Reverse())
            {
                //var comment = changeset.Comment;
                //if (comment.Contains("FI") || comment.Contains("RI"))
                //{
                //    continue;
                //}

                //foreach (Change change in changeset.Changes)
                //{
                //    if (change.ChangeType == ChangeType.Merge || change.ChangeType == ChangeType.Branch ||
                //        change.ChangeType == ChangeType.Rollback)
                //    {
                //        continue;
                //    }

                //    long numSeconds = (long) (change.Item.CheckinDate -
                //                              new DateTime(1970, 1, 1, 0, 0, 0)).
                //                                 TotalMilliseconds;
                //    String strItemName = change.Item.ServerItem.Replace('&', ' ');

                //    //String strDetails = String.Format("{{ owner:\"{0}\",  commit_ticks:\"{1}\"}}", changeset.Owner.Split('\\')[1], change.Item.CheckinDate.Ticks, changeset.Comment);
                //    ////String strDetails = String.Format("<event date=\"{1}\" author=\"{0}\" filename=\"{2}\" />", changeset.Owner.Split('\\')[1], numSeconds, strItemName);
                //    //Console.WriteLine(strDetails);
                //}

                String strDetails = String.Format("{{ owner:\"{0}\",  commit_ticks:\"{1}\", comment:\"{2}\", work_item_ids=\"{3}\" }}", changeset.Owner.Split('\\')[1], changeset.CreationDate.Ticks, changeset.Comment.Replace(Environment.NewLine, "\\n"), String.Join(",",changeset.WorkItems.Select(p => p.Id)));
                //String strDetails = String.Format("<event date=\"{1}\" author=\"{0}\" filename=\"{2}\" />", changeset.Owner.Split('\\')[1], numSeconds, strItemName);
                Console.WriteLine(strDetails);
            }

            //Console.WriteLine("</File_Events>");
        }

        static void Main(string[] args)
        {
            if (3 == args.Length)
            {
                GetChanges(args[0], args[1], args[2]);
            }
            else
            {
                Console.WriteLine("\nUsage: TFSToCodeSwarm <SERVERURL> <REPOSITORY> <STARTDATE>\n");
                Console.WriteLine("\nExample: TFSToCodeSwarm \"http://tfs\" \"$/SourceCode\" \"D03/02/2009\"\n");
            }
        }
    }
}
