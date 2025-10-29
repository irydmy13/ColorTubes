using System;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace ColorTubes.Services;

// Аудиосервис для эффектов. Файлы положи в Resources/Raw: pour.wav, win.wav
public class AudioService
{
    private IAudioPlayer? _pour;
    private IAudioPlayer? _win;

    public async Task InitAsync()
    {
        // В некоторых версиях плагина CreatePlayer синхронный и принимает Stream.
        // Используем текущий AudioManager из плагина с полным именем, чтобы не путать с Android.Media.AudioManager.
        var pourStream = await FileSystem.OpenAppPackageFileAsync("pour.wav");
        _pour = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(pourStream);

        var winStream = await FileSystem.OpenAppPackageFileAsync("win.wav");
        _win = Plugin.Maui.Audio.AudioManager.Current.CreatePlayer(winStream);
    }

    public void PlayPour()
    {
        if (_pour == null) return;
        _pour.Stop();
        _pour.Play();
    }

    public void PlayWin()
    {
        if (_win == null) return;
        _win.Stop();
        _win.Play();
    }
}
