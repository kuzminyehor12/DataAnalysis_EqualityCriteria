using Lab3_DataAnalysis.Forms.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Lab3_DataAnalysis.Forms.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SetTabPages();
        }

        public void SetTabPages()
        {
            DependentForm dependentForm = new DependentForm();
            dependentForm.AddToTabPage(tabControl1, 0);

            IndependentForm independentForm = new IndependentForm();
            independentForm.AddToTabPage(tabControl1, 1);

            Task1Form task1Form = new Task1Form();
            task1Form.AddToTabPage(tabControl1, 2);

            Task2Form task2Form = new Task2Form();
            task2Form.AddToTabPage(tabControl1, 3);

            Task2Part2Form task2part2Form = new Task2Part2Form();
            task2part2Form.AddToTabPage(tabControl1, 4);

            Task3Form task3Form = new Task3Form();
            task3Form.AddToTabPage(tabControl1, 5);

            Task4Form task4Form = new Task4Form();
            task4Form.AddToTabPage(tabControl1, 6);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }
    }
}
