using DataAnalysis1.Computing.Computing;
using Lab3_DataAnalysis.Computing.Extensions;
using Lab3_DataAnalysis.Computing.Interfaces;
using Lab3_DataAnalysis.Computing.Models;
using Lab3_DataAnalysis.DataSource.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3_DataAnalysis.Computing.Computing
{
    public class IndependentCriteriaEqualityComputing : CriteriaEqualityComputing
    {
        private const double Alpha = 0.05;
        public IndependentCriteriaEqualityComputing(VariationalSeries firstDataSource, VariationalSeries secondDataSource)
        {
            FirstDataSource = firstDataSource;
            SecondDataSource = secondDataSource;
        }
        public override CriteriaResult ForDispersion()
        {
            var quantileComputing = new SeriesQuantileComputing();
            var pointEstimationComputingForFirstDataSource = new PointEstimationCharacteristicsComputing(FirstDataSource);
            var pointEstimationComputingForSecondDataSource = new PointEstimationCharacteristicsComputing(SecondDataSource);
            var v1 = FirstDataSource.Series.Count;
            var v2 = SecondDataSource.Series.Count;
            var inf = quantileComputing.ComputePhisherQuantile(Alpha, v1, v2);
            var sup = quantileComputing.ComputePhisherQuantile(1 - Alpha, v1, v2);

            try
            {
                var firstDispersion = pointEstimationComputingForFirstDataSource.ComputeDispersion();
                var secondDispersion = pointEstimationComputingForSecondDataSource.ComputeDispersion();
                var fCriteria = firstDispersion / secondDispersion;

                return new CriteriaResult
                {
                    CriteriaStatistics = fCriteria,
                    Summary = fCriteria >= inf && fCriteria <= sup ? "Equal" : "Not Equal"
                };
            }
            catch (Exception)
            {
                return new CriteriaResult
                {
                    CriteriaStatistics = 0,
                    Summary = 0 >= inf && 0 <= sup ? "Equal" : "Not Equal"
                };
            }
        }

        public override CriteriaResult ForMean()
        {
            var quantileComputing = new SeriesQuantileComputing();
            var pointEstimationComputingForFirstDataSource = new PointEstimationCharacteristicsComputing(FirstDataSource);
            var pointEstimationComputingForSecondDataSource = new PointEstimationCharacteristicsComputing(SecondDataSource);
            var firstMean = pointEstimationComputingForFirstDataSource.ComputeAverage();
            var secondMean = pointEstimationComputingForSecondDataSource.ComputeAverage();
            var firstDispersion = pointEstimationComputingForFirstDataSource.ComputeDispersion();
            var secondDispersion = pointEstimationComputingForSecondDataSource.ComputeDispersion();
            var v = FirstDataSource.Series.Count + SecondDataSource.Series.Count - 2;

            try
            {
                if(ForDispersion().Summary == "Not Equal")
                {
                    var v1 = Math.Pow((firstDispersion / FirstDataSource.Series.Count) + (secondDispersion / SecondDataSource.Series.Count), 2) * Math.Pow(1 / (FirstDataSource.Series.Count - 1) *
                        Math.Pow(firstDispersion / FirstDataSource.Series.Count, 2) + 1 / (SecondDataSource.Series.Count - 1) * Math.Pow(secondDispersion / SecondDataSource.Series.Count, 2), -1);

                    var t1Criteria = (firstMean - secondMean) / Math.Sqrt((firstDispersion / FirstDataSource.Series.Count) + (secondDispersion / SecondDataSource.Series.Count));
                    return new CriteriaResult
                    {
                        CriteriaStatistics = t1Criteria,
                        Summary = Math.Abs(t1Criteria) <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, (int)v1) ? "Equal" : "Not Equal"
                    };
                }

                var generalDispersion = ((FirstDataSource.Series.Count - 1) * firstDispersion + (SecondDataSource.Series.Count - 1) * secondDispersion) / v;
                var tCriteria = (firstMean - secondMean) / Math.Sqrt((generalDispersion / FirstDataSource.Series.Count) + (generalDispersion / SecondDataSource.Series.Count));

                return new CriteriaResult
                {
                    CriteriaStatistics = tCriteria,
                    Summary = Math.Abs(tCriteria) <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, v) ? "Equal" : "Not Equal"
                };
            }
            catch (Exception)
            {
                return new CriteriaResult
                {
                    CriteriaStatistics = 0,
                    Summary = 0 <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, v) ? "Equal" : "Not Equal"
                };
            }
        }

        public override CriteriaResult ForRank()
        {
            var quantileComputing = new SeriesQuantileComputing();

            try
            {
                var vStatistic = FirstDataSource.Series
                                .Select(e => ComputeElementInversion(FirstDataSource.Series.IndexOf(e)))
                                .Sum();

                var ev = 0.5 * FirstDataSource.Series.Count * SecondDataSource.Series.Count;
                var dv = (double)1 / 12 * FirstDataSource.Series.Count * SecondDataSource.Series.Count * (FirstDataSource.Series.Count + SecondDataSource.Series.Count + 1);
                var uCriteria = (vStatistic - ev) / Math.Sqrt(dv);

                return new CriteriaResult
                {
                    CriteriaStatistics = uCriteria,
                    Summary = Math.Abs(uCriteria) <= quantileComputing.ComputeNormalSeriesQuantile(1 - Alpha / 2) ? "Not shifted" : "Shifted"
                };
            }
            catch (Exception)
            {
                return new CriteriaResult
                {
                    CriteriaStatistics = 0,
                    Summary = 0 <= quantileComputing.ComputeNormalSeriesQuantile(1 - Alpha / 2) ? "Not shifted" : "Shifted"
                };
            }
        }

        public double ComputeElementInversion(int index)
        {
            var sum = 0.0;

            foreach (var elem in SecondDataSource.Series)
            {
                if (FirstDataSource.Series[index] > elem)
                {
                    sum += 1;
                }
                else if (FirstDataSource.Series[index] == elem)
                {
                    sum += 0.5;
                }
            }

            return sum;
        }
    }
}
