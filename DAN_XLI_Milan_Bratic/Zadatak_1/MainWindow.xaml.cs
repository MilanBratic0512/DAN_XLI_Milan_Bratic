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
        string numberOfCopies;
        string text { get; set; }
        private int CurrentProgress;
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

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            progressBar.Value = e.ProgressPercentage;
            label1.Content = e.ProgressPercentage.ToString() + "%";
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int copies = int.Parse(numberOfCopies);
            int sum = 0;
            for (int i = 0; i < copies; i++)
            {
                Thread.Sleep(1000);
                CurrentProgress = 100 / copies;

                sum += CurrentProgress;
                bw.ReportProgress(sum);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    bw.ReportProgress(0);
                    return;
                }
                string path = "../../" + i + "." + DateTime.Now.Day + "_" + DateTime.Now.Month +
                    "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt";

                File.WriteAllText(path, text);
            }
            e.Result = CurrentProgress;
        }
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
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            text = Text.Text;
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy)
            {
                bw.CancelAsync();
            }
        }
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
    }
}
