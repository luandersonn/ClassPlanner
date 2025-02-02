using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public abstract class CollectionListViewModel<T> : BaseViewModel where T : class
{
    protected IServiceProvider Provider { get; set; }
    public CollectionListViewModel(IServiceProvider provider) : base(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread())
    {
        Provider = provider;

        Items = [];

        RefreshListCommand = new AsyncRelayCommand(LoadItemsAsync, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    public virtual IAsyncRelayCommand RefreshListCommand { get; }
    public virtual ObservableCollection<T> Items { get; }

    protected abstract Task LoadItemsAsync();

}