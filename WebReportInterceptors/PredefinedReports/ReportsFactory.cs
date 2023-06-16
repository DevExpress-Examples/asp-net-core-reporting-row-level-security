using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;

namespace WebReport.PredefinedReports
{
    public static class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            ["OrdersReport"] = () => new OrdersReport()
        };
    }
}
