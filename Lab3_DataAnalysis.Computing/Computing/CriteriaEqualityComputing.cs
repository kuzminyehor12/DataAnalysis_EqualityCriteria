using DataAnalysis1.Computing.Computing;
using DataAnalysis1.DataSource;
using Lab3_DataAnalysis.Computing.Models;
using Lab3_DataAnalysis.DataSource.Series;
using System;
using System.Collections.Generic;
using System.Text;
using VariationalSeries = Lab3_DataAnalysis.DataSource.Series.VariationalSeries;

namespace Lab3_DataAnalysis.Computing.Interfaces
{
    public abstract class CriteriaEqualityComputing
    {
        private const double Alpha = 0.05;
        public VariationalSeries FirstDataSource { get; set; }
        public VariationalSeries SecondDataSource { get; set; }

        public CriteriaResult ForNormality(BaseSeries<double> series)
        {
            var quantileComputing = new SeriesQuantileComputing();
            var standartDeviationComputing = new StandartDeviationComputing(series);
            var pointEstimationComputing = new PointEstimationCharacteristicsComputing(series);
            var skewnessCriteria = pointEstimationComputing.ComputeSkewnessCoefficient() / standartDeviationComputing.ComputeForSkewness();
            var kurtosisCriteria = pointEstimationComputing.ComputeKurtosisCoefficient() / standartDeviationComputing.ComputeForKurtosis();

            return new CriteriaResult
            {
                CriteriaStatistics = skewnessCriteria,
                ExtraCriteriaStatstics = kurtosisCriteria,
                Summary = Math.Abs(skewnessCriteria) <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, series.Series.Count - 1)
                && Math.Abs(kurtosisCriteria) <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, series.Series.Count - 1) ? "Equal" : "Not Equal"
            };
        }
        public abstract CriteriaResult ForMean();
        public abstract CriteriaResult ForDispersion();
        public abstract CriteriaResult ForRank();
    }
}
