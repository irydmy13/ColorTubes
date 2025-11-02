using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ColorTubes.Models;

public class LiquidSegment : INotifyPropertyChanged
{
    string _colorHex = "#FF0000";
    int _amount = 1;

    public string ColorHex
    {
        get => _colorHex;
        set { if (_colorHex != value) { _colorHex = value; OnPropertyChanged(); } }
    }

    public int Amount
    {
        get => _amount;
        set { if (_amount != value) { _amount = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
