using Estructuras_datos_EP.Modelos;
using System.Diagnostics;

namespace Estructuras_datos_EP.Servicios;

public class ResultadoVolumen
{
    public int Operaciones { get; init; }
    public long Milisegundos { get; init; }
    public int AlturaArbol { get; init; }
    public int Abonos { get; init; }
    public string CodigoBuscado { get; init; } = "";
    public int ComparacionesLista { get; init; }
    public int ComparacionesArbol { get; init; }

    public string Resumen =>
        $"{Operaciones} operaciones en {Milisegundos} ms; buscar {CodigoBuscado}: lista simple {ComparacionesLista} comparaciones, árbol {ComparacionesArbol}.";

    public override string ToString() =>
        $"PRUEBA DE VOLUMEN: {Operaciones} operaciones\r\n" +
        $"Tiempo (cargar, atender y registrar todo): {Milisegundos} ms\r\n" +
        $"Altura del árbol: {AlturaArbol}   |   Abonos en la pila: {Abonos}\r\n" +
        $"Búsqueda de {CodigoBuscado} (la última en llegar):\r\n" +
        $"   lista simple: {ComparacionesLista} comparaciones\r\n" +
        $"   árbol binario de búsqueda: {ComparacionesArbol} comparaciones";
}

public static class Simulacion
{
    private static Operacion Crear(Random azar, int numero, DateTime fecha)
    {
        int t = azar.Next(10);
        TipoOperacion tipo = t < 6 ? TipoOperacion.PagoQR
                           : t < 8 ? TipoOperacion.Transferencia
                           : t < 9 ? TipoOperacion.Abono
                           : TipoOperacion.Recarga;
        return new Operacion
        {
            Codigo = $"OP-{numero:000}",
            FechaHora = fecha,
            Tipo = tipo,
            Monto = azar.Next(100, 20001) / 100m,   // de S/ 1.00 a S/ 200.00
            Origen = $"Cliente {azar.Next(1, 31):00}",
            Destino = ControlDeudas.Bodega
        };
    }

    public static List<Operacion> Generar(int cantidad, int semilla, int primer_numero)
    {
        Random azar = new Random(semilla);
        DateTime inicio = new DateTime(2026, 10, 12, 18, 0, 0);
        List<Operacion> lista = new List<Operacion>(cantidad);
        for (int i = 0; i < cantidad; i++) lista.Add(Crear(azar, primer_numero + i, inicio.AddMinutes(i * 2)));
        return lista;
    }

    public static List<Operacion> GenerarMezclado(int cantidad, int semilla)
    {
        Random azar = new Random(semilla);
        int[] numeros = new int[cantidad];
        for (int i = 0; i < cantidad; i++) numeros[i] = i + 1;
        for (int i = cantidad - 1; i > 0; i--)
        {
            int j = azar.Next(i + 1);
            (numeros[i], numeros[j]) = (numeros[j], numeros[i]);
        }
        DateTime inicio = new DateTime(2026, 10, 1, 8, 0, 0);
        List<Operacion> lista = new List<Operacion>(cantidad);
        for (int i = 0; i < cantidad; i++) lista.Add(Crear(azar, numeros[i], inicio.AddMinutes(i)));
        return lista;
    }

    public static List<string> AtenderTodo(GestorTransacciones gestor, int cada_n_sin_senal = 5)
    {
        int atendidas = 0, sin_senal = 0;
        while (!gestor.Llegadas.EstaVacia)
        {
            atendidas++;
            bool hay_senal = cada_n_sin_senal <= 0 || atendidas % cada_n_sin_senal != 0;
            if (!hay_senal) sin_senal++;
            gestor.Atender(hay_senal);
        }
        List<string> eventos = gestor.Reintentar(operacion => operacion.Intentos >= 2);

        List<string> resumen = new List<string>();
        resumen.Add($"Atendidas {atendidas} operaciones; {sin_senal} sin señal pasaron a reintentos.");
        resumen.Add($"Reintentos: {eventos.Count} eventos; quedan {gestor.Reintentos.Cantidad} pendientes.");
        resumen.Add($"Abonos en la pila (se pueden revertir): {gestor.Abonos.Cantidad}.");
        return resumen;
    }

    public static ResultadoVolumen ProbarVolumen(int cantidad = 10000, int semilla = 7)
    {
        DateTime ahora = new DateTime(2026, 12, 31);
        GestorTransacciones gestor = new GestorTransacciones(() => ahora);
        List<Operacion> operaciones = GenerarMezclado(cantidad, semilla);
        string ultimo_codigo = operaciones[operaciones.Count - 1].Codigo;

        Stopwatch cronometro = Stopwatch.StartNew();
        List<string> errores = gestor.CargarLlegadas(operaciones);
        if (errores.Count > 0) throw new InvalidOperationException("La carga masiva tuvo errores: " + errores[0]);
        while (!gestor.Llegadas.EstaVacia) gestor.Atender(true);
        cronometro.Stop();

        (Operacion? encontrada, int en_lista, int en_arbol) = gestor.CompararBusqueda(ultimo_codigo);
        if (encontrada == null) throw new InvalidOperationException("No se encontró " + ultimo_codigo + " tras la carga masiva.");

        return new ResultadoVolumen
        {
            Operaciones = gestor.Registro.Cantidad,
            Milisegundos = cronometro.ElapsedMilliseconds,
            AlturaArbol = gestor.Indice.Altura(),
            Abonos = gestor.Abonos.Cantidad,
            CodigoBuscado = ultimo_codigo,
            ComparacionesLista = en_lista,
            ComparacionesArbol = en_arbol
        };
    }
}