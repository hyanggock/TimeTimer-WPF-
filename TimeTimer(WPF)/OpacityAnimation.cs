using System.Windows;
using System.Windows.Media.Animation;

namespace TimeTimer_WPF_
{
    public class OpacityAnimation : Animation_system
    {
        public DoubleAnimation animation { get; set; }
        public double? From { get; set; }
        public double? To { get; set; }
        public Duration Duration { get; set; }
        public OpacityAnimation(UIElement targetUIElement) : base(targetUIElement)
        {
            From = null;
            To = 0;
        }
        public override void SetAnimation()
        {
            animation = new DoubleAnimation();
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)"));
            Storyboard.Children.Add(animation);
           
        }
        public override void Begin()
        {
            animation.From = From;
            animation.To = To;
            animation.Duration = Duration;

            base.Begin();
        }
    }
}
