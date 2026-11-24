namespace Estructuras_datos_EP.Modelos;

public class Distribuidor
{
    public int Indice { get; set; }
    public string Nombre { get; set; } = "";

    public bool AceptaFiado => !Nombre.Contains("makro", StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Nombre;
}