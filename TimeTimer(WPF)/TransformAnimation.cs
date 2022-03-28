using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TimeTimer_WPF_
{
    internal class TransformAnimation : Animation_system
    {
        public DoubleAnimation animation { get; set; }
        public double? From { get; set; }
        public double? To { get; set; }
        public Duration Duration { get; set; }
        public TransformAnimation(UIElement targetUIElement) : base(targetUIElement)
        {
            From = null;
            To = 0;
        }
        public override void SetAnimation()
        {
            animation = new DoubleAnimation();
            if (TargetElement.RenderTransform is TransformGroup)
            {
                int index = -1;
                foreach (Transform transform in (TargetElement.RenderTransform as TransformGroup).Children)
                {
                    index++;

                    if (transform is TransformAnimation)
                    {
                        Storyboard.SetTargetProperty
                            (
                            animation,
                            new PropertyPath(
                                String.Format("(UIElement.RenderTransform).(TransformGroup.Children[{0}].(TranslateTransform.X)", index)
                                )
                            );
                        break;
                    }
                }
            }
            else
            {
                if(!(TargetElement.RenderTransform is Transform))
                {
                    throw new NullReferenceException("RotateTransform 객체를 찾지못함.");
                }

                Storyboard.SetTargetProperty
                    (
                        animation,
                        new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)")
                    );
            }
            Storyboard.Children.Add(animation);
            //Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            //Storyboard.SetTargetProperty(animation, new PropertyPath("Top"));
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
