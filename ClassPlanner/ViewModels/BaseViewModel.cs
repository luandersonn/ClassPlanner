using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using System;

namespace ClassPlanner.ViewModels;

public abstract partial class BaseViewModel(DispatcherQueue dispatcherQueue) : ObservableObject
{
    public BaseViewModel() : this(DispatcherQueue.GetForCurrentThread()) { }

    protected virtual DispatcherQueue DispatcherQueue { get; set; } = dispatcherQueue;
    protected virtual IServiceProvider ServiceProvider { get; set; } = App.Current.ServiceProvider;

    protected virtual bool AccessUI(Action action) => DispatcherQueue.TryEnqueue(() => action());
}