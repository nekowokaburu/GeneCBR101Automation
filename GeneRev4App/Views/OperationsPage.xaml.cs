using Plugin.FilePicker;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using OxyPlot;
//using OxyPlot.Axes;
//using OxyPlot.Xamarin.Forms;
//using OxyPlot.Series;

namespace XamarinForms.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OperationsPage : ContentPage
    {
        #region Setup
        private float newSetpoint_ = 0;
        private float oldSetpoint_ = 0;
        static System.Timers.Timer timer_;
        private Int64 secondsCount_ = 0;
        private int fileIndex_ = 0;
        private string[] fileText_ = null;
        private bool useRoastFile_ = false;
        private bool roastStarted_ = false;
        private float roastTime_ = 0;
        private String fileName_ = "";
        private int cleanLineCount_ = 0;
        private float currentTempFloat_ = 0;
        //private List<float> measuredList_ = new List<float>();
        //private PlotView plotView = new PlotView();
        //private PlotView _opv = new PlotView();
        //private List<DataPoint> filePoints_ = new List<DataPoint>();

        public OperationsPage()
        {
            InitializeComponent();

            // Periodic timer for brew time update
            timer_ = new System.Timers.Timer
            {
                Interval = 1000 // Call timer callback every second
            }; // Set up the timer
            timer_.Elapsed += new ElapsedEventHandler(TimerElapsed);

            // Backend thread to update tpc communication and display
            ThreadPool.QueueUserWorkItem(o => HandleTCP());

            //var m = new PlotModel();
            //m.PlotType = PlotType.XY;
            //m.InvalidatePlot(false);
            //m.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 25, StringFormat = "mm" });
            //m.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 250 });
            //m.ResetAllAxes();

            //var ls1 = new LineSeries();
            //ls1.MarkerType = MarkerType.Circle;
            //filePoints_ = new List<DataPoint>
            //  {
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 15)), 157),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 30)), 157),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 1)), 57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 15)), 57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 1)), 57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 15)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 1)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 15)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 31)), 1.57)
            //  };
            //ls1.ItemsSource = filePoints_;

            //m.Series.Add(ls1);

            //chart = new PlotView
            //{
            //  WidthRequest = 300,
            //  HeightRequest = 300,
            //  BackgroundColor = Color.White,
            //};
            //plotView.Model = m;
            //chart = plotView;




            //var Points = new List<DataPoint>
            //  {
            //      //DateTimeAxis.ToDouble(new DateTime(1989, 10, 3)), 8)
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 15)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 30)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 1)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 15)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 1)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 15)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 1)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 15)), 0.75),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 31)), 0.75)
            //  };
            //var Points2 = new List<DataPoint>
            //  {
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 15)), 0.9),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 30)), 0.9),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 1)), 0.9),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 15)), 0.9),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 1)), 1.0),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 15)), 1.15),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 1)), 1.0),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 15)), 0.9),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 31)), 0.9)
            //  };
            //var Points3 = new List<DataPoint>
            //  {
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 15)), 1.42),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 30)), 1.42),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 1)), 1.42),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 15)), 1.42),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 1)), 1.5),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 15)), 1.6),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 1)), 1.41),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 15)), 1.42),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 31)), 1.42)
            //  };
            //var Points4 = new List<DataPoint>
            //  {
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 15)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 10, 30)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 1)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 11, 15)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 1)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2018, 12, 15)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 1)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 15)), 1.57),
            //      new DataPoint(DateTimeAxis.ToDouble(new DateTime(2019, 1, 31)), 1.57)
            //  };


            //var m = new PlotModel();
            //m.PlotType = PlotType.XY;
            //m.InvalidatePlot(false);

            //m.Title = "hello oxy";


            //var startDate = DateTime.Now.AddMonths(-3);
            //var endDate = DateTime.Now;

            //var minValue = DateTimeAxis.ToDouble(startDate);
            //var maxValue = DateTimeAxis.ToDouble(endDate);
            //m.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "MMM/yyyy" });
            //m.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 1.6 });
            //m.ResetAllAxes();

            //var ls1 = new LineSeries();
            //var ls2 = new LineSeries();
            //var ls3 = new LineSeries();
            //var ls4 = new LineSeries();
            ////MarkerType = OxyPlot.MarkerType.Circle,
            //ls1.MarkerType = OxyPlot.MarkerType.Circle;
            //ls2.MarkerType = OxyPlot.MarkerType.Circle;
            //ls3.MarkerType = OxyPlot.MarkerType.Circle;
            //ls4.MarkerType = OxyPlot.MarkerType.Circle;
            //ls1.ItemsSource = Points;
            //ls2.ItemsSource = Points2;
            //ls3.ItemsSource = Points3;
            //ls4.ItemsSource = Points4;

            //m.Series.Add(ls1);
            //m.Series.Add(ls2);
            //m.Series.Add(ls3);
            //m.Series.Add(ls4);
            //_opv = new PlotView
            //{
            //  WidthRequest = 300,
            //  HeightRequest = 300,
            //  BackgroundColor = Color.White,

            //};
            //_opv.Model = m;
            //this.BindingContext = _opv;
        }
        #endregion

        private void HandleTCP()
        {
            while (true)
            {
                // Get current temp to display, update display on state
                var currentTemp = GetDataPerLine();
                if (currentTemp.Length != 0)
                    currentTempFloat_ = float.Parse(currentTemp, CultureInfo.InvariantCulture);

                String lblTxt = "";
                String lbl2Txt = String.Format("Manual Setpoint: {0} °C \n", newSetpoint_);
                if (fileText_ != null && useRoastFile_)
                {
                    lblTxt = "Roasting from file " + fileName_ + ".";
                    lbl2Txt = String.Format("File Setpoint: " + newSetpoint_ + " °C \n");
                }
                else if (fileText_ == null)
                    lblTxt = "No roast profile selected.";
                else
                    lblTxt = "Roasting manually. ";

                lbl2Txt += String.Format("Current roaster temp.: {0} °C \n", currentTempFloat_);
                lbl2Txt += "Diff to target: " + (currentTempFloat_ - newSetpoint_) + " °C";

                Device.BeginInvokeOnMainThread(() =>
                {
                    lbl.Text = lblTxt;
                    currTempLabel.Text = lbl2Txt;
                });

                // Overrides slider value if use roast profile is checked and a file is loaded
                if (roastStarted_)
                {
                    if (useRoastFile_ && fileText_ != null)
                    {
                        if (fileIndex_ < cleanLineCount_)
                        {
                            // Check if txt file with t0 T0 etc or csv file from roaster (0;180) etc. from custom roast profiles so I can keep both while doing a new version later on
                            if (fileText_[0].StartsWith("[t1=") && fileText_[1].StartsWith("[T1="))
                                ReadTxt();
                            else if (fileText_[0].StartsWith("999;"))
                                ReadCsv();
                        }
                        if (fileIndex_ == cleanLineCount_ && secondsCount_ > roastTime_) // Last file point read, wait for time to run out
                        {
                            // XXX Add second timer with cool time, keep display of roast time ?
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Cool(); // Roasting done, user can stop and cool again if desired
                            });
                            roastTime_ = 0;
                            fileIndex_ = 0;
                        }
                    }
                    if (oldSetpoint_ != newSetpoint_)
                    {
                        newSetpoint_ += 1;
                        SendMessage("temp" + newSetpoint_.ToString("F1", CultureInfo.InvariantCulture) + ";");
                        oldSetpoint_ = newSetpoint_;
                    }

                    // Draw chart
                    //measuredList_.Add(float.Parse(currentTemp, CultureInfo.InvariantCulture));
                    //var chartEntries = CreateChartEntries(measuredList_);
                    //chart = new PlotView();
                    //chart.Model.Axes.Add(chartEntries);
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //  plotView = chart;
                    //});
                }
                else
                {
                    if (currentTempFloat_ < 80)
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            stateLabel.Text = "Idle.";
                        });

                }
                Thread.Sleep(100);
            }
        }

        #region Chart
        //public List<DataPoint> CreateChartEntries(List<float> measured)
        //{
        //  var list = new List<DataPoint>();
        //  //for (int i = 0; i < measured.Count(); i++)
        //  //{
        //  //  var point = new DataPoint(measured[i])
        //  //  {
        //  //    Label = measured[i].ToString(),
        //  //    Color = SKColor.Parse("#5E106D")
        //  //  };
        //  //  list.Add(point);
        //  //}
        //  if(fileText_ != null && useRoastFile_)
        //    list.AddRange(FileToChartEntries()); // Add file indices
        //  return list;
        //}

        //public List<DataPoint> FileToChartEntries()
        //{
        //  var list = new List<DataPoint>();
        //  // Txt version
        //  if (fileText_[0].StartsWith("[t1=") && fileText_[1].StartsWith("[T1="))
        //  {
        //    for (int i = fileIndex_; i < cleanLineCount_; i += 2)
        //    {
        //      roastTime_ = float.Parse(Match(fileText_[i], @"(?<=\[t\d+=).*(?=\])"), CultureInfo.InvariantCulture);
        //    TimeSpan timeSpan = TimeSpan.FromSeconds(roastTime_);
        //    var temp = Match(fileText_[i + 1], @"(?<=\[T\d+=).*(?=\])");
        //    list.Add(new DataPoint(timeSpan.Minutes, float.Parse(temp)), CultureInfo.InvariantCulture);
        //  }
        //  }
        //  // Csv version
        //  else if (fileText_[fileIndex_].StartsWith("999;"))
        //  {
        //    ++fileIndex_;
        //    for(int i = fileIndex_; i < cleanLineCount_; i++)
        //    {
        //      roastTime_ = float.Parse(Match(fileText_[i], @"^.*?(?=;)"), CultureInfo.InvariantCulture);
        //      TimeSpan timeSpan = TimeSpan.FromSeconds(roastTime_);
        //      var temp = Match(fileText_[i], @"(?<=;).*");
        //      list.Add(new DataPoint(timeSpan.Minutes, float.Parse(temp)), CultureInfo.InvariantCulture);
        //    }
        //  }
        //  return list;
        //}

        #endregion

        #region Display and processing
        private void ReadTxt()
        {
            //String statusString = "Roasting profile " + fileName_;
            if (secondsCount_ > roastTime_)
            {
                roastTime_ = float.Parse(Match(fileText_[fileIndex_], @"(?<=\[t\d+=).*(?=\])"), CultureInfo.InvariantCulture);
                var temp = Match(fileText_[fileIndex_ + 1], @"(?<=\[T\d+=).*(?=\])");
                //TimeSpan timeSpan = TimeSpan.FromSeconds(roastTime_);
                //statusString += "... holding " + temp + " °C until " + timeSpan.ToString(@"mm\:ss");
                //Device.BeginInvokeOnMainThread(() =>
                //{
                //  stateLabel.Text = statusString;
                //});

                roastTime_ -= 1; // -1 loop time compensation
                newSetpoint_ = float.Parse(temp, CultureInfo.InvariantCulture);
                fileIndex_ += 2; // Advance two because time and temp are seperate lines in txt files
            }
        }
        private void ReadCsv()
        {
            //String statusString = "Roasting profile " + fileName_;
            // ignore first time 999
            if (fileText_[fileIndex_].StartsWith("999;"))
                ++fileIndex_;
            if (secondsCount_ > roastTime_)
            {
                roastTime_ = float.Parse(Match(fileText_[fileIndex_], @"^.*?(?=;)"), CultureInfo.InvariantCulture);
                var temp = Match(fileText_[fileIndex_], @"(?<=;).*");
                //TimeSpan timeSpan = TimeSpan.FromSeconds(roastTime_);
                //statusString += "... setting " + temp + " °C at timepoint " + timeSpan.ToString(@"mm\:ss");
                //Device.BeginInvokeOnMainThread(() =>
                //{
                //  stateLabel.Text = statusString;
                //});

                roastTime_ -= 1; // -1 loop time compensation
                newSetpoint_ = float.Parse(temp, CultureInfo.InvariantCulture);
                ++fileIndex_;
            }
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ++secondsCount_;
            TimeSpan time = TimeSpan.FromSeconds(secondsCount_);
            Device.BeginInvokeOnMainThread(() =>
            {
                timeLabel.Text = "Roast time: " + time.ToString(@"mm\:ss");
            });
        }

        private static string Match(string text, string expr)
        {
            MatchCollection mc = Regex.Matches(text, expr);

            if (mc[0].Success)
                return mc[0].Value;
            return "";
        }
        #endregion

        #region Buttons and button functions
        // Start Button Command
        private void StartResume_Clicked(object sender, EventArgs e)
        {
            Start();
        }
        private void Start()
        {
            if (ContinueBtn.IsVisible)
            {
                StopReturn.IsVisible = true;
                ContinueBtn.IsVisible = false;
            }
            secondsCount_ = 0;
            fileIndex_ = 0;
            roastTime_ = 0;
            timer_.Start();
            roastStarted_ = true;
            SendMessage("Start");
            stateLabel.Text = "Roasting.";
        }

        // Cool Button Command
        private void Cool_Clicked(object sender, EventArgs e)
        {
            Cool();
        }
        private void Cool()
        {
            timer_.Stop();
            roastStarted_ = false;
            SendMessage("Cool");
            stateLabel.Text = "Cooling roast to 80 °C.";
        }

        // Stop/Return Button Command
        private void StopReturn_Clicked(object sender, EventArgs e)
        {
            if (roastStarted_)
            {
                Stop();
                StopReturn.IsVisible = false;
                ContinueBtn.IsVisible = true;
            }
            else
            {
                Stop();
            }
        }

        private void Stop()
        {
            timer_.Stop();
            roastStarted_ = false;
            SendMessage("Stop");
            stateLabel.Text = "Idle.";
        }

        private void Continue_Clicked(object sender, EventArgs e)
        {
            Continue();
            StopReturn.IsVisible = true;
            ContinueBtn.IsVisible = false;
        }
        private void Continue()
        {
            roastStarted_ = true;
            timer_.Start();
            SendMessage("Start");
            stateLabel.Text = "Roasting.";
        }

        // Load user determined roast profile file
        private async void OpenFile_Clicked(object sender, EventArgs e)
        {
            var file = await CrossFilePicker.Current.PickFile();

            if (file != null)
            {
                fileText_ = System.Text.Encoding.UTF8.GetString(file.DataArray).Split('\n');
                cleanLineCount_ = fileText_.ToList().Where(x => !string.IsNullOrEmpty(x)).Count(); // Remove trailing newlines
                fileName_ = file.FileName;
            }
        }

        void OnUseFileCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            useRoastFile_ = e.Value;
            // Must set setpoint from file here if switched to file again. XXX Put in method, called 2 times!
            if (useRoastFile_)
            {
                if (fileIndex_ < cleanLineCount_)
                {
                    // Check if txt file with t0 T0 etc or csv file from roaster (0;180) etc. from custom roast profiles so I can keep both while doing a new version later on
                    if (fileText_[0].StartsWith("[t1=") && fileText_[1].StartsWith("[T1="))
                    {
                        var temp = Match(fileText_[fileIndex_ + 1], @"(?<=\[T\d+=).*(?=\])");
                        newSetpoint_ = float.Parse(temp, CultureInfo.InvariantCulture);
                    }
                }
            }

        }

        // User changes temperature, send info to roaster
        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (!useRoastFile_ || (useRoastFile_ && fileText_ == null))
                newSetpoint_ = (int)args.NewValue;
        }

        #endregion

        #region Tcp communication
        //Data Collecting from Server, read till line ends
        public String GetDataPerLine()
        {
            var client = Connection.Instance.Client;
            NetworkStream stream = client.GetStream();
            // Check to see if this NetworkStream is readable.
            String line = "";
            if (stream.CanRead && stream.DataAvailable)
            {
                StreamReader reader = new StreamReader(stream);
                line = reader.ReadLine();
            }
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(line));
        }

        // Send a message to arduino
        private void SendMessage(string s)
        {
            var client = Connection.Instance.Client;
            NetworkStream stream = client.GetStream();
            s += "\r\n"; // add line endings
            byte[] message = Encoding.ASCII.GetBytes(s);
            stream.Write(message, 0, message.Length);
        }
        #endregion

    }
}
