using ClassPlanner.Timetabling.Validation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace ClassPlanner.Controls;

[ContentProperty(Name = "Content")]
public partial class ValidationMessageControl : ContentControl
{
    public ValidationMessageControl()
    {
        DefaultStyleKey = typeof(ValidationMessageControl);
        Loaded += (_, _) => UpdateVisualState(false);
    }


    public ValidationResultType Validation
    {
        get => (ValidationResultType)GetValue(ValidationProperty);
        set
        {
            SetValue(ValidationProperty, value);
            UpdateVisualState(false);
        }
    }

    public static readonly DependencyProperty ValidationProperty = DependencyProperty.Register("Validation",
                                                                                               typeof(ValidationResultType),
                                                                                               typeof(ValidationMessageControl),
                                                                                               new PropertyMetadata(0, OnValidationChanged));


    private static void OnValidationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ValidationMessageControl control)
        {
            control.UpdateVisualState(false);
        }
    }


    private void UpdateVisualState(bool useTransitions) => VisualStateManager.GoToState(this, Validation.ToString(), useTransitions);
}
