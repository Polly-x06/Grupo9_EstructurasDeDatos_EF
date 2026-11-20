using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Modelos;

public enum TipoOperacion { PagoQR, Transferencia, Recarga, Abono }

public enum EstadoOperacion { Pendiente, Procesada, Fallida, Revertida }

public class Operacion
{
    public string Codigo { get; set; } = "";
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

    public override string ToString() =>
        $"{Codigo} | {FechaHora:dd/MM/yyyy HH:mm} | {Tipo} | S/ {Monto:0.00} | {Origen} -> {Destino} | {Estado} | intentos: {Intentos}";
}
