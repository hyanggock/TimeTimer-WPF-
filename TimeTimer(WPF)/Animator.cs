using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace TimeTimer_WPF_
{
    internal class Animator
    {
        public TimeSpan? BeginTime
        {
            get;
            set;
        }
        public Duration Duration
        {
            get;
            set;
        }
        public double SpeedRatio
        {
            get;
            set;
        }
        public RepeatBehavior RepeatBehavior
        {
            get;
            set;
        }
        public bool AutoReverse
        {
            get;
            set;
        }
        public FillBehavior FillBehavior
        {
            get;
            set;
        }

        public List<Animation_system> AnimationBaseList
        {
            get;
            set;
        }
        public Animator()
        {
            SpeedRatio = 1;
            RepeatBehavior = new RepeatBehavior(1);
            AnimationBaseList = new List<Animation_system>();
        }

        public void Begin()
        {
            foreach(Animation_system animationBase in AnimationBaseList)
            {
                SetValue(animationBase);
            }
            foreach(Animation_system animationBase in AnimationBaseList)
            {
                animationBase.Begin();
            }
        }

        private void SetValue(Animation_system animationBase)
        {
            animationBase.Storyboard.BeginTime = BeginTime;
            animationBase.Storyboard.Duration = Duration;
            animationBase.Storyboard.SpeedRatio = SpeedRatio;
            animationBase.Storyboard.RepeatBehavior = RepeatBehavior;
            animationBase.Storyboard.AutoReverse = AutoReverse;
            animationBase.Storyboard.FillBehavior = FillBehavior;
        }
    }
}
