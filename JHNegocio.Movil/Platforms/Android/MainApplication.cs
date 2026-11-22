using Android.App;
using Android.Runtime;

namespace JHNegocio.Movil;

// UsesCleartextTraffic = true permite llamar a la API por http:// en el emulador (solo desarrollo)
[Application(UsesCleartextTraffic = true)]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
