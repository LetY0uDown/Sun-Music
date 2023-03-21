using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Base;

public abstract class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [Obsolete("Use PropertyChanged.Fody nuget instead")]
    protected void OnPropertyChanged ([CallerMemberName] string callerName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));
    }

    public virtual Task Display() => Task.CompletedTask;

    public virtual Task Leave() => Task.CompletedTask;
}