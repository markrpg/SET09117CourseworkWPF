using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
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

namespace SET09117CourseworkWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Instances
        private VRProblem vrProblem;
        private VRSolution vrSolution;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadProblem(string filename)
        {
            try
            {
                //Create the problem instance
                vrProblem = new VRProblem(filename);
                //Create the solution instance with the problem
                vrSolution = new VRSolution(vrProblem);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void solveProblem(string solutionType)
        {
            string workingdir = Directory.GetCurrentDirectory() + "\\";
            List<double> averages;
            ComboBoxItem cbItem;
            cbItem = (ComboBoxItem) cbTimesToRun.SelectedItem;
           double timesToRun = double.Parse(cbItem.Content.ToString());

            switch (solutionType)
            {
                case "author":
                    averages = new List<double>();
                    for (int i = 0; i <timesToRun; i++)
                    {
                        var sw = Stopwatch.StartNew();
                        vrSolution.mySolver();
                        sw.Stop();
                        vrSolution.resetCustomersVisited();
                        averages.Add(Math.Round(sw.Elapsed.TotalMilliseconds, 3));
                        lblTimeTakenAUTH.Content = String.Format("{0}", Math.Round(sw.Elapsed.TotalMilliseconds, 3));
                        sw.Reset();
                    }
                    AverageAuthTime.Content = Math.Round(averages.Average(num => num),3);;
                    vrSolution.writeBufferSVG("svgAuthBuffer.svg", "svgAuthImage.bmp");
                    lblCostOutputAUTH.Content = vrSolution.solnCost();
                    lblRoutesAUTH1.Content = vrSolution.soln.Count;
                    //Set images
                    BitmapImage _image = new BitmapImage();
                    _image.BeginInit();
                    _image.CacheOption = BitmapCacheOption.None;
                    _image.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                    _image.CacheOption = BitmapCacheOption.OnLoad;
                    _image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    _image.UriSource = new Uri(workingdir + "svgAuthImage.bmp", UriKind.Absolute);
                    _image.EndInit();
                    vrpAuthorsSolution.Source = _image;
                    //Write out CSV file containing solution
                    vrSolution.writeOut("MySolution.csv");
                    //Write out SVG files
                    vrSolution.writeSVG(System.IO.Path.GetFileNameWithoutExtension(vrProblem.id) + ".svg", "MyPictureSolution.svg");
                    if (!File.Exists(System.IO.Path.GetFileName(vrProblem.id)))
                    {
                        //Copy file
                        File.Copy(vrProblem.id, System.IO.Path.GetFileName(vrProblem.id), true);
                    }
                    //Verify Solution
                    verifySolution();
                    break;
                case "clarke":
                    //Implement clarke
                    break;
                case "oneroute":
                    averages = new List<double>();
                    for (int i = 0; i < timesToRun; i++)
                    {
                        var sww = Stopwatch.StartNew();
                        vrSolution.oneRoutePerCustomerSolution();
                        sww.Stop();
                        averages.Add(Math.Round(sww.Elapsed.TotalMilliseconds, 3));
                        lblTimeTakenORIG.Content = String.Format("{0}", Math.Round(sww.Elapsed.TotalMilliseconds, 3));
                        sww.Reset();
                    }
                    averageOrigTime.Content = Math.Round(averages.Average(num => num), 3);
                    vrSolution.writeBufferSVG("svgORIGBuffer.svg", "svgORIGImage.bmp");
                    lblCostOutputORIG.Content = vrSolution.solnCost();
                    lblRoutesORIG.Content = vrSolution.soln.Count;
                    //Set image
                    BitmapImage __image = new BitmapImage();
                    __image.BeginInit();
                    __image.CacheOption = BitmapCacheOption.None;
                    __image.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                    __image.CacheOption = BitmapCacheOption.OnLoad;
                    __image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    __image.UriSource = new Uri(workingdir + "svgORIGImage.bmp", UriKind.Relative);
                    __image.EndInit();
                    vrpOrigionalSolution.Source = __image;
                    break;
            }

        }

        private void lbProblems_Drop(object sender, DragEventArgs e)
        {
            foreach (var s in (string[])e.Data.GetData(DataFormats.FileDrop, false))
            {
                if (!Directory.Exists(s) && !lbProblems.Items.Contains(s) && System.IO.Path.GetExtension(s) == ".csv")
                {
                    ListBoxItem problem = new ListBoxItem();
                    problem.Content = System.IO.Path.GetFileName(s);
                    problem.ToolTip = s;
                    //Add filepath
                    lbProblems.Items.Add(problem);
                }
                else
                {
                    MessageBox.Show("Please drop a proper .csv problem file.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void btnSolveVRP_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = new ListBoxItem();

            if (lbProblems.SelectedIndex != -1)
            {
                lbi = (ListBoxItem) lbProblems.SelectedItem;
            }
            else
            {
                MessageBox.Show("Select a problem to solve from the listbox above!");
                return;
            }

            try
            {
                //Create solution for comparison using origional solutions
                switch (cbOrigionalCompare.SelectedIndex)
                {
                    case -1:
                        MessageBox.Show("Select origional comparison solution to compare to Authors.");
                        return;
                    case 0:
                        loadProblem(lbi.ToolTip.ToString());
                        solveProblem("oneroute");
                        break;
                    case 1:
                        //Implement clarke
                        break;
                }

                //Create solution with authors algorithm
                loadProblem(lbi.ToolTip.ToString());
                solveProblem("author");
            }
            catch (Exception)
            {
                MessageBox.Show("Problem solving VRP problem, check its a valid problem.");
            }
          
            //Calculate % Change for time
           double timePercentageChange = Math.Round((((double.Parse(averageOrigTime.Content.ToString()) -
                                                                         double.Parse(AverageAuthTime.Content.ToString())) /
                                                                         double.Parse(averageOrigTime.Content.ToString())) * 100));

            double costPercentageChange = Math.Round((((double.Parse(lblCostOutputORIG.Content.ToString()) -
                                                                         double.Parse(lblCostOutputAUTH.Content.ToString())) /
                                                                         double.Parse(lblCostOutputORIG.Content.ToString())) * 100));

            double routePercentageChange = Math.Round(((double.Parse(lblRoutesORIG.Content.ToString()) -
                                             double.Parse(lblRoutesAUTH1.Content.ToString()))/
                                            double.Parse(lblRoutesORIG.Content.ToString()))*100);

                lblPercentageChangeCost.Content = costPercentageChange.ToString()+"%";
                lblPercentageChangeTime.Content = timePercentageChange.ToString() +"%";
                lblPercentageChangeRoute.Content =  routePercentageChange.ToString()+"%";

        }

        private void verifySolution()
        {
            string arguments = @"/K" + "java -jar " + "Verify.jar " + System.IO.Path.GetFileName(vrProblem.id) + " " + "MySolution.csv";
            System.Diagnostics.Process clientProcess = new Process();
            clientProcess.StartInfo.FileName = "CMD";
            clientProcess.StartInfo.Arguments = arguments;
            clientProcess.Start();
        }

        private void averageOrigTime_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(averageOrigTime.Content.ToString());
        }

        private void AverageAuthTime_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(AverageAuthTime.Content.ToString());
        }

        private void lblCostOutputORIG_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(lblCostOutputORIG.Content.ToString());
        }

        private void lblCostOutputAUTH_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(lblCostOutputAUTH.Content.ToString());
        }
    }
}
