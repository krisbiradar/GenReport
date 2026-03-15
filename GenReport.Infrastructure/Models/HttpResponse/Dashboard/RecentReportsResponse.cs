using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenReport.Infrastructure.Models.HttpResponse.Dashboard
{
    /// <summary>
    /// List of Rencent Reports 
    /// </summary>
    public class RecentReportsResponse
    {
        public required IList<DashboardReportResponse> Reports{ get; set; }
    }
    public class DashboardReportResponse
    {
        public required string Name { get; set; }
        public required string RawQuery { get; set; }
        public int NoOfRows { get; set; }
        public required string StorageUrl { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
