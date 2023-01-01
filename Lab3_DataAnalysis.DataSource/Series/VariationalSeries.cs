using DataAnalysis1.DataSource;
using Lab3_DataAnalysis.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab3_DataAnalysis.DataSource.Series
{
    public class VariationalSeries : BaseSeries<double>
    {
        public bool IsFirst { get; set; }
        public bool IsIndependent { get; set; }
        public VariationalSeries()
        {
            Series = new List<double>();
        }
        public VariationalSeries(string path, bool isFirst, bool isIndependent = false)
        {
            Series = new List<double>();
            IsFirst = isFirst;
            IsIndependent = isIndependent;
            InitSeries(path);
        }

        public override void InitSeries(string path)
        {
            if (!IsIndependent)
            {
                using (TextReader reader = File.OpenText(path))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split(' ', '\r', '\n');

                    var findingStrings = bits.Where(b => !string.IsNullOrEmpty(b)).ToArray();

                    for (int i = 0; i < findingStrings.Length; i++)
                    {
                        double res = 0;

                        if ((IsFirst ? (i % 2 == 0) : (i % 2 != 0)) && double.TryParse(findingStrings[i], out res))
                        {
                            Series.Add(res);
                        }
                    }
                }

                return;
            }

            using (TextReader reader = File.OpenText(path))
            {
                string text = reader.ReadToEnd();
                string[] bits = text.Split(' ', '\r', '\n');

                var findingStrings = bits.Where(b => !string.IsNullOrEmpty(b)).ToArray();
                var firstDataSource = findingStrings
                                        .TakeWhile(s => s != "1")
                                        .SkipLast(1)
                                        .Where(s => s != "0")
                                        .ToArray();

                var secondDataSource = findingStrings
                                        .Where(s => s != "0" && s != "1")
                                        .Skip(firstDataSource.Length)
                                        .ToArray();

                if (IsFirst)
                {
                    for (int i = 0; i < firstDataSource.Length; i++)
                    {
                        double res = 0;

                        if (double.TryParse(firstDataSource[i], out res))
                        {
                            Series.Add(res);
                        }
                    }

                    return;
                }

                for (int i = 0; i < secondDataSource.Length; i++)
                {
                    double res = 0;

                    if (double.TryParse(secondDataSource[i], out res))
                    {
                        Series.Add(res);
                    }
                }
            }
        }
    }
}
