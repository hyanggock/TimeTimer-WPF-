using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TimeTimer_WPF_
{
    /// <summary>
    /// AlarmSet.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmSet : Window
    {
        private Color color;
        private SolidColorBrush dow_val_BtnSelCol;
        private SolidColorBrush dow_val_BtnIdleCol = new SolidColorBrush();
        private int dow_val = 0;
        private bool tw_Time = false;
        public bool alarmSetted = false;
        private bool[] dow_selected = new bool[7];
        private SolidColorBrush[] dow_originCol = new SolidColorBrush[7];
        private Label[] dowLabels = new Label[7];
        private Label[] dowoptionLabels = new Label[4];
        private AlarmData initialData = new AlarmData();

        public AlarmSet()
        {
            InitializeComponent();
            dow_originCol[0] = (SolidColorBrush)mon.Background;
            dow_originCol[1] = (SolidColorBrush)tue.Background;
            dow_originCol[2] = (SolidColorBrush)wed.Background;
            dow_originCol[3] = (SolidColorBrush)thu.Background;
            dow_originCol[4] = (SolidColorBrush)fri.Background;
            dow_originCol[5] = (SolidColorBrush)sat.Background;
            dow_originCol[6] = (SolidColorBrush)sun.Background;
            dow_val_BtnSelCol = (SolidColorBrush)(onceBtn.Background);
            dow_val_BtnIdleCol.Color = Color.FromArgb(0, 255, 255, 255);
        }
        public AlarmSet(AlarmData data,bool isEditMode)
        {
            InitializeComponent();
            dowLabels[0] = mon;
            dowLabels[1] = tue;
            dowLabels[2] = wed;
            dowLabels[3] = thu;
            dowLabels[4] = fri;
            dowLabels[5] = sat;
            dowLabels[6] = sun;

            dowoptionLabels[0] = onceBtn;
            dowoptionLabels[1] = everydayBtn;
            dowoptionLabels[2] = weekdayBtn;
            dowoptionLabels[3] = dow_SelectBtn;

            dow_originCol[0] = (SolidColorBrush)mon.Background;
            dow_originCol[1] = (SolidColorBrush)tue.Background;
            dow_originCol[2] = (SolidColorBrush)wed.Background;
            dow_originCol[3] = (SolidColorBrush)thu.Background;
            dow_originCol[4] = (SolidColorBrush)fri.Background;
            dow_originCol[5] = (SolidColorBrush)sat.Background;
            dow_originCol[6] = (SolidColorBrush)sun.Background;
            dow_val_BtnSelCol = (SolidColorBrush)(onceBtn.Background);
            dow_val_BtnIdleCol.Color = Color.FromArgb(0, 255, 255, 255);

            initialData = data;

            if(isEditMode)
            {
                alarm_Apply.Content="수정";
            }
            else
            {
                alarm_Apply.Content="추가";
            }
            if (data.hour > 12)
            {
                am_pm.Content = "오후";
                tw_Time = true;
                data.hour -= 12;
            }
            for (int i = 0; i < 7; i++)
            {
                if (data.repeatDOW[i])
                {
                    dowLabels[i].RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = MouseLeftButtonDownEvent });
                    //RoutedEventArgs newEventArgs = new RoutedEventArgs(MouseLeftButtonDownEvent);
                    //dowLabels[i].RaiseEvent(newEventArgs);
                }
            }
            dowoptionLabels[data.repeatCycle].RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = MouseLeftButtonDownEvent });
            //RoutedEventArgs eventArgs = new RoutedEventArgs(MouseLeftButtonDownEvent);
            //dowoptionLabels[data.repeatCycle].RaiseEvent(eventArgs);

            alarm_Hour.Content = data.hour.ToString();
            alarm_Minute.Content = data.minute.ToString();
            if (data.alarmTitle != "")
            {
                alarmTitle_label.Visibility = Visibility.Hidden;
                alarm_Title.Text = data.alarmTitle.ToString();
            }

        }
        public AlarmData GetAlaraData()
        {
            if (tw_Time)
                initialData.hour = int.Parse(alarm_Hour.Content.ToString()) + 12;
            else
                initialData.hour = int.Parse(alarm_Hour.Content.ToString());
            initialData.minute = int.Parse(alarm_Minute.Content.ToString());
            initialData.activated = true;
            initialData.alarmTitle = alarm_Title.Text;
            initialData.repeatDOW = dow_selected;
            initialData.repeatCycle = dow_val;
            initialData.command = command_box.Text;
            return initialData;
        }

        private void alarm_Hour_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.Delta > 0)
            {
                time_control(1, true);
            }
            else if (e.Delta < 0)
            {
                time_control(1, false);
            }
        }
        private void alarm_Minute_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                time_control(0, true);
            }
            if (e.Delta < 0)
            {
                time_control(0, false);
            }
        }

        private void time_control(int type, bool plus_minus)
        {
            var temp_val = new int();
            switch (type)
            {
                case 0: //분
                    temp_val = int.Parse(alarm_Minute.Content.ToString());
                    if (plus_minus)
                    {
                        if (temp_val < 59)
                        {
                            alarm_Minute.Content = temp_val + 1;
                        }
                        else
                        {
                            alarm_Minute.Content = "0";
                            time_control(1, true);
                        }
                    }
                    else
                    {
                        if (temp_val > 0)
                        {
                            alarm_Minute.Content = temp_val - 1;
                        }
                        else
                        {
                            alarm_Minute.Content = "59";
                            time_control(1, false);
                        }
                    }
                    break;
                case 1: //시
                    temp_val = int.Parse(alarm_Hour.Content.ToString());
                    if (plus_minus)
                    {
                        if (temp_val < 12)
                        {
                            if (temp_val == 11)
                            {
                                time_control(2, true);
                            }
                            alarm_Hour.Content = temp_val + 1;
                        }
                        else
                        {
                            alarm_Hour.Content = "1";
                        }
                    }
                    else
                    {
                        if (temp_val > 1)
                        {
                            if (temp_val == 12)
                            {
                                time_control(2, true);
                            }
                            alarm_Hour.Content = temp_val - 1;
                        }
                        else
                        {
                            alarm_Hour.Content = "12";
                        }
                    }
                    break;
                case 2://오전, 오후
                    if (am_pm.Content.ToString() == "오전")
                    {
                        am_pm.Content = "오후";
                        tw_Time = true;
                    }
                    else if (am_pm.Content.ToString() == "오후")
                    {
                        am_pm.Content = "오전";
                        tw_Time = false;
                    }
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Rectangle_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    time_control(0, true);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    time_control(0, false);
                }
            }
        }

        private void mon_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dow_selected[0] = !dow_selected[0];
                if (dow_selected[0])
                {
                    color = dow_originCol[0].Color;
                    color.A = 255;
                    mon.Background = new SolidColorBrush(color);
                }
                else
                {
                    mon.Background = dow_originCol[0];
                }
            }
        }
        private void tue_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dow_selected[1] = !dow_selected[1];
                if (dow_selected[1])
                {
                    color = dow_originCol[1].Color;
                    color.A = 255;
                    tue.Background = new SolidColorBrush(color);
                }
                else
                {
                    tue.Background = dow_originCol[1];
                }
            }
        }
        private void we_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dow_selected[2] = !dow_selected[2];
                if (dow_selected[2])
                {
                    color = dow_originCol[2].Color;
                    color.A = 255;
                    wed.Background = new SolidColorBrush(color);
                }
                else
                {
                    wed.Background = dow_originCol[2];
                }
            }
        }
        private void thu_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dow_selected[3] = !dow_selected[3];
                if (dow_selected[3])
                {
                    color = dow_originCol[3].Color;
                    color.A = 255;
                    thu.Background = new SolidColorBrush(color);
                }
                else
                {
                    thu.Background = dow_originCol[3];
                }
            }
        }
        private void fri_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dow_selected[4] = !dow_selected[4];
                if (dow_selected[4])
                {
                    color = dow_originCol[4].Color;
                    color.A = 255;
                    fri.Background = new SolidColorBrush(color);
                }
                else
                {
                    fri.Background = dow_originCol[4];
                }
            }
        }
        private void sat_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dow_selected[5] = !dow_selected[5];
                if (dow_selected[5])
                {
                    color = dow_originCol[5].Color;
                    color.A = 255;
                    sat.Background = new SolidColorBrush(color);
                }
                else
                {
                    sat.Background = dow_originCol[5];
                }
            }
        }
        private void sun_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                dow_selected[6] = !dow_selected[6];
                if (dow_selected[6])
                {
                    color = dow_originCol[6].Color;
                    color.A = 255;
                    sun.Background = new SolidColorBrush(color);
                }
                else
                {
                    sun.Background = dow_originCol[6];
                }
            }
        }

        private void mon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_selected[0] = !dow_selected[0];
            if (dow_selected[0])
            {
                color = dow_originCol[0].Color;
                color.A = 255;
                mon.Background = new SolidColorBrush(color);
            }
            else
            {
                mon.Background = dow_originCol[0];
            }
        }
        private void tue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_selected[1] = !dow_selected[1];
            if (dow_selected[1])
            {
                color = dow_originCol[1].Color;
                color.A = 255;
                tue.Background = new SolidColorBrush(color);
            }
            else
            {
                tue.Background = dow_originCol[1];
            }
        }
        private void wed_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_selected[2] = !dow_selected[2];
            if (dow_selected[2])
            {
                color = dow_originCol[2].Color;
                color.A = 255;
                wed.Background = new SolidColorBrush(color);
            }
            else
            {
                wed.Background = dow_originCol[2];
            }
        }
        private void thu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_selected[3] = !dow_selected[3];
            if (dow_selected[3])
            {
                color = dow_originCol[3].Color;
                color.A = 255;
                thu.Background = new SolidColorBrush(color);
            }
            else
            {
                thu.Background = dow_originCol[3];
            }
        }
        private void fri_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_selected[4] = !dow_selected[4];
            if (dow_selected[4])
            {
                color = dow_originCol[4].Color;
                color.A = 255;
                fri.Background = new SolidColorBrush(color);
            }
            else
            {
                fri.Background = dow_originCol[4];
            }
        }
        private void sat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_selected[5] = !dow_selected[5];
            if (dow_selected[5])
            {
                color = dow_originCol[5].Color;
                color.A = 255;
                sat.Background = new SolidColorBrush(color);
            }
            else
            {
                sat.Background = dow_originCol[5];
            }
        }
        private void sun_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_selected[6] = !dow_selected[6];
            if (dow_selected[6])
            {
                color = dow_originCol[6].Color;
                color.A = 255;
                sun.Background = new SolidColorBrush(color);
            }
            else
            {
                sun.Background = dow_originCol[6];
            }
        }

        private void onceBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_val = 0;
            onceBtn.Background = dow_val_BtnSelCol;
            everydayBtn.Background = dow_val_BtnIdleCol;
            weekdayBtn.Background = dow_val_BtnIdleCol;
            dow_SelectBtn.Background = dow_val_BtnIdleCol;

            dowSelectPanel.Visibility = Visibility.Hidden;

            var label = (Label)sender;
            Keyboard.Focus(label.Target);
        }

        private void everydayBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_val = 1;
            onceBtn.Background = dow_val_BtnIdleCol;
            everydayBtn.Background = dow_val_BtnSelCol;
            weekdayBtn.Background = dow_val_BtnIdleCol;
            dow_SelectBtn.Background = dow_val_BtnIdleCol;
            dowSelectPanel.Visibility = Visibility.Hidden;

            var label = (Label)sender;
            Keyboard.Focus(label.Target);
        }

        private void weekdayBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_val = 2;
            onceBtn.Background = dow_val_BtnIdleCol;
            everydayBtn.Background = dow_val_BtnIdleCol;
            weekdayBtn.Background = dow_val_BtnSelCol;
            dow_SelectBtn.Background = dow_val_BtnIdleCol;
            dowSelectPanel.Visibility = Visibility.Hidden;

            var label = (Label)sender;
            Keyboard.Focus(label.Target);
        }

        private void dow_SelectBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dow_val = 3;
            onceBtn.Background = dow_val_BtnIdleCol;
            everydayBtn.Background = dow_val_BtnIdleCol;
            weekdayBtn.Background = dow_val_BtnIdleCol;
            dow_SelectBtn.Background = dow_val_BtnSelCol;
            dowSelectPanel.Visibility = Visibility.Visible;

            Keyboard.ClearFocus();
        }

        private void alarm_Cancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            alarmSetted = false;
            this.Close();
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            e.Handled = true;
        }

        private void alarm_Title_GotFocus(object sender, RoutedEventArgs e)
        {
            alarmTitle_label.Visibility = Visibility.Hidden;
        }

        private void alarm_Title_LostFocus(object sender, RoutedEventArgs e)
        {
            if (alarm_Title.Text == "")
            {
                alarm_Title.Visibility = Visibility.Visible;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            alarmgrid.Focus();
        }

        private void alarm_Apply_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            alarmSetted = true;
            this.Close();
        }
    }
}
