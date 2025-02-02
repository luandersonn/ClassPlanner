using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Reflection;
using Windows.System;

namespace ClassPlanner.Extensions;

public static class ControlExtensions
{
    public static T GetTemplateChild<T>(this Control controlTemplate, string childName)
        where T : DependencyObject
    {
        MethodInfo? methodInfo = controlTemplate.GetType().GetMethod(nameof(GetTemplateChild), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                ?? throw new InvalidOperationException("Metodo não disponível");
        T? t = methodInfo.Invoke(controlTemplate, [childName]) as T;
        return t ?? throw new InvalidOperationException("Controle não encontrado");
    }

    public delegate void KeyboardAcceleratorInvokedCallBack(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args);
    public static KeyboardAccelerator AddKeyboardAccelerator(this UIElement element, VirtualKey Key, KeyboardAcceleratorInvokedCallBack callBack)
    {
        return AddKeyboardAccelerator(element, Key, VirtualKeyModifiers.None, callBack);
    }

    public static KeyboardAccelerator AddKeyboardAccelerator(this UIElement element, VirtualKey key, VirtualKeyModifiers modifiers, KeyboardAcceleratorInvokedCallBack callBack)
    {
        var accelerator = new KeyboardAccelerator
        {
            Key = key,
            Modifiers = modifiers
        };
        accelerator.Invoked += (s, e) => callBack(s, e);
        element.KeyboardAccelerators.Add(accelerator);
        return accelerator;
    }
}
