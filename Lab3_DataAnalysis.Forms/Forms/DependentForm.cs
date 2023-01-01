using DataAnalysis1.Computing;
using DataAnalysis1.Computing.Computing;
using Lab3_DataAnalysis.Computing.Computing;
using Lab3_DataAnalysis.Computing.Interfaces;
using Lab3_DataAnalysis.DataSource.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3_DataAnalysis.Forms
{
    public partial class DependentForm : Form
    {
        private const string Format = "F5";
        private DependentCriteriaEqualityComputing _computing;
        private VariationalSeries FirstDataSource { get; set; }
        private VariationalSeries SecondDataSource { get; set; }
        public DependentForm()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

                    this.textBox3.Text = pointEstimationForFirstDataSource.ComputeAverage().ToString();
                    this.textBox4.Text = pointEstimationForFirstDataSource.ComputeDispersion().ToString();
                    this.textBox5.Text = pointEstimationForFirstDataSource.ComputeStandartDeviation().ToString();
                }));

                this.dataGridView2.Invoke(new MethodInvoker(() =>
                {
                    this.dataGridView2.Rows.Clear();

                    foreach (var value in SecondDataSource.Series)
                    {
                        this.dataGridView2.Rows.Add(SecondDataSource.Series.IndexOf(value), value);
                    }

                    var pointEstimationForFirstSecondSource = new PointEstimationCharacteristicsComputing(SecondDataSource);

                    this.textBox8.Text = pointEstimationForFirstSecondSource.ComputeAverage().ToString();
                    this.textBox7.Text = pointEstimationForFirstSecondSource.ComputeDispersion().ToString();
                    this.textBox6.Text = pointEstimationForFirstSecondSource.ComputeStandartDeviation().ToString();
                }));

                this.dataGridView3.Invoke(new MethodInvoker(() =>
                {
                    this.dataGridView3.Rows.Clear();
                    foreach (var value in _computing.ComputeResultSeries().Series)
                    {
                        this.dataGridView3.Rows.Add(_computing.ComputeResultSeries().Series.IndexOf(value), value.ToString(Format));
                    }

                    var pointEstimationForResultSource = new PointEstimationCharacteristicsComputing(_computing.ComputeResultSeries());

                    this.textBox9.Text = pointEstimationForResultSource.ComputeAverage().ToString();
                    this.textBox2.Text = pointEstimationForResultSource.ComputeDispersion().ToString();
                    this.textBox1.Text = pointEstimationForResultSource.ComputeStandartDeviation().ToString();
                }));
            });
        }

        public void FillCriterias()
        {
            textBox12.Text = _computing.ForMean().ToString(Format);
            textBox11.Text = _computing.ForDispersion().ToString(Format);
            textBox10.Text = _computing.ForRank().ToString(Format);
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
