using Estructuras_datos_EP.Modelos;
using System.Globalization;
using System.Text;

namespace Estructuras_datos_EP.Estructuras;

public class Arista
{
    public int Origen { get; }
    public int Destino { get; }
    public double Km { get; }
    public double? Minutos { get; }
    public Arista(int origen, int destino, double km, double? minutos = null)
    {
        Origen = origen; Destino = destino; Km = km; Minutos = minutos;
    }
}

public class ResultadoPrim
{
    public ListaSimple<Arista> Aristas { get; }
    public double Total { get; }
    public bool Conexo { get; }
    public ResultadoPrim(ListaSimple<Arista> aristas, double total, bool conexo)
    {
        Aristas = aristas; Total = total; Conexo = conexo;
    }
}

public class PlanVisita
{
    public int[] Orden { get; }
    public double Km { get; }
    public PlanVisita(int[] orden, double km) { Orden = orden; Km = km; }
}

public class Grafo
{
    public const double Infinito = double.PositiveInfinity;
    public const int MaxParadas = 10;

    private readonly Distribuidor[] vertices;
    private readonly double[,] pesos;
    private readonly ListaSimple<Arista>[] adyacencia;
    private readonly ListaSimple<Arista> aristas = new ListaSimple<Arista>();
    private double[,]? distancias;
    private int[,]? siguiente;

    public Grafo(Distribuidor[] vertices)
    {
        this.vertices = vertices;
        int n = vertices.Length;
        pesos = new double[n, n];
        adyacencia = new ListaSimple<Arista>[n];
        for (int i = 0; i < n; i++)
        {
            vertices[i].Indice = i;
            adyacencia[i] = new ListaSimple<Arista>();
            for (int j = 0; j < n; j++) pesos[i, j] = i == j ? 0 : Infinito;
        }
    }

    public int Cantidad => vertices.Length;
    public int CantidadAristas => aristas.Cantidad;
    public ListaSimple<Arista> Aristas => aristas;
    public Distribuidor Vertice(int indice) => vertices[indice];
    public ListaSimple<Arista> Vecinos(int indice) => adyacencia[indice];
    public double Peso(int a, int b) => pesos[a, b];

    public int IndiceDe(string nombre)
    {
        string buscado = (nombre ?? "").Trim();
        for (int i = 0; i < vertices.Length; i++)
            if (string.Equals(vertices[i].Nombre.Trim(), buscado, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    public bool AgregarArista(int a, int b, double km, double? minutos = null)
    {
        int n = Cantidad;
        if (a < 0 || b < 0 || a >= n || b >= n || a == b) return false;
        if (double.IsNaN(km) || double.IsInfinity(km) || km <= 0) return false;
        if (pesos[a, b] != Infinito) return false;

        pesos[a, b] = km;
        pesos[b, a] = km;
        aristas.AgregarFinal(new Arista(a, b, km, minutos));
        adyacencia[a].AgregarFinal(new Arista(a, b, km, minutos));
        adyacencia[b].AgregarFinal(new Arista(b, a, km, minutos));
        distancias = null;
        siguiente = null;
        return true;
    }

    public void Floyd()
    {
        int n = Cantidad;
        double[,] dist = (double[,])pesos.Clone();
        int[,] sig = new int[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                sig[i, j] = (i != j && pesos[i, j] != Infinito) ? j : -1;

        for (int k = 0; k < n; k++)
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    double por_k = dist[i, k] + dist[k, j];
                    if (por_k < dist[i, j]) { dist[i, j] = por_k; sig[i, j] = sig[i, k]; }
                }

        distancias = dist;
        siguiente = sig;
    }

    public double Distancia(int a, int b)
    {
        if (distancias == null) Floyd();
        return distancias![a, b];
    }

    public int[]? Ruta(int origen, int destino)
    {
        if (distancias == null) Floyd();
        if (origen == destino) return new[] { origen };
        int[,] sig = siguiente!;
        if (sig[origen, destino] == -1) return null;

        int[] camino = new int[Cantidad];
        int cuenta = 0;
        int actual = origen;
        camino[cuenta++] = actual;
        while (actual != destino && cuenta < camino.Length)
        {
            actual = sig[actual, destino];
            camino[cuenta++] = actual;
        }
        int[] resultado = new int[cuenta];
        Array.Copy(camino, resultado, cuenta);
        return resultado;
    }

    public ResultadoPrim Prim(int inicio = 0)
    {
        int n = Cantidad;
        ListaSimple<Arista> elegidas = new ListaSimple<Arista>();
        if (n == 0) return new ResultadoPrim(elegidas, 0, true);
        if (inicio < 0 || inicio >= n) inicio = 0;

        bool[] en_arbol = new bool[n];
        double[] mejor = new double[n];
        int[] padre = new int[n];
        for (int i = 0; i < n; i++) { mejor[i] = Infinito; padre[i] = -1; }
        mejor[inicio] = 0;

        double total = 0;
        int agregados = 0;
        for (int paso = 0; paso < n; paso++)
        {
            int u = -1;
            for (int v = 0; v < n; v++)
                if (!en_arbol[v] && mejor[v] != Infinito && (u == -1 || mejor[v] < mejor[u])) u = v;
            if (u == -1) break;

            en_arbol[u] = true;
            agregados++;
            if (padre[u] != -1)
            {
                elegidas.AgregarFinal(new Arista(padre[u], u, pesos[padre[u], u]));
                total += pesos[padre[u], u];
            }
            foreach (Arista arista in adyacencia[u])
                if (!en_arbol[arista.Destino] && arista.Km < mejor[arista.Destino])
                {
                    mejor[arista.Destino] = arista.Km;
                    padre[arista.Destino] = u;
                }
        }
        return new ResultadoPrim(elegidas, total, agregados == n);
    }

    public PlanVisita? PlanOptimo(int inicio, int[] por_visitar, bool regresar)
    {
        if (distancias == null) Floyd();
        double[,] d = distancias!;

        bool[] marcado = new bool[Cantidad];
        int[] lista = new int[por_visitar.Length];
        int k = 0;
        foreach (int v in por_visitar)
            if (v != inicio && v >= 0 && v < Cantidad && !marcado[v]) { marcado[v] = true; lista[k++] = v; }

        if (k == 0) return new PlanVisita(new[] { inicio }, 0);
        if (k > MaxParadas) throw new InvalidOperationException($"El plan óptimo admite hasta {MaxParadas} paradas (se pidieron {k}).");

        bool[] usado = new bool[k];
        int[] actual = new int[k];
        int[] mejor_orden = new int[k];
        double mejor = Infinito;

        void Buscar(int nivel, int ultimo, double acumulado)
        {
            if (acumulado >= mejor) return;
            if (nivel == k)
            {
                double total = acumulado + (regresar ? d[ultimo, inicio] : 0);
                if (total < mejor) { mejor = total; Array.Copy(actual, mejor_orden, k); }
                return;
            }
            for (int i = 0; i < k; i++)
            {
                if (usado[i]) continue;
                usado[i] = true;
                actual[nivel] = lista[i];
                Buscar(nivel + 1, lista[i], acumulado + d[ultimo, lista[i]]);
                usado[i] = false;
            }
        }
        Buscar(0, inicio, 0);

        if (mejor == Infinito) return null;
        int[] orden = new int[k + (regresar ? 2 : 1)];
        orden[0] = inicio;
        for (int i = 0; i < k; i++) orden[i + 1] = mejor_orden[i];
        if (regresar) orden[k + 1] = inicio;
        return new PlanVisita(orden, mejor);
    }

    private static string Num(double valor) => valor.ToString("0.##", CultureInfo.InvariantCulture);

    private string Etiqueta(int i)
    {
        string nombre = vertices[i].Nombre;
        return (nombre.Length > 8 ? nombre.Substring(0, 8) : nombre).PadRight(9);
    }

    private string TextoMatriz(double[,] matriz)
    {
        StringBuilder texto = new StringBuilder();
        texto.Append(new string(' ', 9));
        for (int j = 0; j < Cantidad; j++) texto.Append(Etiqueta(j));
        texto.AppendLine();
        for (int i = 0; i < Cantidad; i++)
        {
            texto.Append(Etiqueta(i));
            for (int j = 0; j < Cantidad; j++)
                texto.Append((matriz[i, j] == Infinito ? "∞" : Num(matriz[i, j])).PadRight(9));
            texto.AppendLine();
        }
        return texto.ToString();
    }

    public string TextoMatrizAdyacencia() => TextoMatriz(pesos);

    public string TextoMatrizFloyd()
    {
        if (distancias == null) Floyd();
        return TextoMatriz(distancias!);
    }

    public string TextoListaAdyacencia()
    {
        StringBuilder texto = new StringBuilder();
        for (int i = 0; i < Cantidad; i++)
        {
            texto.Append(vertices[i].Nombre).Append(" -> ");
            if (adyacencia[i].EstaVacia) texto.Append("(sin conexiones)");
            else
            {
                bool primero = true;
                foreach (Arista arista in adyacencia[i])
                {
                    if (!primero) texto.Append(", ");
                    texto.Append(vertices[arista.Destino].Nombre).Append(" (").Append(Num(arista.Km)).Append(" km)");
                    primero = false;
                }
            }
            texto.AppendLine();
        }
        return texto.ToString();
    }
}