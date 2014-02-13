using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSources.ManicTime
{
    public class ManicTimeDataProvider
    {
        /***
         * CREATE TABLE [Activity] (
              [ActivityId] int NOT NULL
            , [TimelineId] int NOT NULL
            , [DisplayName] nvarchar(4000) NOT NULL
            , [GroupId] int NULL
            , [GroupListId] int NULL
            , [StartLocalDate] int NOT NULL
            , [StartLocalTime] datetime NOT NULL
            , [StartUtcTime] datetime NOT NULL
            , [EndLocalDate] int NOT NULL
            , [EndLocalTime] datetime NOT NULL
            , [EndUtcTime] datetime NOT NULL
            , [TextData] nvarchar(4000) NULL
            , [UpdatedTimestamp] int NOT NULL
            );
         * 
         * CREATE TABLE [Group] (
              [GroupId] int NOT NULL
            , [TimelineId] int NOT NULL
            , [DisplayName] nvarchar(4000) NULL
            , [Color] int NOT NULL
            , [Icon16] image NULL
            , [Icon32] image NULL
            , [FolderId] int NULL
            , [TextData] nvarchar(4000) NULL
            , [Key] nvarchar(50) NOT NULL
            , [DeleteWhenNotUsed] bit NOT NULL
            , [UpdatedTimestamp] int NOT NULL
            , [SkipColor] bit NOT NULL
            );
         */
        public static void PullActivities(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            var manicTimeDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Local\\Finkit\\ManicTime");

            if(!Directory.Exists(manicTimeDbPath))
            {
                manicTimeDbPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            using(var conn = new SqlCeConnection("Data Source = " + Path.Combine(manicTimeDbPath, "ManicTime.sdf") + ";Persist Security Info=False"))
            {                
                conn.Open();

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 100 act.[DisplayName], act.[StartUtcTime], act.[EndUtcTime], act.[TextData], grp.[DisplayName] GroupDisplayName FROM [Activity] act INNER JOIN [Group] grp ON act.GroupId = grp.GroupId  WHERE act.DisplayName <> '' ORDER BY act.EndUtcTime DESC";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader[0] + " " + reader[1]);
                }
            }
        }
    }
}
