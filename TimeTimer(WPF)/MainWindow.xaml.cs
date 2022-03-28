using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Hardcodet.Wpf.TaskbarNotification;
using System.Reflection;

//using System.Xaml;

namespace TimeTimer_WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<StackPanel> alarmObjects = new List<StackPanel>();
        List<AlarmData> alarmDatas = new List<AlarmData>();
        AlarmData selectedAlarmdata;
        StackPanel selectedAlarmPanel;
        Thickness cp, cp2 = new Thickness(0, 0, 570, 0);
        DispatcherTimer dT = new DispatcherTimer();
        DispatcherTimer clickdT = new DispatcherTimer();
        AlarmSet asw = new AlarmSet();
        AlarmData alData = new AlarmData();
        Stopwatch sw = new Stopwatch();
        TaskbarIcon tbi = new TaskbarIcon();
        System.Windows.Forms.NotifyIcon ni;
        int selectedIndex;
        int dClickspeed;
        int dClickCounter = 0;
        bool alarmSelected = false;
        bool isEditMode = false;
        bool dClicked = false;
        public MainWindow()
        {
            InitializeComponent();
            alarmDatas = DeSerializeAlObjs();
            RefreshAllAlarmObject();
            cp = Menupanel.Margin;
            cp2 = new Thickness(0, 0, 570, 0);
            Dtimer_start();
            //ShowAlarms();
            dClickspeed = 3;
            ni = new System.Windows.Forms.NotifyIcon();
            try
            {
                ni.Icon = new System.Drawing.Icon("alarmicon.ico");
                ni.Visible = true;
                ni.DoubleClick += delegate (object sender, EventArgs e)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
                ni.Text = "TimeTimer";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void TestFunction(string title)
        {
            new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("알람")
            .AddText(title)
            .Show();
        }
        public void addAlarmObjectInList(AlarmData alData, bool isEdit)
        {
            Rectangle[] dowRect = new Rectangle[7];
            dowRect[0] = OriginPanel_WOD_Mon;
            dowRect[1] = OriginPanel_WOD_Tue;
            dowRect[2] = OriginPanel_WOD_Wed;
            dowRect[3] = OriginPanel_WOD_Thu;
            dowRect[4] = OriginPanel_WOD_Fri;
            dowRect[5] = OriginPanel_WOD_Sat;
            dowRect[6] = OriginPanel_WOD_Sun;

            string am_pm = "";
            int hour = alData.hour;
            if (alData.hour > 12)
            {
                am_pm = "오후";
                hour -= 12;
            }
            else
            {
                am_pm = "오전";
            }
            if (alData.activated)
            {
                OriginPanel_OF.Content = "ON";
            }
            else
            {
                OriginPanel_OF.Content = "OFF";
            }
            for (int i = 0; i < 7; i++)
            {
                SolidColorBrush scbtemp = (SolidColorBrush)dowRect[i].Fill;
                dowRect[i].Fill = new SolidColorBrush(Color.FromArgb(5, scbtemp.Color.R, scbtemp.Color.G, scbtemp.Color.B));
            }
            if (alData.repeatCycle == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    SolidColorBrush scbtemp = (SolidColorBrush)dowRect[i].Fill;
                    dowRect[i].Fill = new SolidColorBrush(Color.FromArgb(125, scbtemp.Color.R, scbtemp.Color.G, scbtemp.Color.B));
                }
            }
            else if (alData.repeatCycle == 2)
            {
                for (int i = 0; i < 5; i++)
                {
                    SolidColorBrush scbtemp = (SolidColorBrush)dowRect[i].Fill;
                    dowRect[i].Fill = new SolidColorBrush(Color.FromArgb(125, scbtemp.Color.R, scbtemp.Color.G, scbtemp.Color.B));
                }
            }
            else if (alData.repeatCycle == 3)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (alData.repeatDOW[i])
                    {
                        SolidColorBrush scbtemp = (SolidColorBrush)dowRect[i].Fill;
                        dowRect[i].Fill = new SolidColorBrush(Color.FromArgb(125, scbtemp.Color.R, scbtemp.Color.G, scbtemp.Color.B));
                    }
                }
            }
            originPanel_ampm.Content = am_pm;
            OriginPanel_time.Content = (hour.ToString() + " : " + alData.minute.ToString());
            OriginPanel_Label.Content = alData.alarmTitle;
            //alData.activated = true;
            if (alData.repeatCycle == 0)
                OriginPanel_Wod_oneImage.Visibility = Visibility.Visible;
            else
                OriginPanel_Wod_oneImage.Visibility = Visibility.Hidden;

            alarmObjects.Add(CloneXaml<StackPanel>(Originpanel));
            if (!isEdit)
            {
                alarmDatas.Add(alData);
            }
            RefreshAlarms();
        }
        public void RefreshAllAlarmObject()
        {

            for (int i = 0; i < alarmObjects.Count; i++)
            {
                AlarmListPanel.Children.Remove(alarmObjects[i]);
            }
            alarmObjects.Clear();
            for (int i = 0; i < alarmDatas.Count; i++)
            {
                addAlarmObjectInList(alarmDatas[i], true);
            }
            alarmSelected = false;
        }
        public void ShowAlarms()
        {
            for (int i = 0; i < alarmObjects.Count; i++)
            {
                alarmObjects[i].Visibility = Visibility.Visible;
                alarmObjects[i].Name = "Alarm" + i;
                AlarmListPanel.Children.Add(alarmObjects[i]);
                // AddHandler(alarmObjects[i].MouseLeftButtonDown,Originpanel_MouseLeftButtonDown)
            }
            for (int i = 0; i < alarmObjects.Count; i++)
            {
                alarmObjects[i].PreviewMouseLeftButtonDown += Originpanel_MouseLeftButtonDown;
            }
        }
        public void RefreshAlarms()
        {
            for (int i = 0; i < alarmObjects.Count; i++)
            {
                AlarmListPanel.Children.Remove(alarmObjects[i]);
            }

            ShowAlarms();
        }
        public T CloneXaml<T>(T source) //오브젝트의 xaml을 복사하는 코드(하위 컨트롤까지 모두)
        {
            string objXaml = XamlWriter.Save(source);
            StringReader sr = new StringReader(objXaml);
            XmlReader xr = XmlReader.Create(sr);
            T t = (T)XamlReader.Load(xr);
            return t;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            TimeLabel.Content = System.DateTime.Now.ToString("tt h:mm:ss");
            dateLabel.Content = System.DateTime.Now.ToString("yyyy/MM/dd일 (dddd)");
            Chk_alarms_activate();
        }
        void ClickdT_Tick(object sender, EventArgs e)
        {

            if (dClickCounter > 0)
            {
                dClickCounter--;
            }
        }
        private int GetDateTime()
        {
            int dow;

            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    dow = 0;
                    break;
                case DayOfWeek.Tuesday:
                    dow = 1;
                    break;
                case DayOfWeek.Wednesday:
                    dow = 2;
                    break;
                case DayOfWeek.Thursday:
                    dow = 3;
                    break;
                case DayOfWeek.Friday:
                    dow = 4;
                    break;
                case DayOfWeek.Saturday:
                    dow = 5;
                    break;
                case DayOfWeek.Sunday:
                    dow = 6;
                    break;
                default:
                    dow = 10;
                    break;
            }

            return dow;
        }
        private void GreyBackground_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SineEase easingFunc = new SineEase();
            easingFunc.EasingMode = EasingMode.EaseOut;
            this.Menupanel.RenderTransform = new TranslateTransform();
            this.Menupanel.RenderTransformOrigin = new Point(0, 0);

            TransformAnimation menupanelanim = new TransformAnimation(Menupanel);

            menupanelanim.Storyboard.FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd;
            menupanelanim.animation.EasingFunction = easingFunc;
            menupanelanim.From = 230;
            menupanelanim.To = 0;
            menupanelanim.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            menupanelanim.Begin();



            OpacityAnimation greyanimation = new OpacityAnimation(this.GreyBackground);
            greyanimation.From = 0.2;
            greyanimation.To = 0;
            greyanimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            greyanimation.Storyboard.FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd;

            greyanimation.Begin();
            GreyBackground.IsHitTestVisible = false;
            //Menupanel.Margin = cp;

        }
        private void hamburgerBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SineEase easingFunc = new SineEase();
            easingFunc.EasingMode = EasingMode.EaseOut;
            this.Menupanel.RenderTransform = new TranslateTransform();
            this.Menupanel.RenderTransformOrigin = new Point(0, 0);

            TransformAnimation menupanelanim = new TransformAnimation(Menupanel);

            menupanelanim.Storyboard.FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd;
            menupanelanim.animation.EasingFunction = easingFunc;
            menupanelanim.From = 0;
            menupanelanim.To = 230;
            menupanelanim.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            menupanelanim.Begin();

            OpacityAnimation greyanimation = new OpacityAnimation(this.GreyBackground);
            greyanimation.From = 0;
            greyanimation.To = 0.2;
            greyanimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            greyanimation.Storyboard.FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd;

            greyanimation.Begin();

            GreyBackground.Visibility = Visibility.Visible;
            GreyBackground.IsHitTestVisible = true;
            //Menupanel.Margin = cp2;
        }
        private void Dtimer_start()
        {
            dT.Tick += timer_Tick;
            dT.Interval = TimeSpan.FromMilliseconds(100);
            dT.Start();
            clickdT.Tick += ClickdT_Tick;
            clickdT.Interval = TimeSpan.FromMilliseconds(100);
            clickdT.Start();
        }
        private void Current_time_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            movepanel(1);
            GreyBackground_MouseLeftButtonDown(sender, e);
        }
        private void Current_time_Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            movepanel(2);
            GreyBackground_MouseLeftButtonDown(sender, e);
        }
        private void Current_time_Copy1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            movepanel(3);
            GreyBackground_MouseLeftButtonDown(sender, e);
        }
        private void Add_alarm_btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedAlarmdata != null)
            {
                if (dClicked)
                {
                    isEditMode = true;
                    asw = new AlarmSet(selectedAlarmdata, isEditMode);
                    dClicked = false;
                }
                else
                {
                    isEditMode = false;
                    AlarmData tempad = new AlarmData();
                    tempad.hour = DateTime.Now.Hour;
                    tempad.minute = DateTime.Now.Minute;
                    asw = new AlarmSet(tempad, isEditMode);
                }
            }
            else
            {
                isEditMode = false;
                AlarmData tempad = new AlarmData();
                tempad.hour = DateTime.Now.Hour;
                tempad.minute = DateTime.Now.Minute;
                asw = new AlarmSet(tempad, isEditMode);
            }
            asw.Closed += AswClosed;
            asw.ShowDialog();
            dClicked = false;
        }
        private void AswClosed(object sender, EventArgs e)
        {
            if (asw.alarmSetted)
            {
                alData.recent_activated_month = 0;
                alData.recent_activated_month = 0;
                if (isEditMode)
                {
                    alData = asw.GetAlaraData();
                    alarmDatas[selectedIndex] = alData;
                    RefreshAllAlarmObject();
                    alarmSelected = false;
                }
                else
                {
                    alData = asw.GetAlaraData();
                    addAlarmObjectInList(alData, false);
                }
            }

        }
        private void movepanel(int page)
        {
            switch (page)
            {
                case 1:
                    TimePanel.Visibility = Visibility.Visible;
                    Alarm_Panel.Visibility = Visibility.Hidden;
                    Setting_Panel.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    TimePanel.Visibility = Visibility.Hidden;
                    Alarm_Panel.Visibility = Visibility.Visible;
                    Setting_Panel.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    TimePanel.Visibility = Visibility.Hidden;
                    Alarm_Panel.Visibility = Visibility.Hidden;
                    Setting_Panel.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void testBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RefreshAlarms();
        }
        public void Create_alarmPanel(AlarmData data)
        {

        }
        private void Originpanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel temp = (StackPanel)sender;
            if (selectedAlarmPanel == temp)
            {
                if (dClickCounter > 0 && dClickCounter < dClickspeed - 1)//0.1초의 틈을 주어 중복 클릭 이벤트를 flush.(중복클릭은 시간차가 아닌 연속적이기에 0.1초만 줘도 전부 무시가능하다)
                {
                    if (!dClicked)
                    {
                        dClicked = true;
                        Add_alarm_btn_MouseLeftButtonDown(sender, e);
                        dClickCounter = 0;
                    }
                }
                else
                {
                    dClickCounter = dClickspeed;
                    dClicked = false;
                }
            }
            else
            {
                dClickCounter = dClickspeed;
                selectedAlarmPanel = temp;
                dClicked = false;
            }
            for (int i = 0; i < alarmObjects.Count; i++)
            {
                if (temp == alarmObjects[i])
                {
                    selectedAlarmdata = alarmDatas[i];
                    selectedIndex = i;
                    alarmSelected = true;
                }
                alarmObjects[i].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            temp.Background = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0));
        }
        private void OnOriginDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void SerializeAlObjs(List<AlarmData> alObjList)
        {
            Stream dataStream = new FileStream("Alarmdata.xml", FileMode.Create);
            using (StreamWriter wr = new StreamWriter(dataStream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<AlarmData>));
                serializer.Serialize(wr, alObjList);
            }
        }
        private List<AlarmData> DeSerializeAlObjs()
        {
            List<AlarmData> temp = new List<AlarmData>();
            if (File.Exists("Alarmdata.xml"))
            {
                Stream dataStream = new FileStream("Alarmdata.xml", FileMode.Open);
                using (var reader = new StreamReader(dataStream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<AlarmData>));
                    temp = (List<AlarmData>)serializer.Deserialize(reader);
                }
            }

            return temp;
        }
        void Cmd_manager(string command)
        {
            ProcessStartInfo cmd = new ProcessStartInfo();
            Process process = new Process();

            cmd.FileName = @"cmd";
            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            cmd.RedirectStandardInput = true;
            cmd.RedirectStandardOutput = true;
            cmd.RedirectStandardError = true;

            process.EnableRaisingEvents = false;
            process.StartInfo = cmd;
            process.Start();
            process.StandardInput.Write(command + Environment.NewLine);
            process.StandardInput.Close();

            string tempVal = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            Console.WriteLine(tempVal);
        }
        void Chk_alarms_activate()
        {
            var hour = System.DateTime.Now.Hour;
            var minute = System.DateTime.Now.Minute;
            var second = System.DateTime.Now.Second;

            for (int i = 0; i < alarmDatas.Count; i++)
            {
                if (alarmDatas[i].hour == hour && alarmDatas[i].minute == minute)
                {
                    if (alarmDatas[i].repeatDOW[GetDateTime()] == true || alarmDatas[i].repeatCycle == 0)
                    {
                        if (alarmDatas[i].activated == true)
                        {
                            if (alarmDatas[i].recent_activated_month != DateTime.Now.Month && alarmDatas[i].recent_activated_day != DateTime.Now.Day)
                            {
                                alarmDatas[i].recent_activated_month = DateTime.Now.Month;
                                alarmDatas[i].recent_activated_day = DateTime.Now.Day;
                                TestFunction(alarmDatas[i].alarmTitle);
                                if (alarmDatas[i].repeatCycle == 0)
                                {
                                    alarmDatas[i].activated = false;
                                }
                                RefreshAllAlarmObject();
                                if (alarmDatas[i].command != "")
                                {
                                    if (MessageBox.Show(alarmDatas[i].command + "를 실행합니다.\n계속하시겠습니까?", "알람 커맨드 실행", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                    {
                                        Cmd_manager(alarmDatas[i].command);
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        private void Notibtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TestFunction("example");
        }

        private void cmdtest(object sender, RoutedEventArgs e)
        {
            Cmd_manager("taskmgr");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SerializeAlObjs(alarmDatas);
            Environment.Exit(0);
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (alarmSelected)
            {
                alarmDatas.RemoveAt(selectedIndex);
                RefreshAllAlarmObject();
            }
        }

        private void Label_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                key.DeleteValue(curAssembly.GetName().Name);
            }
            catch
            {
                Console.WriteLine("ERRROR on Startup");
            }
            finally
            {
                Console.WriteLine("StartupOK");
            }
        }

        public async Task Animate()
        {

        }
    }


}
