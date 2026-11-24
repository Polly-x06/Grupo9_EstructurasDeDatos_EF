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
    private static ArbolBinarioBusqueda<Operacion> NuevoArbol() =>
    new ArbolBinarioBusqueda<Operacion>(operacion => operacion.Numero);

    private static ArbolBinarioBusqueda<Operacion> ArbolDeEjemplo()
    {
        ArbolBinarioBusqueda<Operacion> arbol = NuevoArbol();
        foreach (int numero in new[] { 50, 30, 70, 20, 40, 60, 80 }) arbol.Insertar(Op($"OP-{numero:000}"));
        return arbol;
    }

    private static string Codigos(IEnumerable<Operacion> operaciones) =>
        string.Join(",", operaciones.Select(operacion => operacion.Codigo));

    public string Correr()
    {
        reporte.Clear(); Ok = 0; Falla = 0;
        Escribir("PRUEBAS - Estructuras de Datos (EF) - Bodega Chabelita");
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
        Caso("10. Árbol: insertar, buscar y recorridos (inorden, preorden, postorden)", () =>
        {
            ArbolBinarioBusqueda<Operacion> arbol = ArbolDeEjemplo();
            Afirmar(arbol.Cantidad == 7 && arbol.Altura() == 3, "7 nodos y altura 3");
            Afirmar(Codigos(arbol.InOrden()) == "OP-020,OP-030,OP-040,OP-050,OP-060,OP-070,OP-080", "inorden queda ordenado por código");
            Afirmar(Codigos(arbol.PreOrden()) == "OP-050,OP-030,OP-020,OP-040,OP-070,OP-060,OP-080", "preorden");
            Afirmar(Codigos(arbol.PostOrden()) == "OP-020,OP-040,OP-030,OP-060,OP-080,OP-070,OP-050", "postorden");
            Afirmar(arbol.Buscar(40)?.Codigo == "OP-040", "se encuentra OP-040");
            Afirmar(arbol.Comparaciones == 3, "OP-040 se halla en 3 comparaciones (50, 30, 40)");
            Afirmar(arbol.Buscar(45) == null, "OP-045 no existe -> null");
            Afirmar(!arbol.Insertar(Op("OP-050")), "no se permiten claves repetidas");
            Afirmar(arbol.Cantidad == 7, "la cantidad no cambió");
        });

        Caso("11. Árbol: eliminar (hoja, un hijo, dos hijos, raíz) y árbol vacío", () =>
        {
            ArbolBinarioBusqueda<Operacion> arbol = ArbolDeEjemplo();
            Afirmar(arbol.Eliminar(20), "elimina una hoja (OP-020)");
            Afirmar(Codigos(arbol.InOrden()) == "OP-030,OP-040,OP-050,OP-060,OP-070,OP-080", "sin OP-020");
            Afirmar(arbol.Eliminar(30), "elimina un nodo con un hijo (OP-030)");
            Afirmar(arbol.Raiz!.Izquierdo!.Dato.Codigo == "OP-040", "OP-040 ocupa el lugar de OP-030");
            Afirmar(arbol.Eliminar(70), "elimina un nodo con dos hijos (OP-070)");
            Afirmar(Codigos(arbol.InOrden()) == "OP-040,OP-050,OP-060,OP-080", "sin OP-070 y con el orden intacto");
            Afirmar(arbol.Eliminar(50), "elimina la raíz con dos hijos");
            Afirmar(arbol.Raiz!.Dato.Codigo == "OP-060", "el sucesor inorden (OP-060) pasa a ser la raíz");
            Afirmar(!arbol.Eliminar(999), "eliminar un código inexistente devuelve false");
            Afirmar(arbol.Cantidad == 3, "quedan 3 nodos");
            arbol.Eliminar(40); arbol.Eliminar(60); arbol.Eliminar(80);
            Afirmar(arbol.EstaVacio && arbol.Raiz == null && arbol.Cantidad == 0, "el árbol queda vacío");
            Afirmar(!arbol.Eliminar(40) && arbol.Buscar(40) == null, "operar sobre un árbol vacío no falla");
            Afirmar(arbol.InOrden().Cantidad == 0 && arbol.PreOrden().Cantidad == 0 && arbol.PostOrden().Cantidad == 0, "recorridos vacíos");
            Afirmar(arbol.Altura() == 0 && arbol.AsignarDisposicion() == 0, "altura 0");
        });

        Caso("12. Árbol: degeneración con códigos ascendentes y efecto del orden de inserción", () =>
        {
            ArbolBinarioBusqueda<Operacion> ascendente = NuevoArbol();
            for (int numero = 1; numero <= 15; numero++) ascendente.Insertar(Op($"OP-{numero:000}"));
            Afirmar(ascendente.Altura() == 15, "con códigos ascendentes el árbol es una cadena (altura = n)");
            ascendente.Buscar(15);
            Afirmar(ascendente.Comparaciones == 15, "buscar el último cuesta n comparaciones, como en una lista");

            ArbolBinarioBusqueda<Operacion> equilibrado = NuevoArbol();
            foreach (int numero in new[] { 8, 4, 12, 2, 6, 10, 14, 1, 3, 5, 7, 9, 11, 13, 15 }) equilibrado.Insertar(Op($"OP-{numero:000}"));
            Afirmar(equilibrado.Altura() == 4, "con otro orden de inserción la altura baja a 4");
            equilibrado.Buscar(15);
            Afirmar(equilibrado.Comparaciones == 4, "buscar el mismo código cuesta 4 comparaciones");
        });

        Caso("13. Flujo: el árbol se llena al atender y la consulta por código lo usa", () =>
        {
            GestorTransacciones gestor = GestorConCampo();
            Afirmar(gestor.Indice.EstaVacio, "mientras están en la cola todavía no entran al árbol");
            Afirmar(gestor.BuscarPorCodigo("OP-005") != null, "una operación aún en la cola se encuentra igual");
            while (!gestor.Llegadas.EstaVacia) gestor.Atender();
            Afirmar(gestor.Indice.Cantidad == 12, "las 12 operaciones atendidas están en el árbol");
            Afirmar(gestor.BuscarPorCodigo("op-005")?.Monto == 20.50m, "OP-005 se encuentra por el árbol (sin distinguir mayúsculas)");
            Afirmar(gestor.Indice.Comparaciones == 5, "OP-005 cuesta 5 comparaciones (códigos ascendentes forman una cadena)");
            Afirmar(gestor.BuscarPorCodigo("OP-9999") == null, "un código inexistente devuelve null");
            Afirmar(gestor.BuscarPorCodigo("basura") == null, "un texto que no es código devuelve null sin fallar");
            (Operacion? encontrada, int en_lista, int en_arbol) = gestor.CompararBusqueda("OP-012");
            Afirmar(encontrada?.Codigo == "OP-012" && en_lista == 12 && en_arbol == 12, "con códigos ascendentes lista y árbol cuestan lo mismo (12)");
        });

        Caso("14. Árbol: la clave es el número del código (OP-999 va antes que OP-1000)", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            Afirmar(gestor.Llegar(Op("OP-1000")).Count == 0 && gestor.Llegar(Op("OP-999")).Count == 0, "ambos códigos se aceptan");
            gestor.Atender(); gestor.Atender();
            Afirmar(Codigos(gestor.Indice.InOrden()) == "OP-999,OP-1000", "inorden por número, no por texto");
            Afirmar(gestor.Llegar(Op("OP-0999")).Count > 0, "OP-0999 es el mismo número que OP-999: se rechaza como duplicado");
        });

        Caso("15. Cola y pila: limpiar (también en estructuras vacías)", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            Afirmar(gestor.LimpiarCola() == 0 && gestor.LimpiarPila() == 0, "limpiar estructuras vacías no falla");
            gestor.Llegar(Op("OP-801")); gestor.Llegar(Op("OP-802")); gestor.Llegar(Op("OP-803"));
            gestor.Atender(); gestor.Atender();
            Afirmar(gestor.LimpiarCola() == 1 && gestor.Llegadas.EstaVacia, "se descarta 1 operación pendiente (OP-803)");
            Afirmar(gestor.Registro.Cantidad == 2, "las ya atendidas siguen registradas");
            Afirmar(gestor.LimpiarPila() == 2 && gestor.Abonos.EstaVacia, "se confirman 2 abonos");
            bool lanzo_excepcion = false;
            try { gestor.RevertirUltimoAbono(); } catch (InvalidOperationException) { lanzo_excepcion = true; }
            Afirmar(lanzo_excepcion, "con la pila limpia ya no hay nada que revertir");
            Afirmar(gestor.Registro.All(operacion => operacion.Estado == EstadoOperacion.Procesada), "los abonos confirmados siguen Procesados");

            Cola<Operacion> cola = new Cola<Operacion>();
            cola.Enqueue(Op("OP-804")); cola.Limpiar();
            Afirmar(cola.EstaVacia && cola.Cantidad == 0, "Cola.Limpiar deja la cola vacía");
            cola.Enqueue(Op("OP-805"));
            Afirmar(cola.Peek().Codigo == "OP-805", "la cola sigue funcionando tras limpiarla");
            Pila<Operacion> pila = new Pila<Operacion>();
            pila.Apilar(Op("OP-806")); pila.Limpiar();
            Afirmar(pila.EstaVacia && pila.Cantidad == 0, "Pila.Limpiar deja la pila vacía");
        });

        Caso("16. Anular operación (Fallida o Revertida): registro, historial y árbol consistentes", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            gestor.Llegar(Op("OP-601", 10m)); gestor.Llegar(Op("OP-602", 20m)); gestor.Llegar(Op("OP-603", 30m));
            gestor.Atender();
            gestor.Atender(hay_senal: false);
            gestor.Atender();
            gestor.Reintentar(operacion => false);
            gestor.RevertirUltimoAbono();
            Afirmar(gestor.BuscarPorCodigo("OP-602")!.Estado == EstadoOperacion.Fallida, "OP-602 quedó Fallida");
            Afirmar(gestor.BuscarPorCodigo("OP-603")!.Estado == EstadoOperacion.Revertida, "OP-603 quedó Revertida");

            bool avisa = false;
            try { gestor.AnularOperacion("OP-601"); } catch (InvalidOperationException) { avisa = true; }
            Afirmar(avisa, "una operación Procesada no se anula (se revierte)");
            avisa = false;
            try { gestor.AnularOperacion("OP-999"); } catch (InvalidOperationException) { avisa = true; }
            Afirmar(avisa, "anular un código inexistente avisa");

            Afirmar(gestor.AnularOperacion("op-602").Codigo == "OP-602", "se anula la Fallida");
            Afirmar(gestor.Registro.Cantidad == 2 && gestor.Historial.Cantidad == 2 && gestor.Indice.Cantidad == 2, "sale del registro, del historial y del árbol");
            Afirmar(gestor.BuscarPorCodigo("OP-602") == null, "ya no se encuentra");
            Afirmar(gestor.AnularOperacion("OP-603").Estado == EstadoOperacion.Revertida, "se anula la Revertida");
            Afirmar(gestor.Historial.Actual?.Codigo == "OP-601", "el historial queda apuntando a una operación vigente");
            Afirmar(gestor.Llegar(Op("OP-602")).Count == 0, "el código anulado vuelve a estar disponible");
        });

        Caso("17. Lista doble: eliminar medio, cabeza y cola sin romper la navegación", () =>
        {
            ListaDoble<Operacion> lista = new ListaDoble<Operacion>();
            foreach (string codigo in new[] { "OP-701", "OP-702", "OP-703", "OP-704" }) lista.AgregarFinal(Op(codigo));
            Afirmar(lista.Eliminar(operacion => operacion.Codigo == "OP-702"), "elimina un nodo del medio");
            Afirmar(lista.Eliminar(operacion => operacion.Codigo == "OP-701"), "elimina la cabeza");
            lista.IrAlInicio();
            Afirmar(lista.Actual!.Codigo == "OP-703", "la nueva cabeza es OP-703");
            lista.IrAlFinal();
            Afirmar(lista.Actual!.Codigo == "OP-704", "el actual es la cola");
            Afirmar(lista.Eliminar(operacion => operacion.Codigo == "OP-704"), "elimina la cola que era el actual");
            Afirmar(lista.Actual!.Codigo == "OP-703" && lista.Cantidad == 1, "el actual retrocede a OP-703");
            Afirmar(!lista.Eliminar(operacion => operacion.Codigo == "OP-999"), "eliminar un inexistente devuelve false");
            Afirmar(lista.Eliminar(operacion => operacion.Codigo == "OP-703") && lista.EstaVacia && lista.Actual == null, "al quitar el último queda vacía");
        });

        Caso("18. Lista simple: eliminar cabeza, medio, cola e inexistente", () =>
        {
            ListaSimple<Operacion> lista = new ListaSimple<Operacion>();
            foreach (string codigo in new[] { "OP-751", "OP-752", "OP-753", "OP-754" }) lista.AgregarFinal(Op(codigo));
            Afirmar(lista.Eliminar(operacion => operacion.Codigo == "OP-751") && lista.Cantidad == 3, "elimina la cabeza");
            Afirmar(lista.Eliminar(operacion => operacion.Codigo == "OP-753") && lista.Cantidad == 2, "elimina un nodo del medio");
            Afirmar(lista.Eliminar(operacion => operacion.Codigo == "OP-754") && lista.Cantidad == 1, "elimina la cola");
            lista.AgregarFinal(Op("OP-755"));
            Afirmar(Codigos(lista) == "OP-752,OP-755", "tras eliminar la cola, agregar al final sigue funcionando");
            Afirmar(!lista.Eliminar(operacion => operacion.Codigo == "OP-999"), "eliminar un inexistente devuelve false");
            lista.Eliminar(operacion => operacion.Codigo == "OP-752"); lista.Eliminar(operacion => operacion.Codigo == "OP-755");
            Afirmar(lista.EstaVacia && lista.Cantidad == 0, "queda vacía");
            Afirmar(!lista.Eliminar(operacion => true), "eliminar en una lista vacía devuelve false");
        });

        Escribir(new string('-', 70));
        Escribir($"Resultado: {Ok} OK, {Falla} con falla.");
        return reporte.ToString();
    }
}