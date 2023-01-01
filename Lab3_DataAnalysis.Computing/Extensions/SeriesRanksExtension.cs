using Lab3_DataAnalysis.Computing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Lab3_DataAnalysis.Computing.Extensions
{
    public static class SeriesRanksExtension
    {
        public static List<SignRankValue> CountRanks(this List<SignRankValue> signRankValues)
        {
            for (int i = 0; i < signRankValues.Count(); i++)
            {
                signRankValues[i].Rank = i + 1;
            }

            var resultList = new List<SignRankValue>();
            var repetableValues = new List<SignRankValue>();
            var rankDivider = 1;
            var rankSum = 0.0;

            foreach(var value in signRankValues)
            {
                if (!resultList.Select(e => e.Value).Contains(value.Value))
                {
                    resultList.Add(value);
                }
                else
                {
                    rankSum += (double)value.Rank;
                    rankDivider++;
                    repetableValues.Add(new SignRankValue { 
                        Value = value.Value,
                        Rank = rankSum / rankDivider,
                        Positive = value.Positive
                    });
                    rankSum = 0;
                }
            }

            foreach (var criteria in repetableValues
                                .OrderByDescending(e => e.Rank)
                                .Distinct(new SignRankCriteriaComparer()))
            {
                resultList.First(e => e.Value == criteria.Value).Rank = criteria.Rank;
            }

            return resultList;
    }
}

    public class SignRankCriteriaComparer : IEqualityComparer<SignRankValue>
    {
        public bool Equals([AllowNull] SignRankValue x, [AllowNull] SignRankValue y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode([DisallowNull] SignRankValue obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
