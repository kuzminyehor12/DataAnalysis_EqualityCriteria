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
    public partial class IndependentForm : Form
    {
        private const string Format = "F5";
        private IndependentCriteriaEqualityComputing _computing;
        private VariationalSeries FirstDataSource { get; set; }
        private VariationalSeries SecondDataSource { get; set; }
        public IndependentForm()
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

                    this.textBox3.Text = pointEstimationForFirstDataSource.ComputeAverage().ToString(Format);
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

                    this.textBox8.Text = pointEstimationForFirstSecondSource.ComputeAverage().ToString(Format);
                    this.textBox7.Text = pointEstimationForFirstSecondSource.ComputeDispersion().ToString(Format);
                    this.textBox6.Text = pointEstimationForFirstSecondSource.ComputeStandartDeviation().ToString(Format);
                }));
            });
        }

        private void FillCriterias()
        {
            textBox12.Text = _computing.ForMean().ToString(Format);
            textBox11.Text = _computing.ForDispersion().ToString(Format);
            textBox10.Text = _computing.ForRank().ToString(Format);
        }
    }
}
