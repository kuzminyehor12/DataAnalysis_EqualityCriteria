using DataAnalysis1.Computing;
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
    public class DependentCriteriaEqualityComputing : CriteriaEqualityComputing
    {
        public const double Alpha = 0.05;
        public DependentCriteriaEqualityComputing(VariationalSeries firstDataSource, VariationalSeries secondDataSource)
        {
            FirstDataSource = firstDataSource;
            SecondDataSource = secondDataSource;
        }

        public override CriteriaResult ForDispersion()
        {
            var pointEstimationComputingForFirstDataSource = new PointEstimationCharacteristicsComputing(FirstDataSource);
            var pointEstimationComputingForSecondDataSource = new PointEstimationCharacteristicsComputing(SecondDataSource);
            var quantileComputing = new SeriesQuantileComputing();
            var firstDataSourceDispersion = pointEstimationComputingForFirstDataSource.ComputeDispersion();
            var secondDataSourceDispersion = pointEstimationComputingForSecondDataSource.ComputeDispersion();
            var isFirstDispersionMore = firstDataSourceDispersion >= secondDataSourceDispersion;

            try
            {
                if (firstDataSourceDispersion == 0 || secondDataSourceDispersion == 0)
                {
                    return new CriteriaResult
                    {
                        CriteriaStatistics = 0,
                        Summary = 0 <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, FirstDataSource.Series.Count - 1) ? "Equal" : "Not Equal"
                    };
                }

                var fCriteria = isFirstDispersionMore ?
                        firstDataSourceDispersion / secondDataSourceDispersion : secondDataSourceDispersion / firstDataSourceDispersion;

                return new CriteriaResult
                {
                    CriteriaStatistics = fCriteria,
                    Summary = fCriteria <= quantileComputing.ComputePhisherQuantile(1 - Alpha,
                    isFirstDispersionMore ? FirstDataSource.Series.Count - 1 : SecondDataSource.Series.Count - 1,
                     isFirstDispersionMore ? SecondDataSource.Series.Count - 1 : FirstDataSource.Series.Count - 1)
                    ? "Equal" : "Not Equal"
                };
            }
            catch (Exception)
            {
                
                return new CriteriaResult
                {
                    CriteriaStatistics = 0,
                    Summary = 0 <= quantileComputing.ComputePhisherQuantile(1 - Alpha,
                    isFirstDispersionMore ? FirstDataSource.Series.Count - 1 : SecondDataSource.Series.Count - 1,
                     isFirstDispersionMore ? SecondDataSource.Series.Count - 1 : FirstDataSource.Series.Count - 1) ? "Equal" : "Not Equal"
                };
            }
        }

        public override CriteriaResult ForMean()
        {
            var quantileComputing = new SeriesQuantileComputing();
            var pointEstimationComputing = new PointEstimationCharacteristicsComputing(ComputeResultSeries());

            try
            {
                if (pointEstimationComputing.ComputeStandartDeviation() == 0)
                {
                    return new CriteriaResult
                    {
                        CriteriaStatistics = 0,
                        Summary = 0 <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, FirstDataSource.Series.Count - 1) ? "Equal" : "Not Equal"
                    };
                }

                var tCriteria = pointEstimationComputing.ComputeAverage() * Math.Sqrt(FirstDataSource.Series.Count) / pointEstimationComputing.ComputeStandartDeviation();

                return new CriteriaResult
                {
                    CriteriaStatistics = tCriteria,
                    Summary = Math.Abs(tCriteria) <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, FirstDataSource.Series.Count - 1) ? "Equal" : "Not Equal"
                };
            }
            catch (Exception)
            {
                return new CriteriaResult
                {
                    CriteriaStatistics = 0,
                    Summary = 0 <= quantileComputing.ComputeStudentQuantile(1 - Alpha / 2, FirstDataSource.Series.Count - 1) ? "Equal" : "Not Equal"
                };
            }
        }

        public override CriteriaResult ForRank()
        {
            var quantileComputing = new SeriesQuantileComputing();
            double rankSum = 0;

            try
            {
                var result = ComputeRankingCriteria()
                            .OrderBy(e => e.Value)
                            .ToList()
                            .CountRanks();

                for (int i = 0; i < result.Count; i++)
                {
                    rankSum += result[i].Positive * (double)result[i].Rank;
                }

                var et = 0.25 * result.Count * (result.Count + 1);
                var dt = (double)1 / 24 * result.Count * (result.Count + 1) * (2 * result.Count + 1);

                if(dt == 0)
                {
                    return new CriteriaResult
                    {
                        CriteriaStatistics = 0,
                        Summary = 0 <= quantileComputing.ComputeNormalSeriesQuantile(1 - Alpha / 2) ? "Not shifted" : "Shifted"
                    };
                }

                var uCriteria = (rankSum - et) / Math.Sqrt(dt);

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

        public IEnumerable<SignRankValue> ComputeRankingCriteria()
        {
            var result = new List<SignRankValue>();

            for (int i = 0; i < FirstDataSource.Series.Count; i++)
            {
                var signRankCriteria = new SignRankValue
                {
                    Rank = null,
                    Value = FirstDataSource.Series[i] - SecondDataSource.Series[i],
                    Positive = FirstDataSource.Series[i] - SecondDataSource.Series[i] > 0 ? 1 : 0
                };

                result.Add(signRankCriteria);
            }

            return result
                .Where(e => e.Value != 0)
                .Select(e => new SignRankValue
                {
                    Rank = e.Rank,
                    Value = Math.Abs(e.Value),
                    Positive = e.Positive
                });
        }

        public VariationalSeries ComputeResultSeries()
        {
            VariationalSeries result = new VariationalSeries();

            for (int i = 0; i < FirstDataSource.Series.Count; i++)
            {
                result.Series.Add(FirstDataSource.Series[i] - SecondDataSource.Series[i]);
            }

            return result;
        }
    }
}
