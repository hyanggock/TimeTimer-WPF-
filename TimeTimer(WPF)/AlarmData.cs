using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTimer_WPF_
{
    [Serializable]
    public class AlarmData
    {
        public int hour;
        public int minute;
        public int recent_activated_day;
        public int recent_activated_month;

        /// <summary>
        /// repeat Cycle : 0=한번 1=매일 2=평일,3=요일지정
        /// repeat DOW : 요일
        /// </summary>
        public int repeatCycle;
        public bool[] repeatDOW = new bool[7];
        public bool activated;

        public string alarmTitle = "";
        public string command = "";
        //public string alarmDescription = "";
    }
}
