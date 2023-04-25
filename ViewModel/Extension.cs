using System.Windows;

namespace ViewModel
{
    public static class ActualSizeGetter
    {
        #region ActualSizeBool

        public static readonly DependencyProperty ActualSizeProperty = DependencyProperty.RegisterAttached("ActualSize", typeof(bool), typeof(ActualSizeGetter), new UIPropertyMetadata(false, OnActualSizeChanged));

        public static bool GetActualSize(DependencyObject obj)
        {
            return (bool)obj.GetValue(ActualSizeProperty);
        }
        public static void SetActualSize(DependencyObject obj, bool value)
        {
            obj.SetValue(ActualSizeProperty, value);
        }
        private static void OnActualSizeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dpo;
            if ((bool)e.NewValue == true)
            {
                element.SizeChanged += Element_SizeChanged;
            }
            else
            {
                element.SizeChanged -= Element_SizeChanged;
            }
        }
        static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement? element = sender as FrameworkElement;
            SetActualWidth(element, element?.ActualWidth);
            SetActualHeight(element, element?.ActualHeight);
        }

        #endregion ActualSizeBool

        #region ActualWidth

        private static readonly DependencyProperty ActualWidthProperty = DependencyProperty.RegisterAttached("ActualWidth", typeof(double), typeof(ActualSizeGetter));

        public static void SetActualWidth(DependencyObject? element, double? value)
        {
            element?.SetValue(ActualWidthProperty, value);
        }
        public static double GetActualWidth(DependencyObject element)
        {
            return (double)element.GetValue(ActualWidthProperty);
        }

        #endregion ActualWidth

        #region ActualHeight

        private static readonly DependencyProperty ActualHeightProperty =
            DependencyProperty.RegisterAttached("ActualHeight", typeof(double), typeof(ActualSizeGetter));

        public static void SetActualHeight(DependencyObject? element, double? value)
        {
            element?.SetValue(ActualHeightProperty, value);
        }
        public static double GetActualHeight(DependencyObject element)
        {
            return (double)element.GetValue(ActualHeightProperty);
        }

        #endregion ActualHeight
    }
}
