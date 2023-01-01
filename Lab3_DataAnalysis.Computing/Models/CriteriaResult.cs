using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3_DataAnalysis.Computing.Models
{
    public class CriteriaResult
    {
        public double CriteriaStatistics { get; set; }
        public double? ExtraCriteriaStatstics { get; set; } = null;
        public string Summary { get; set; }

        public string ToString(string format = "")
        {
            if(ExtraCriteriaStatstics == null)
            {
                return "(" + CriteriaStatistics.ToString(format) + ", " + Summary + ")";
            }

            return "(" + CriteriaStatistics.ToString(format) + ", " + ((double)ExtraCriteriaStatstics).ToString(format) + ", " + Summary + ")";
        }
    }
}
