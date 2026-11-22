using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JHNegocio.Movil.ViewModels;

/// <summary>Base MVVM: notifica cambios a la interfaz.</summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _estaOcupado;
    private string _titulo = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool EstaOcupado
    {
        get => _estaOcupado;
        set { if (SetProperty(ref _estaOcupado, value)) OnPropertyChanged(nameof(NoEstaOcupado)); }
    }

    public bool NoEstaOcupado => !EstaOcupado;

    public string Titulo
    {
        get => _titulo;
        set => SetProperty(ref _titulo, value);
    }

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
        campo = valor;
        OnPropertyChanged(nombre);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? nombre = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
}
