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
    public partial class Task3Form : Form
    {
        private const string Format = "F5";
        private DependentCriteriaEqualityComputing _computing;
        private VariationalSeries FirstDataSource { get; set; }
        private VariationalSeries SecondDataSource { get; set; }
        public Task3Form()
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
                FirstDataSource = new VariationalSeries(path, true);
                SecondDataSource = new VariationalSeries(path, false);
                _computing = new DependentCriteriaEqualityComputing(FirstDataSource, SecondDataSource);
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
                    var intervalForFirstSource = new IntervalEstimationCharacteristicsComputing(FirstDataSource);

                    this.textBox3.Text = pointEstimationForFirstDataSource.ComputeAverage().ToString(Format) + "," + TupleToInterval(intervalForFirstSource.ComputeIntervalForAverage());
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

                    var pointEstimationForFirstSecondSource = new PointEstimationCharacteristicsComputing(SecondDataSource);
                    var intervalForSecondSource = new IntervalEstimationCharacteristicsComputing(SecondDataSource);

                    this.textBox8.Text = pointEstimationForFirstSecondSource.ComputeAverage().ToString(Format) + "," + TupleToInterval(intervalForSecondSource.ComputeIntervalForAverage());
                    this.textBox7.Text = pointEstimationForFirstSecondSource.ComputeDispersion().ToString(Format);
                    this.textBox6.Text = pointEstimationForFirstSecondSource.ComputeStandartDeviation().ToString(Format);
                }));

                this.dataGridView3.Invoke(new MethodInvoker(() =>
                {
                    this.dataGridView3.Rows.Clear();

                    var resultSeries = _computing.ComputeResultSeries();
                    foreach (var value in resultSeries.Series)
                    {
                        this.dataGridView3.Rows.Add(_computing.ComputeResultSeries().Series.IndexOf(value), value);
                    }

                    var pointEstimationForResultSource = new PointEstimationCharacteristicsComputing(resultSeries);
                    var intervalForResultSource = new IntervalEstimationCharacteristicsComputing(resultSeries);

                    this.textBox9.Text = pointEstimationForResultSource.ComputeAverage().ToString(Format) + "," + TupleToInterval(intervalForResultSource.ComputeIntervalForAverage());
                    this.textBox2.Text = pointEstimationForResultSource.ComputeDispersion().ToString(Format);
                    this.textBox1.Text = pointEstimationForResultSource.ComputeStandartDeviation().ToString(Format);
                }));
            });
        }

        public void FillCriterias()
        {
            textBox12.Text = _computing.ForMean().ToString(Format);
            textBox11.Text = _computing.ForDispersion().ToString(Format);
            textBox10.Text = _computing.ForRank().ToString(Format);
        }

        private void FillCharacteristics()
        {
            dataGridView4.Rows.Clear();
            var resultSource = _computing.ComputeResultSeries();
            var pointEstimationForResultDataSource = new PointEstimationCharacteristicsComputing(resultSource);
            var intervalForResultDataSource = new IntervalEstimationCharacteristicsComputing(resultSource);

            dataGridView4.Rows.Add("Skewness", pointEstimationForResultDataSource.ComputeSkewnessCoefficient().ToString(Format),
                TupleToInterval(intervalForResultDataSource.ComputeIntervalForSkewness()));

            dataGridView4.Rows.Add("Kurtosis", pointEstimationForResultDataSource.ComputeKurtosisCoefficient().ToString(Format),
               TupleToInterval(intervalForResultDataSource.ComputeIntervalForKurtosis()));

            var normalityResultForResultSource = _computing.ForNormality(resultSource);

            dataGridView4.Rows.Add("Normality", normalityResultForResultSource.ToString(Format), "-");

            if (normalityResultForResultSource.Summary == "Not Equal")
            {
                dataGridView4.Rows.Add("Median", pointEstimationForResultDataSource.ComputeMedian(),
                    TupleToInterval(intervalForResultDataSource.ComputeIntervalForMedian()));
            }
        }

        private string TupleToInterval(Tuple<double, double> tuple)
        {
            return $"[ {tuple.Item1.ToString(Format)} : {tuple.Item2.ToString(Format)} ]";
        }
    }
}
