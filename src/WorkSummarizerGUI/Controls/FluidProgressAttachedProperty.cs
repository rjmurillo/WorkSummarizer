using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WorkSummarizerGUI.Controls
{
    public class FluidProgress
    {
        public static double GetValue(DependencyObject obj)
        {
            return (double)obj.GetValue(SmoothValueProperty);
        }

        public static void SetValue(DependencyObject obj, double value)
        {
            obj.SetValue(SmoothValueProperty, value);
        }

        public static readonly DependencyProperty SmoothValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(FluidProgress), new PropertyMetadata(0.0, OnValueChanging));

        private static void OnValueChanging(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var progressBar = d as ProgressBar;
            var oldValue = (double)e.OldValue;
            var newValue = (double)e.NewValue;
            if (newValue < oldValue)
            {
                progressBar.Value = newValue;
            }
            else
            {
                var animation = new DoubleAnimation(oldValue, newValue, new TimeSpan(0, 0, 0, 0, 250));
                animation.EasingFunction = new CircleEase();
                progressBar.BeginAnimation(ProgressBar.ValueProperty, animation, HandoffBehavior.Compose);
            }
        }
    }
}
