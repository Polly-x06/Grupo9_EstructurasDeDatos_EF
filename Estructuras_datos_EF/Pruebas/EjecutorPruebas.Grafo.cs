using Estructuras_datos_EP.Datos;
using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using Estructuras_datos_EP.Servicios;
using System.Text;

namespace Estructuras_datos_EP.Pruebas;

public partial class EjecutorPruebas
{
    private static Grafo GrafoEjemplo()
    {
        Distribuidor[] vertices =
        {
            new Distribuidor { Nombre = "A" }, new Distribuidor { Nombre = "B" }, new Distribuidor { Nombre = "C" },
            new Distribuidor { Nombre = "D" }, new Distribuidor { Nombre = "E" }
        };
        Grafo grafo = new Grafo(vertices);
        grafo.AgregarArista(0, 1, 4);
        grafo.AgregarArista(0, 2, 2);
        grafo.AgregarArista(1, 2, 1);
        grafo.AgregarArista(1, 3, 5);
        grafo.AgregarArista(2, 3, 8);
        grafo.AgregarArista(3, 4, 3);
        return grafo;
    }

    // X-Y (7 km) y Z aislado.
    private static Grafo GrafoPartido()
    {
        Grafo grafo = new Grafo(new[]
        {
            new Distribuidor { Nombre = "X" }, new Distribuidor { Nombre = "Y" }, new Distribuidor { Nombre = "Z" }
        });
        grafo.AgregarArista(0, 1, 7);
        return grafo;
    }

    private static string Camino(Grafo grafo, int[]? ruta) =>
        ruta == null ? "sin ruta" : string.Join("", ruta.Select(indice => grafo.Vertice(indice).Nombre));

    private void CasosGrafo()
    {
        Caso("19. Grafo: matriz y lista de adyacencia (5 vértices, 6 conexiones)", () =>
        {
            Grafo grafo = GrafoEjemplo();
            Afirmar(grafo.Cantidad == 5 && grafo.CantidadAristas == 6, "5 vértices y 6 conexiones");
            Afirmar(grafo.Peso(0, 1) == 4 && grafo.Peso(1, 0) == 4, "la matriz es simétrica (A-B = 4)");
            Afirmar(grafo.Peso(2, 2) == 0, "la diagonal vale 0");
            Afirmar(double.IsPositiveInfinity(grafo.Peso(0, 3)), "A y D no tienen conexión directa (infinito)");
            Afirmar(grafo.Vecinos(0).Cantidad == 2 && grafo.Vecinos(3).Cantidad == 3 && grafo.Vecinos(4).Cantidad == 1, "vecinos: A=2, D=3, E=1");
            int suma = 0;
            for (int i = 0; i < grafo.Cantidad; i++) suma += grafo.Vecinos(i).Cantidad;
            Afirmar(suma == 12, "la lista guarda cada conexión dos veces (2 × 6 = 12)");
            Afirmar(!grafo.AgregarArista(0, 1, 9), "rechaza una conexión repetida");
            Afirmar(!grafo.AgregarArista(2, 2, 1), "rechaza una conexión de un punto consigo mismo");
            Afirmar(!grafo.AgregarArista(0, 4, -3), "rechaza una distancia negativa");
            Afirmar(grafo.CantidadAristas == 6, "ninguna conexión inválida se agregó");
        });

        Caso("20. Floyd-Warshall: distancias mínimas y rutas (calculadas a mano)", () =>
        {
            Grafo grafo = GrafoEjemplo();
            grafo.Floyd();
            Afirmar(grafo.Distancia(0, 1) == 3, "A-B = 3 (por C; el directo mide 4)");
            Afirmar(grafo.Distancia(0, 3) == 8, "A-D = 8");
            Afirmar(grafo.Distancia(0, 4) == 11, "A-E = 11");
            Afirmar(grafo.Distancia(1, 4) == 8, "B-E = 8");
            Afirmar(grafo.Distancia(2, 3) == 6, "C-D = 6 (por B; el directo mide 8)");
            Afirmar(grafo.Distancia(2, 4) == 9, "C-E = 9");
            Afirmar(grafo.Distancia(3, 0) == 8, "la distancia es simétrica (D-A = 8)");
            Afirmar(grafo.Distancia(3, 3) == 0, "un punto a sí mismo = 0");
            Afirmar(Camino(grafo, grafo.Ruta(0, 4)) == "ACBDE", "ruta A→E = A-C-B-D-E");
            Afirmar(Camino(grafo, grafo.Ruta(1, 0)) == "BCA", "ruta B→A = B-C-A");
            Afirmar(Camino(grafo, grafo.Ruta(2, 2)) == "C", "ruta de un punto a sí mismo");

            Grafo partido = GrafoPartido();
            Afirmar(double.IsPositiveInfinity(partido.Distancia(0, 2)) && partido.Ruta(0, 2) == null,
                    "sin conexión: distancia infinita y sin ruta");
        });

        Caso("21. Prim: árbol de expansión mínima (calculado a mano)", () =>
        {
            Grafo grafo = GrafoEjemplo();
            ResultadoPrim resultado = grafo.Prim(0);
            Afirmar(resultado.Conexo && resultado.Aristas.Cantidad == 4, "5 puntos → 4 conexiones");
            Afirmar(resultado.Total == 11, "total 11 km (1 + 2 + 3 + 5)");
            Afirmar(string.Join(",", resultado.Aristas.Select(arista => arista.Km)) == "2,1,5,3", "orden de elección desde A: 2,1,5,3");
            Afirmar(grafo.Prim(4).Total == 11, "empezando en E el total es el mismo");

            ResultadoPrim parcial = GrafoPartido().Prim(0);
            Afirmar(!parcial.Conexo && parcial.Total == 7 && parcial.Aristas.Cantidad == 1,
                    "grafo no conexo: avisa y cubre solo lo alcanzable");
        });

        Caso("22. Plan de visita óptimo con Floyd (calculado a mano)", () =>
        {
            Grafo grafo = GrafoEjemplo();
            int[] visitar = { 1, 3, 4 };  
            PlanVisita ida = grafo.PlanOptimo(0, visitar, false)!;
            Afirmar(ida.Km == 11 && Camino(grafo, ida.Orden) == "ABDE", "sin regresar: A-B-D-E = 11 km");
            PlanVisita ida_y_vuelta = grafo.PlanOptimo(0, visitar, true)!;
            Afirmar(ida_y_vuelta.Km == 22 && ida_y_vuelta.Orden.Length == 5
                    && ida_y_vuelta.Orden[0] == 0 && ida_y_vuelta.Orden[4] == 0, "ida y vuelta: 22 km");
            Afirmar(grafo.PlanOptimo(0, new int[0], true)!.Km == 0, "sin paradas: 0 km");
            Afirmar(GrafoPartido().PlanOptimo(0, new[] { 2 }, false) == null, "parada inalcanzable: no hay plan");
        });

        Caso("23. Carga desde CSV (; o ,  /  decimales con coma  /  minutos opcionales)", () =>
        {
            string carpeta = Path.Combine(Path.GetTempPath(), "ef_csv_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta);
            try
            {
                string ruta_d = Path.Combine(carpeta, "d.csv");
                string ruta_c = Path.Combine(carpeta, "c.csv");
                File.WriteAllText(ruta_d, "nombre\r\nX\r\nY\r\nZ\r\n", Encoding.UTF8);
                File.WriteAllText(ruta_c, "origen;destino;km;minutos\r\nX;Y;2,5;10\r\nY;Z;4;\r\n", Encoding.UTF8);
                Grafo grafo = CargadorCsv.Cargar(ruta_d, ruta_c);
                Afirmar(grafo.Cantidad == 3 && grafo.CantidadAristas == 2, "3 puntos y 2 conexiones");
                Afirmar(grafo.Peso(0, 1) == 2.5 && grafo.Peso(1, 2) == 4, "lee el decimal con coma (2,5)");
                Afirmar(grafo.Distancia(0, 2) == 6.5, "Floyd X-Z = 2.5 + 4 = 6.5");
                Afirmar(grafo.Vecinos(0).Buscar(arista => arista.Destino == 1)!.Minutos == 10
                        && grafo.Vecinos(1).Buscar(arista => arista.Destino == 2)!.Minutos == null, "minutos opcionales");

                File.WriteAllText(ruta_c, "origen,destino,km\r\nX,Y,\"3,5\"\r\n", Encoding.UTF8);
                Afirmar(CargadorCsv.Cargar(ruta_d, ruta_c).Peso(0, 1) == 3.5, "separador coma con el decimal entre comillas");

                File.WriteAllText(ruta_c, "origen;destino;km\r\nX;W;4\r\n", Encoding.UTF8);
                bool rechazo = false;
                try { CargadorCsv.Cargar(ruta_d, ruta_c); } catch (InvalidDataException) { rechazo = true; }
                Afirmar(rechazo, "un distribuidor desconocido en conexiones.csv se rechaza con mensaje");
            }
            finally { Directory.Delete(carpeta, true); }
        });
    }

    private void CasosDeudas()
    {
        Caso("24. Compras fiadas: todos los distribuidores menos Makro", () =>
        {
            ControlDeudas deudas = new ControlDeudas(Ahora);
            Distribuidor makro = new Distribuidor { Nombre = "Makro" };
            Distribuidor norte = new Distribuidor { Nombre = "Distribuidora Norte" };
            Afirmar(!makro.AceptaFiado && norte.AceptaFiado, "solo Makro no acepta fiado");
            Afirmar(deudas.ComprarFiado(makro, 50m) != null, "Makro rechaza la compra fiada");
            Afirmar(deudas.ComprarFiado(norte, 0m) != null, "monto 0 se rechaza");
            Afirmar(deudas.ComprarFiado(norte, 120.50m) == null && deudas.ComprarFiado(norte, 30m) == null, "dos compras válidas");
            Afirmar(deudas.TotalFiado(norte) == 150.50m && deudas.TotalFiado(makro) == 0m && deudas.Compras.Cantidad == 2,
                    "total fiado 150.50 y nada a Makro");
        });

        Caso("25. Pago de deuda: pasa por cola, reintentos y pila (y se puede revertir)", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            ControlDeudas deudas = new ControlDeudas(Ahora);
            Distribuidor norte = new Distribuidor { Nombre = "Distribuidora Norte" };
            deudas.ComprarFiado(norte, 100m);

            Afirmar(deudas.PagarDeuda(gestor, norte, 101m, out _).Count > 0, "no se puede pagar más de lo que se debe");
            List<string> errores = deudas.PagarDeuda(gestor, norte, 40m, out Operacion? pago);
            Afirmar(errores.Count == 0 && pago != null && gestor.Llegadas.Cantidad == 1, "el pago entra a la cola");
            Afirmar(deudas.EnTramite(gestor, norte) == 40m && deudas.Saldo(gestor, norte) == 60m, "en trámite 40; saldo 60");

            gestor.Atender(false);
            Afirmar(gestor.Reintentos.Cantidad == 1 && deudas.Pagado(gestor, norte) == 0m, "sin señal: va a reintentos y aún no cuenta como pagado");
            gestor.Reintentar(operacion => true);
            Afirmar(pago!.Estado == EstadoOperacion.Procesada && deudas.Pagado(gestor, norte) == 40m && deudas.Saldo(gestor, norte) == 60m,
                    "el reintento procesa el pago: pagado 40, saldo 60");
            Afirmar(gestor.Abonos.Cantidad == 1, "el pago procesado queda en la pila");

            gestor.RevertirUltimoAbono();
            Afirmar(deudas.Pagado(gestor, norte) == 0m && deudas.Saldo(gestor, norte) == 100m, "al revertir, la deuda vuelve a 100");

            Distribuidor makro = new Distribuidor { Nombre = "Makro" };
            Afirmar(deudas.PagarDeuda(gestor, makro, 10m, out _).Count > 0, "Makro no tiene deuda que pagar");
        });

        Caso("26. Datos simulados: 40 operaciones por cola, reintentos y pila", () =>
        {
            GestorTransacciones gestor = NuevoGestor();
            List<Operacion> operaciones = Simulacion.Generar(40, 2026, 1);
            List<string> errores = gestor.CargarLlegadas(operaciones);
            Afirmar(operaciones.Count == 40 && errores.Count == 0, "las 40 son válidas: " + string.Join(" / ", errores));
            Afirmar(gestor.Llegadas.Cantidad == 40 && Codigos(gestor.Llegadas).StartsWith("OP-001,OP-002"), "entran a la cola en orden FIFO");

            Simulacion.AtenderTodo(gestor, 5);
            Afirmar(gestor.Llegadas.EstaVacia && gestor.Reintentos.EstaVacia, "cola y reintentos quedan vacíos");
            Afirmar(gestor.Registro.Cantidad == 40 && gestor.Historial.Cantidad == 40 && gestor.Indice.Cantidad == 40,
                    "40 en el registro, el historial y el árbol");
            Afirmar(gestor.Registro.All(operacion => operacion.Estado == EstadoOperacion.Procesada), "todas quedan procesadas");
            Afirmar(gestor.Registro.Count(operacion => operacion.Intentos == 2) == 8, "las 8 sin señal (1 de cada 5) se procesan en el intento 2");
            int abonos = gestor.Registro.Count(operacion => operacion.EsAbono);
            Afirmar(gestor.Abonos.Cantidad == abonos && abonos > 0, "la pila tiene un abono por cada operación que es abono");
            Operacion ultimo = gestor.RevertirUltimoAbono();
            Afirmar(ultimo.Estado == EstadoOperacion.Revertida && gestor.Abonos.Cantidad == abonos - 1, "se revierte el último abono");
        });

        Caso("27. Volumen de 10 000 operaciones: lista simple vs árbol", () =>
        {
            ResultadoVolumen resultado = Simulacion.ProbarVolumen(10000, 7);
            Afirmar(resultado.Operaciones == 10000, "se registraron las 10 000");
            Afirmar(resultado.ComparacionesLista == 10000, "la lista recorre las 10 000 (la buscada es la última en llegar)");
            Afirmar(resultado.ComparacionesArbol < 100, "el árbol necesita menos de 100 comparaciones");
            Afirmar(resultado.AlturaArbol < 100, "el árbol no se degeneró (altura menor a 100)");
            Escribir("        " + resultado.Resumen);
        });
    }
}