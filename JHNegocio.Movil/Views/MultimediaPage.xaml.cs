using CommunityToolkit.Maui.Views;

namespace JHNegocio.Movil.Views;

/// <summary>
/// Centro multimedia: reproduce un video y un audio empaquetados en Resources/Raw
/// usando MediaElement del .NET MAUI Community Toolkit.
/// </summary>
public partial class MultimediaPage : ContentPage
{
    private const string ArchivoVideo = "promo_jhnegocio.mp4";
    private const string ArchivoAudio = "jingle_jhnegocio.mp3";

    private static readonly Color ColorActivo = Color.FromArgb("#00E5FF");
    private static readonly Color ColorInactivo = Color.FromArgb("#141B34");

    private bool _arrastrando;

    public MultimediaPage()
    {
        InitializeComponent();

        // Eventos del reproductor (se actualiza la interfaz en el hilo principal)
        player.MediaOpened += (s, e) => MainThread.BeginInvokeOnMainThread(ActualizarDuracion);
        player.PositionChanged += (s, e) => MainThread.BeginInvokeOnMainThread(ActualizarPosicion);
        player.MediaEnded += (s, e) => MainThread.BeginInvokeOnMainThread(() =>
        {
            LblEstado.Text = "Estado: reproducción terminada";
            BtnPlay.Text = "▶  Reproducir";
        });
        player.MediaFailed += (s, e) => MainThread.BeginInvokeOnMainThread(() =>
            LblEstado.Text = $"Error: {e.ErrorMessage}");
        player.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MediaElement.CurrentState))
                MainThread.BeginInvokeOnMainThread(ActualizarEstado);
        };

        MarcarSeleccion(esAudio: false);
    }

    // ---------- Selección de recurso ----------

    private void OnVideoClicked(object? sender, EventArgs e) =>
        CambiarRecurso(ArchivoVideo, esAudio: false,
            "Video promocional JH Negocio", "Presentación de la app · MP4 (H.264 + AAC)");

    private void OnAudioClicked(object? sender, EventArgs e) =>
        CambiarRecurso(ArchivoAudio, esAudio: true,
            "Jingle JH Negocio", "Audio original · MP3 128 kbps");

    private void CambiarRecurso(string archivo, bool esAudio, string titulo, string descripcion)
    {
        player.Stop();
        player.Source = MediaSource.FromResource(archivo);   // equivale a embed://archivo
        Portada.IsVisible = esAudio;
        LblTitulo.Text = titulo;
        LblDescripcion.Text = descripcion;
        LblFuente.Text = $"Fuente: embed://{archivo}";
        SliderProgreso.Value = 0;
        LblPosicion.Text = "00:00";
        MarcarSeleccion(esAudio);
    }

    private void MarcarSeleccion(bool esAudio)
    {
        BtnVideo.BackgroundColor = esAudio ? ColorInactivo : ColorActivo;
        BtnVideo.TextColor = esAudio ? Colors.White : ColorInactivo;
        BtnAudio.BackgroundColor = esAudio ? ColorActivo : ColorInactivo;
        BtnAudio.TextColor = esAudio ? ColorInactivo : Colors.White;
    }

    // ---------- Controles ----------

    private void OnPlayPauseClicked(object? sender, EventArgs e)
    {
        if (player.CurrentState.ToString() == "Playing")
            player.Pause();
        else
            player.Play();
    }

    private void OnStopClicked(object? sender, EventArgs e)
    {
        player.Stop();
        SliderProgreso.Value = 0;
        LblPosicion.Text = "00:00";
    }

    private void OnDragStarted(object? sender, EventArgs e) => _arrastrando = true;

    private async void OnDragCompleted(object? sender, EventArgs e)
    {
        _arrastrando = false;
        await player.SeekTo(TimeSpan.FromSeconds(SliderProgreso.Value), CancellationToken.None);
    }

    // ---------- Actualización de la interfaz ----------

    private void ActualizarDuracion()
    {
        var segundos = player.Duration.TotalSeconds;
        if (segundos > 0)
            SliderProgreso.Maximum = segundos;
        LblDuracion.Text = Formato(player.Duration);
    }

    private void ActualizarPosicion()
    {
        if (player.Duration.TotalSeconds > SliderProgreso.Maximum)
            ActualizarDuracion();

        if (!_arrastrando)
            SliderProgreso.Value = Math.Min(player.Position.TotalSeconds, SliderProgreso.Maximum);

        LblPosicion.Text = Formato(player.Position);
    }

    private void ActualizarEstado()
    {
        var estado = player.CurrentState.ToString();
        LblEstado.Text = $"Estado: {Traducir(estado)}";
        BtnPlay.Text = estado == "Playing" ? "⏸  Pausar" : "▶  Reproducir";
        PuntoEstado.Color = estado switch
        {
            "Playing" => Color.FromArgb("#00E676"),
            "Paused" => Color.FromArgb("#FFB300"),
            "Failed" => Color.FromArgb("#FF5252"),
            _ => Color.FromArgb("#9AA4C7")
        };
    }

    private static string Traducir(string estado) => estado switch
    {
        "Playing" => "reproduciendo",
        "Paused" => "en pausa",
        "Stopped" => "detenido",
        "Opening" => "abriendo archivo",
        "Buffering" => "cargando",
        "Failed" => "error",
        _ => "listo"
    };

    private static string Formato(TimeSpan t) => $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        player.Stop();   // no seguir sonando al salir de la pantalla
    }
}
