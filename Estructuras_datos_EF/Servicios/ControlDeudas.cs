using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;

namespace Estructuras_datos_EP.Servicios;

public class Compra
{
    public DateTime Fecha { get; set; }
    public string Distribuidor { get; set; } = "";
    public decimal Monto { get; set; }
}

public class ControlDeudas
{
    public const string Bodega = "Bodega Chabelita";

    private readonly Func<DateTime> reloj;

    public ListaSimple<Compra> Compras { get; private set; } = new ListaSimple<Compra>();

    public ControlDeudas(Func<DateTime>? reloj_externo = null)
    {
        reloj = reloj_externo ?? (() => DateTime.Now);
    }

    public void Limpiar() => Compras = new ListaSimple<Compra>();

    private static bool Mismo(string a, string b) => string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool EsPagoA(Operacion operacion, Distribuidor distribuidor) =>
        operacion.Tipo == TipoOperacion.Transferencia
        && Mismo(operacion.Origen, Bodega)
        && Mismo(operacion.Destino, distribuidor.Nombre);

    public string? ComprarFiado(Distribuidor distribuidor, decimal monto)
    {
        if (!distribuidor.AceptaFiado) return $"{distribuidor.Nombre} no vende fiado: solo al contado.";
        if (monto <= 0) return "El monto de la compra debe ser mayor que 0.";
        Compras.AgregarFinal(new Compra { Fecha = reloj(), Distribuidor = distribuidor.Nombre, Monto = monto });
        return null;
    }

    public decimal TotalFiado(Distribuidor distribuidor)
    {
        decimal total = 0;
        foreach (Compra compra in Compras)
            if (Mismo(compra.Distribuidor, distribuidor.Nombre)) total += compra.Monto;
        return total;
    }

    public decimal Pagado(GestorTransacciones gestor, Distribuidor distribuidor)
    {
        decimal total = 0;
        foreach (Operacion operacion in gestor.Registro)
            if (EsPagoA(operacion, distribuidor) && operacion.Estado == EstadoOperacion.Procesada) total += operacion.Monto;
        return total;
    }

    public decimal EnTramite(GestorTransacciones gestor, Distribuidor distribuidor)
    {
        decimal total = 0;
        foreach (Operacion operacion in gestor.Registro.Concat(gestor.Llegadas))
            if (EsPagoA(operacion, distribuidor) && operacion.Estado == EstadoOperacion.Pendiente) total += operacion.Monto;
        return total;
    }

    public decimal Saldo(GestorTransacciones gestor, Distribuidor distribuidor) =>
        TotalFiado(distribuidor) - Pagado(gestor, distribuidor) - EnTramite(gestor, distribuidor);

    public static int SiguienteNumero(GestorTransacciones gestor) =>
        gestor.Registro.Concat(gestor.Llegadas).Select(operacion => operacion.Numero).DefaultIfEmpty(0).Max() + 1;

    public List<string> PagarDeuda(GestorTransacciones gestor, Distribuidor distribuidor, decimal monto, out Operacion? operacion)
    {
        operacion = null;
        List<string> errores = new List<string>();
        decimal saldo = Saldo(gestor, distribuidor);
        if (saldo <= 0)
        {
            errores.Add($"{distribuidor.Nombre} no tiene saldo pendiente de pago.");
            return errores;
        }
        if (monto > saldo)
        {
            errores.Add($"El pago (S/ {monto:0.00}) supera el saldo pendiente (S/ {saldo:0.00}).");
            return errores;
        }

        Operacion nueva = new Operacion
        {
            Codigo = $"OP-{SiguienteNumero(gestor):000}",
            FechaHora = reloj(),
            Tipo = TipoOperacion.Transferencia,
            Monto = monto,
            Origen = Bodega,
            Destino = distribuidor.Nombre
        };
        errores = gestor.Llegar(nueva);
        if (errores.Count == 0) operacion = nueva;
        return errores;
    }
}
