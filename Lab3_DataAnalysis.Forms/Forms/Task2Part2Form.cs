using DataAnalysis1.Computing.Computing;
using Lab3_DataAnalysis.Computing.Computing;
using Lab3_DataAnalysis.DataSource.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3_DataAnalysis.Forms.Forms
{
    public partial class Task2Part2Form : Form
    {
        private const string Format = "F5";
        private IndependentCriteriaEqualityComputing _computing;
        private VariationalSeries FirstDataSource { get; set; }
        private VariationalSeries SecondDataSource { get; set; }
        public Task2Part2Form()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                FirstDataSource = new VariationalSeries(path, true, true);
                SecondDataSource = new VariationalSeries(path, false, true);
                _computing = new IndependentCriteriaEqualityComputing(FirstDataSource, SecondDataSource);
                await UploadVariationalSeries();
                FillCriterias();
                FillCharacteristics();
            }
        }

        private async Task UploadVariationalSeries()
        {
            await Task.Run(() =>
            {
                this.dataGridView1.Invoke(new MethodInvoker(() =>
                {
                    this.dataGridView1.Rows.Clear();
                    foreach (var value in FirstDataSource.Series)
                    {
                        this.dataGridView1.Rows.Add(FirstDataSource.Series.IndexOf(value), value);
                    }

                    var pointEstimationForFirstDataSource = new PointEstimationCharacteristicsComputing(FirstDataSource);
                    var intervalForFirstDataSource = new IntervalEstimationCharacteristicsComputing(FirstDataSource);

                    this.textBox3.Text = pointEstimationForFirstDataSource.ComputeAverage().ToString(Format) + ", " + TupleToInterval(intervalForFirstDataSource.ComputeIntervalForAverage());
                    this.textBox4.Text = pointEstimationForFirstDataSource.ComputeDispersion().ToString(Format);
                    this.textBox5.Text = pointEstimationForFirstDataSource.ComputeStandartDeviation().ToString(Format);
                }));

                this.dataGridView2.Invoke(new MethodInvoker(() =>
                {
                    this.dataGridView2.Rows.Clear();
                    foreach (var value in SecondDataSource.Series)
                    {
                        this.dataGridView2.Rows.Add(SecondDataSource.Series.IndexOf(value), value);
                    }

                    var pointEstimationForSecondSource = new PointEstimationCharacteristicsComputing(SecondDataSource);
                    var intervalForSecondDataSource = new IntervalEstimationCharacteristicsComputing(SecondDataSource);

                    this.textBox8.Text = pointEstimationForSecondSource.ComputeAverage().ToString(Format) + ", " + TupleToInterval(intervalForSecondDataSource.ComputeIntervalForAverage());
                    this.textBox7.Text = pointEstimationForSecondSource.ComputeDispersion().ToString(Format);
                    this.textBox6.Text = pointEstimationForSecondSource.ComputeStandartDeviation().ToString(Format);
                }));
            });
        }

        private void FillCriterias()
        {
            textBox12.Text = _computing.ForMean().ToString(Format);
            textBox11.Text = _computing.ForDispersion().ToString(Format);
            textBox10.Text = _computing.ForRank().ToString(Format);
        }

        private void FillCharacteristics()
        {
            dataGridView3.Rows.Clear();
            var pointEstimationForFirstDataSource = new PointEstimationCharacteristicsComputing(FirstDataSource);
            var intervalForFirstDataSource = new IntervalEstimationCharacteristicsComputing(FirstDataSource);

            var pointEstimationForSecondSource = new PointEstimationCharacteristicsComputing(SecondDataSource);
            var intervalForSecondDataSource = new IntervalEstimationCharacteristicsComputing(SecondDataSource);

            dataGridView3.Rows.Add("Skewness", "North Hardness", pointEstimationForFirstDataSource.ComputeSkewnessCoefficient().ToString(Format),
                TupleToInterval(intervalForFirstDataSource.ComputeIntervalForSkewness()));
            dataGridView3.Rows.Add("Skewness", "South Hardness", pointEstimationForSecondSource.ComputeSkewnessCoefficient().ToString(Format),
                TupleToInterval(intervalForSecondDataSource.ComputeIntervalForKurtosis()));

            dataGridView3.Rows.Add("Kurtosis", "North Hardness", pointEstimationForFirstDataSource.ComputeKurtosisCoefficient().ToString(Format),
               TupleToInterval(intervalForFirstDataSource.ComputeIntervalForSkewness()));
            dataGridView3.Rows.Add("Kurtosis", "South Hardness", pointEstimationForSecondSource.ComputeKurtosisCoefficient().ToString(Format),
                TupleToInterval(intervalForSecondDataSource.ComputeIntervalForKurtosis()));

            var normalityResultForFirstDataSource = _computing.ForNormality(FirstDataSource);
            var normalityResultForSecondDataSource = _computing.ForNormality(SecondDataSource);

            dataGridView3.Rows.Add("Normality", "North Hardness", normalityResultForFirstDataSource.ToString(Format), "-");
            dataGridView3.Rows.Add("Normality", "South Hardness", normalityResultForSecondDataSource.ToString(Format), "-");

            if (normalityResultForFirstDataSource.Summary == "Not Equal")
            {
                dataGridView3.Rows.Add("Median", "North Hardness", pointEstimationForFirstDataSource.ComputeMedian(),
                    TupleToInterval(intervalForFirstDataSource.ComputeIntervalForMedian()));
            }

            if (normalityResultForSecondDataSource.Summary == "Not Equal")
            {
                dataGridView3.Rows.Add("Median", "South Hardness", pointEstimationForSecondSource.ComputeMedian(),
                    TupleToInterval(intervalForSecondDataSource.ComputeIntervalForMedian()));
            }
        }

        private string TupleToInterval(Tuple<double, double> tuple)
        {
            return $"[ {tuple.Item1.ToString(Format)} : {tuple.Item2.ToString(Format)} ]";
        }
    }
}
