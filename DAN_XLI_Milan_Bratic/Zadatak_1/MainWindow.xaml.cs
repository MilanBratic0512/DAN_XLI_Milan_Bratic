using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zadatak_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static BackgroundWorker bw = new BackgroundWorker();
        //the number of copies is placed in this string, later it will be parsed in int
        string numberOfCopies;
        //text is placed in this string
        string text { get; set; }
        private double CurrentProgress;
        //regex for validation
        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public MainWindow()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            bw.DoWork += backgroundWorker_DoWork;
            bw.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            bw.ProgressChanged += ProgressChanged;
            CurrentProgress = 0;

        }
        /// <summary>
        /// a method that tracks progress
        /// </summary>
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            progressBar.Value = e.ProgressPercentage;
            label1.Content = e.ProgressPercentage.ToString() + "%";
        }
        /// <summary>
        /// do work method
        /// </summary>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //parse input
            int copies = int.Parse(numberOfCopies);
            double sum = 0;

            for (int i = 0; i < copies; i++)
            {
                Thread.Sleep(1000);
                CurrentProgress = 100 / copies;

                sum += CurrentProgress;
                bw.ReportProgress((int)sum);
                //if we click cancel
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    bw.ReportProgress(0);
                    return;
                }
                //the path in the required format is written to the file
                string path = "../../" + i + "." + DateTime.Now.Day + "_" + DateTime.Now.Month +
                    "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt";
                File.WriteAllText(path, text);
            }
            e.Result = sum;
        }
        /// <summary>
        /// run worker method
        /// </summary>
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                label1.Content = "Processing cancelled";
            }
            else if (e.Error != null)
            {
                label1.Content = e.Error.Message;
            }
            else
            {
                label1.Content = e.Result.ToString();
            }
        }
        /// <summary>
        /// method for text input
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            text = Text.Text;
        }

        /// <summary>
        /// method for number input (with validation)
        /// </summary>
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (IsTextAllowed(Copies.Text))
            {
                numberOfCopies = Copies.Text;

            }
            else
            {
                numberOfCopies = "0";
            }
               
          
        }
        /// <summary>
        /// on button click method, for print
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
        }
        /// <summary>
        /// on button click method, for cancel
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy)
            {
                bw.CancelAsync();
            }
        }
        /// <summary>
        /// regex validation
        /// </summary>
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
    }
}
