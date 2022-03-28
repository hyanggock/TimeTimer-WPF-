using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace TimeTimer_WPF_
{
    public abstract class Animation_system
    {
        public event EventHandler completed;
        public UIElement TargetElement
        {
            get;
            set;
        }
        public Storyboard Storyboard
        {
            get;
            set;
        }
        public Animation_system(UIElement targetUIelement)
        {
            TargetElement = targetUIelement ?? throw new NullReferenceException("pTargetUIElement is null");
            Storyboard = new Storyboard();
            Storyboard.Completed += new EventHandler(StoryBoard_Completed);
            Storyboard.SetTarget(Storyboard, TargetElement);
            SetAnimation();
        }
        public abstract void SetAnimation();
        public virtual void Begin()
        {
            Storyboard.Begin();
        }
        private void StoryBoard_Completed(object sender, EventArgs e)
        {
            completed?.Invoke(sender, e);
        }
    }
}
