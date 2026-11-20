namespace Estructuras_datos_EP;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FrmOperacionesBodega());
    }
}