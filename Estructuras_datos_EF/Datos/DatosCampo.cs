using Estructuras_datos_EP.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Datos;

public static class DatosCampo
{
    private const string Destino = "Bodega Chabelita";

    public static List<Operacion> Obtener()
    {
        (string codigo, int hora, int minuto, decimal monto, string cliente)[] filas = new (string codigo, int hora, int minuto, decimal monto, string cliente)[]
        {
            ("OP-001", 18,  3,  4.50m, "Cliente 01"),
            ("OP-002", 18,  8,  3.00m, "Cliente 02"),
            ("OP-003", 18, 15,  5.20m, "Cliente 03"),
            ("OP-004", 18, 24, 10.00m, "Cliente 04"),
            ("OP-005", 18, 32, 20.50m, "Cliente 05"),
            ("OP-006", 18, 38,  1.00m, "Cliente 06"),
            ("OP-007", 18, 46,  6.30m, "Cliente 07"),
            ("OP-008", 18, 47,  2.00m, "Cliente 08"),
            ("OP-009", 18, 52,  4.60m, "Cliente 09"),
            ("OP-010", 19,  4, 10.50m, "Cliente 10"),
            ("OP-011", 19, 13,  6.00m, "Cliente 11"),
            ("OP-012", 19, 24,  5.00m, "Cliente 12"),
        };

        List<Operacion> lista_operaciones = new List<Operacion>();
        foreach ((string codigo, int hora, int minuto, decimal monto, string cliente) fila in filas)
        {
            lista_operaciones.Add(new Operacion
            {
                Codigo = fila.codigo,
                FechaHora = new DateTime(2026, 10, 12, fila.hora, fila.minuto, 0),
                Tipo = TipoOperacion.PagoQR,
                Monto = fila.monto,
                Origen = fila.cliente,
                Destino = Destino,
                Estado = EstadoOperacion.Procesada,
                Intentos = 1
            });
        }
        return lista_operaciones;
    }
}
