using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Modelos;

public enum TipoOperacion { PagoQR, Transferencia, Recarga, Abono }

public enum EstadoOperacion { Pendiente, Procesada, Fallida, Revertida }

public class Operacion
{
    private string codigo = "";
    private int numero;

    public string Codigo
    {
        get => codigo;
        set
        {
            codigo = value;
            TryObtenerNumero(value, out numero);
        }
    }

    public int Numero => numero;

    public DateTime FechaHora { get; set; }
    public TipoOperacion Tipo { get; set; }
    public decimal Monto { get; set; }
    public string Origen { get; set; } = "";
    public string Destino { get; set; } = "";
    public EstadoOperacion Estado { get; set; } = EstadoOperacion.Pendiente;
    public int Intentos { get; set; }

    public bool EsAbono => Tipo == TipoOperacion.PagoQR
                        || Tipo == TipoOperacion.Transferencia
                        || Tipo == TipoOperacion.Abono;

    public static bool TryObtenerNumero(string? codigo, out int numero)
    {
        numero = 0;
        string texto = (codigo ?? "").Trim().ToUpperInvariant();
        return texto.StartsWith("OP-", StringComparison.Ordinal)
            && int.TryParse(texto.AsSpan(3), NumberStyles.None, CultureInfo.InvariantCulture, out numero);
    }

    public override string ToString() =>
        $"{Codigo} | {FechaHora:dd/MM/yyyy HH:mm} | {Tipo} | S/ {Monto:0.00} | {Origen} -> {Destino} | {Estado} | intentos: {Intentos}";
}