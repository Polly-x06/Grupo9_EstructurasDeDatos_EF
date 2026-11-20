using Estructuras_datos_EP.Datos;
using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using Estructuras_datos_EP.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras_datos_EP.Pruebas;

public class EjecutorPruebas
{
    private readonly StringBuilder reporte = new();
    public int Ok { get; private set; }
    public int Falla { get; private set; }

    private static DateTime Ahora() => new DateTime(2026, 10, 17, 20, 0, 0);
    private static GestorTransacciones NuevoGestor() => new GestorTransacciones(Ahora);

    private void Escribir(string texto) => reporte.AppendLine(texto);

    private static void Afirmar(bool condicion, string mensaje)
    {
        if (!condicion) throw new Exception("Falló: " + mensaje);
    }

    private void Caso(string nombre, Action prueba)
    {
        try { prueba(); Ok++; Escribir($"[OK]    {nombre}"); }
        catch (Exception excepcion) { Falla++; Escribir($"[FALLA] {nombre} -> {excepcion.Message}"); }
    }

    private static Operacion Op(string codigo, decimal monto = 5m, string origen = "Cliente 01",
                                string destino = "Bodega Chabelita", TipoOperacion tipo = TipoOperacion.PagoQR) =>
        new Operacion
        {
            Codigo = codigo,
            FechaHora = new DateTime(2026, 10, 12, 18, 3, 0),
            Tipo = tipo,
            Monto = monto,
            Origen = origen,
            Destino = destino
        };

    private static GestorTransacciones GestorConCampo()
    {
        GestorTransacciones gestor = NuevoGestor();
        List<string> errores = gestor.CargarLlegadas(DatosCampo.Obtener());
        Afirmar(errores.Count == 0, "los datos de campo tienen errores: " + string.Join(" / ", errores));
        return gestor;
    }

    public string Correr()
    {
        reporte.Clear(); Ok = 0; Falla = 0;
        Escribir("PRUEBAS - Estructuras de Datos (EP) - Bodega Chabelita");
        Escribir($"Fecha de ejecución: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        Escribir(new string('-', 70));

        Caso("1. Estructura vacía (las 5 estructuras)", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            Afirmar(gestor.Registro.EstaVacia && gestor.Registro.Cantidad == 0, "lista simple vacía");
            Afirmar(gestor.Historial.EstaVacia && gestor.Historial.Actual == null, "lista doble vacía");
            Afirmar(!gestor.Historial.Siguiente() && !gestor.Historial.Anterior(), "navegar en lista vacía devuelve false");
            Afirmar(gestor.Reintentos.EstaVacia && gestor.Reintentos.Actual == null, "lista circular vacía");
            Afirmar(gestor.Reintentos.Cantidad == 0, "circular sin elementos");
            Afirmar(gestor.Reintentos.Any() == false, "recorrer circular vacía no entra en bucle");
            Afirmar(gestor.Abonos.EstaVacia, "pila vacía");
            Afirmar(gestor.Llegadas.EstaVacia, "cola vacía");
        });

        Caso("2. Registro de una operación (encolar, atender, queda registrada)", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            Afirmar(gestor.Llegar(Op("OP-001")).Count == 0, "debe aceptarse");
            Afirmar(gestor.Llegadas.Cantidad == 1, "1 en cola");
            Operacion operacion_atendida = gestor.Atender();
            Afirmar(operacion_atendida.Estado == EstadoOperacion.Procesada && operacion_atendida.Intentos == 1, "procesada al 1er intento");
            Afirmar(gestor.Registro.Cantidad == 1 && gestor.Historial.Cantidad == 1 && gestor.Abonos.Cantidad == 1, "en lista simple, doble y pila");
            Afirmar(gestor.Llegadas.EstaVacia, "cola vacía tras atender");
        });

        Caso("3. Registro de varias operaciones (12 operaciones de campo, orden FIFO)", () =>
        {
            GestorTransacciones gestor = GestorConCampo();
            Afirmar(gestor.Llegadas.Cantidad == 12, "12 llegadas encoladas");
            List<string> orden_atencion = new List<string>();
            while (!gestor.Llegadas.EstaVacia) orden_atencion.Add(gestor.Atender().Codigo);
            Afirmar(orden_atencion[0] == "OP-001" && orden_atencion[11] == "OP-012", "orden de arribo respetado");
            Afirmar(gestor.Registro.Cantidad == 12, "12 en el registro");
            Afirmar(gestor.Registro.All(operacion => operacion.Estado == EstadoOperacion.Procesada), "todas procesadas, como en campo");
            Afirmar(gestor.Registro.Sum(operacion => operacion.Monto) == 78.60m, "monto total S/ 78.60 igual que la bitácora");
        });

        Caso("4. Navegación del historial hacia adelante y atrás", () =>
        {
            GestorTransacciones gestor = GestorConCampo();
            while (!gestor.Llegadas.EstaVacia) gestor.Atender();
            gestor.Historial.IrAlInicio();
            Afirmar(gestor.Historial.Actual!.Codigo == "OP-001", "inicio = OP-001");
            Afirmar(!gestor.Historial.Anterior(), "no se puede retroceder desde el inicio");
            gestor.Historial.Siguiente(); gestor.Historial.Siguiente();
            Afirmar(gestor.Historial.Actual!.Codigo == "OP-003", "dos pasos adelante = OP-003");
            gestor.Historial.Anterior();
            Afirmar(gestor.Historial.Actual!.Codigo == "OP-002", "un paso atrás = OP-002");
            gestor.Historial.IrAlFinal();
            Afirmar(gestor.Historial.Actual!.Codigo == "OP-012", "final = OP-012");
            Afirmar(!gestor.Historial.Siguiente(), "no se puede avanzar desde el final");
        });

        Caso("5. Recorrido circular con condición de parada (3 intentos)", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            gestor.Llegar(Op("OP-101", 15m, "Cliente A")); gestor.Llegar(Op("OP-102", 9.5m, "Cliente B"));
            gestor.Atender(hay_senal: false); gestor.Atender(hay_senal: false);
            Afirmar(gestor.Reintentos.Cantidad == 2, "2 pendientes en el anillo");
            List<string> lista_eventos = gestor.Reintentar(operacion => operacion.Codigo == "OP-102" && operacion.Intentos >= 2);
            Operacion op_a = gestor.BuscarPorCodigo("OP-101")!;
            Operacion op_b = gestor.BuscarPorCodigo("OP-102")!;
            Afirmar(op_a.Estado == EstadoOperacion.Fallida && op_a.Intentos == 3, "OP-101 fallida con 3 intentos (se detiene)");
            Afirmar(op_b.Estado == EstadoOperacion.Procesada && op_b.Intentos == 2, "OP-102 procesada en el intento 2");
            Afirmar(gestor.Reintentos.EstaVacia, "el anillo termina vacío (no hay bucle infinito)");
            Afirmar(lista_eventos.Count > 0, "se registraron eventos");
            foreach (string evento in lista_eventos) Escribir("        " + evento);
        });

        Caso("6. Reversa del último abono (LIFO)", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            gestor.Llegar(Op("OP-201", 10m)); gestor.Llegar(Op("OP-202", 95m, "Cliente 02"));
            gestor.Atender(); gestor.Atender();
            Afirmar(gestor.Abonos.Cima().Codigo == "OP-202", "la cima es el último abono");
            Operacion operacion_revertida = gestor.RevertirUltimoAbono();
            Afirmar(operacion_revertida.Codigo == "OP-202" && operacion_revertida.Estado == EstadoOperacion.Revertida, "se revirtió OP-202");
            Afirmar(gestor.Abonos.Cima().Codigo == "OP-201" && gestor.Abonos.Cima().Estado == EstadoOperacion.Procesada, "OP-201 intacta");
            gestor.RevertirUltimoAbono();
            bool lanzo_excepcion = false;
            try { gestor.RevertirUltimoAbono(); } catch (InvalidOperationException) { lanzo_excepcion = true; }
            Afirmar(lanzo_excepcion, "revertir con pila vacía debe avisar");
        });

        Caso("7. Búsqueda de una operación inexistente", () =>
        {
            GestorTransacciones gestor = GestorConCampo();
            while (!gestor.Llegadas.EstaVacia) gestor.Atender();
            Afirmar(gestor.BuscarPorCodigo("OP-9999") == null, "OP-9999 no existe -> null");
            Afirmar(gestor.BuscarPorCodigo("op-005")?.Monto == 20.50m, "OP-005 existe (sin distinguir mayúsculas)");
        });

        Caso("8. Control de datos inválidos", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            void Rechaza(Operacion operacion_a_validar, string motivo) =>
                Afirmar(gestor.Llegar(operacion_a_validar).Count > 0, "debía rechazar: " + motivo);
            Rechaza(Op("OP-301", -5m), "monto negativo");
            Rechaza(Op("OP-302", 0m), "monto cero");
            Rechaza(Op("OP-303", 501m), "monto sobre el tope");
            Rechaza(Op("", 5m), "código vacío");
            Rechaza(Op("ABC-1", 5m), "código con formato malo");
            Rechaza(Op("OP-304", 5m, ""), "origen vacío");
            Rechaza(Op("OP-305", 5m, "Cliente 01", ""), "destino vacío");
            Rechaza(Op("OP-306", 5m, "Bodega Chabelita", "Bodega Chabelita"), "origen igual al destino");
            Operacion operacion_futura = Op("OP-307"); operacion_futura.FechaHora = Ahora().AddDays(1);
            Rechaza(operacion_futura, "fecha futura");
            Afirmar(gestor.Llegadas.EstaVacia, "ninguna operación inválida entró a la cola");
            gestor.Llegar(Op("OP-308"));
            Rechaza(Op("OP-308"), "código duplicado");
            Afirmar(gestor.Llegadas.Cantidad == 1, "solo quedó la válida");
        });

        Caso("9. Cola: encolar, consultar frente y desencolar por orden de llegada", () =>
        {
            Cola<Operacion> cola = new Cola<Operacion>();
            cola.Enqueue(Op("OP-401")); cola.Enqueue(Op("OP-402")); cola.Enqueue(Op("OP-403"));
            Afirmar(cola.Peek().Codigo == "OP-401" && cola.Cantidad == 3, "el frente es el primero y no se saca");
            Afirmar(cola.Dequeue().Codigo == "OP-401", "sale OP-401");
            Afirmar(cola.Dequeue().Codigo == "OP-402", "sale OP-402");
            Afirmar(cola.Peek().Codigo == "OP-403", "frente ahora OP-403");
            cola.Dequeue();
            bool lanzo_excepcion = false;
            try { cola.Dequeue(); } catch (InvalidOperationException) { lanzo_excepcion = true; }
            Afirmar(lanzo_excepcion && cola.EstaVacia, "desencolar vacía avisa");
        });

        Caso("Extra. Lista circular: el último vuelve al primero", () =>
        {
            ListaCircular<Operacion> anillo = new ListaCircular<Operacion>();
            anillo.Agregar(Op("OP-501")); anillo.Agregar(Op("OP-502")); anillo.Agregar(Op("OP-503"));
            anillo.Avanzar(); anillo.Avanzar(); anillo.Avanzar();
            Afirmar(anillo.Actual!.Codigo == "OP-501", "tras 3 avances vuelve a OP-501");
            anillo.RemoverActual();
            Afirmar(anillo.Cantidad == 2 && anillo.Actual!.Codigo == "OP-502", "al quitar OP-501 queda OP-502");
        });

        Escribir(new string('-', 70));
        Escribir($"Resultado: {Ok} OK, {Falla} con falla.");
        return reporte.ToString();
    }
}