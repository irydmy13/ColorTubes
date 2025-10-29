using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ColorTubes.ViewModels;

//Базовая имплементация INotifyPropertyChanged.
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (!Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
