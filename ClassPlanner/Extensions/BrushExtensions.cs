using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Reflection;

namespace ClassPlanner.Extensions;

public static class BrushExtensions
{
    public static readonly DependencyProperty ApplyBrushToProperty =
        DependencyProperty.RegisterAttached(
            "ApplyBrushTo",
            typeof(BrushTarget),
            typeof(BrushExtensions),
            new PropertyMetadata(BrushTarget.None, OnApplyBrushToChanged));

    public static readonly DependencyProperty DynamicBrushKeyProperty =
        DependencyProperty.RegisterAttached(
            "DynamicBrushKey",
            typeof(string),
            typeof(BrushExtensions),
            new PropertyMetadata(null, OnDynamicBrushKeyChanged));

    public static BrushTarget GetApplyBrushTo(DependencyObject obj) =>
        (BrushTarget)obj.GetValue(ApplyBrushToProperty);

    public static void SetApplyBrushTo(DependencyObject obj, BrushTarget value) =>
        obj.SetValue(ApplyBrushToProperty, value);

    public static string GetDynamicBrushKey(DependencyObject obj) =>
        (string)obj.GetValue(DynamicBrushKeyProperty);

    public static void SetDynamicBrushKey(DependencyObject obj, string value) =>
        obj.SetValue(DynamicBrushKeyProperty, value);

    private static void OnApplyBrushToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            ApplyBrushes(element);
        }
    }

    private static void OnDynamicBrushKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            ApplyBrushes(element);
        }
    }

    private static void ApplyBrushes(FrameworkElement element)
    {
        string key = GetDynamicBrushKey(element);
        BrushTarget target = GetApplyBrushTo(element);

        if (string.IsNullOrEmpty(key) || target == BrushTarget.None)
            return;

        Brush brush = ColorGenerator.GetBrush(key);

        foreach (BrushTarget value in Enum.GetValues<BrushTarget>())
        {
            if (value != BrushTarget.None && target.HasFlag(value))
            {
                PropertyInfo? property = element.GetType().GetProperty(value.ToString());

                if (property is not null && property.PropertyType == typeof(Brush))
                {
                    try
                    {
                        property.SetValue(element, brush);
                    }
                    catch { }
                }
            }
        }
        //SolidColorBrush brush = ColorGenerator.GetBrush(key);


        //if (target.HasFlag(BrushTarget.Background) && element is Panel panelWithBackground)
        //{
        //    panelWithBackground.Background = brush;
        //}
        //else if (target.HasFlag(BrushTarget.Background) && element is Control controlWithBackground)
        //{
        //    controlWithBackground.Background = brush;
        //}

        //if (target.HasFlag(BrushTarget.Foreground) && element is Control controlWithForeground)
        //{
        //    controlWithForeground.Foreground = brush;
        //}

        //if (target.HasFlag(BrushTarget.Foreground) && element is FontIcon icon)
        //{
        //    icon.Foreground = brush;
        //}

        //if (target.HasFlag(BrushTarget.BorderBrush))
        //{
        //    if (element is Control controlWithBorderBrush)
        //    {
        //        controlWithBorderBrush.BorderBrush = brush;
        //    }
        //    else if (element is Border borderWithBorderBrush)
        //    {
        //        borderWithBorderBrush.BorderBrush = brush;
        //    }
        //    else if (element is Grid gridWithBorderBrush)
        //    {
        //        gridWithBorderBrush.BorderBrush = brush;
        //    }
        //}
    }
}
