using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Modelos;

public static class Validador
{
    public const decimal MontoMaximo = 500m;
    public const int MaxIntentos = 3;

    private static readonly Regex FormatoCodigo = new(@"^OP-\d{3,4}$");

    public static List<string> Validar(Operacion operacion, DateTime ahora)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(operacion.Codigo))
            errores.Add("El código no puede estar vacío.");
        else if (!FormatoCodigo.IsMatch(operacion.Codigo))
            errores.Add("El código debe tener el formato OP-001 (OP- y 3 o 4 dígitos).");

        if (operacion.FechaHora > ahora)
            errores.Add("La fecha y hora no puede ser futura.");

        if (!Enum.IsDefined(typeof(TipoOperacion), operacion.Tipo))
            errores.Add("El tipo de operación no es válido.");

        if (operacion.Monto <= 0)
            errores.Add("El monto debe ser mayor que 0.");
        else if (operacion.Monto > MontoMaximo)
            errores.Add($"El monto no puede superar S/ {MontoMaximo:0.00}.");

        if (string.IsNullOrWhiteSpace(operacion.Origen))
            errores.Add("El origen no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(operacion.Destino))
            errores.Add("El destino no puede estar vacío.");
        if (!string.IsNullOrWhiteSpace(operacion.Origen) && !string.IsNullOrWhiteSpace(operacion.Destino)
            && string.Equals(operacion.Origen.Trim(), operacion.Destino.Trim(), StringComparison.OrdinalIgnoreCase))
            errores.Add("El origen y el destino deben ser distintos.");

        if (operacion.Intentos < 0 || operacion.Intentos > MaxIntentos)
            errores.Add($"El número de intentos debe estar entre 0 y {MaxIntentos}.");

        return errores;
    }
}