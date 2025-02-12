using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public abstract class EntityViewModel : BaseViewModel, IEquatable<EntityViewModel>
{
    public EntityViewModel()
    {
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, CanDelete, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }
    public abstract long Id { get; set; }

    public abstract string DisplayName { get; }

    public IAsyncRelayCommand DeleteCommand { get; }

    protected virtual bool CanDelete() => true;
    protected abstract Task DeleteAsync();

    public bool Equals(EntityViewModel? other)
    {
        return other is not null && other.GetType() == GetType() && Id == other.Id;
    }

    public override bool Equals(object? obj) => Equals(obj as EntityViewModel);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(EntityViewModel left, EntityViewModel right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(EntityViewModel left, EntityViewModel right) => !(left == right);
}
