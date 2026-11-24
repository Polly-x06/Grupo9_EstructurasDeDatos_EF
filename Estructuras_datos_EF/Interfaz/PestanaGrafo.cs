using Estructuras_datos_EP.Datos;
using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;

namespace Estructuras_datos_EP.Interfaz;

public class PestanaGrafo : TabPage
{
    private const float RadioNodo = 20f;

    private sealed class LienzoDoble : Panel
    {
        public LienzoDoble() { DoubleBuffered = true; ResizeRedraw = true; BackColor = Color.White; }
    }

    private readonly Action<string> mensaje;
    private readonly LienzoDoble lienzo = new LienzoDoble();
    private readonly ComboBox cmbOrigen = new ComboBox();
    private readonly ComboBox cmbDestino = new ComboBox();
    private readonly TextBox txtSalida = new TextBox();
    private bool[,]? en_ruta;
    private bool[,]? en_arbol;

    public Grafo? GrafoActual { get; private set; }
    public event Action? GrafoCambiado;

    public static string CarpetaDatos => Path.Combine(AppContext.BaseDirectory, "Datos");

    public PestanaGrafo(Action<string> mensaje)
    {
        this.mensaje = mensaje;
        Text = "Grafo";

        cmbOrigen.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbOrigen.Width = 150;
        cmbDestino.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDestino.Width = 150;

        txtSalida.Multiline = true;
        txtSalida.ReadOnly = true;
        txtSalida.WordWrap = false;
        txtSalida.ScrollBars = ScrollBars.Both;
        txtSalida.Font = new Font("Consolas", 9f);
        txtSalida.Dock = DockStyle.Fill;

        lienzo.Dock = DockStyle.Fill;
        lienzo.Paint += Dibujar;

        FlowLayoutPanel barra = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true };
        barra.Controls.Add(Boton("Cargar CSV...", CargarManual));
        barra.Controls.Add(Etiqueta("Origen:"));
        barra.Controls.Add(cmbOrigen);
        barra.Controls.Add(Etiqueta("Destino:"));
        barra.Controls.Add(cmbDestino);
        barra.Controls.Add(Boton("Ruta más corta", Ruta));
        barra.Controls.Add(Boton("Floyd-Warshall", MostrarFloyd));
        barra.Controls.Add(Boton("Prim (árbol mínimo)", MostrarPrim));
        barra.Controls.Add(Boton("Matriz y lista", MostrarMatriz));
        barra.Controls.Add(Boton("Quitar resaltado", QuitarResaltado));

        TableLayoutPanel tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));
        tabla.Controls.Add(barra, 0, 0);
        tabla.Controls.Add(lienzo, 0, 1);
        tabla.Controls.Add(txtSalida, 0, 2);
        Controls.Add(tabla);
    }

    private static Label Etiqueta(string texto) =>
        new Label { Text = texto, AutoSize = true, Margin = new Padding(6, 8, 0, 0) };

    private Button Boton(string texto, Action accion)
    {
        Button boton = new Button { Text = texto, AutoSize = true, Margin = new Padding(3) };
        boton.Click += (remitente, argumentos) =>
        {
            try { accion(); }
            catch (Exception excepcion) { mensaje("GRAFO: " + excepcion.Message); }
        };
        return boton;
    }

    private Grafo Requerir() =>
        GrafoActual ?? throw new InvalidOperationException("Primero cargue el grafo con «Cargar CSV...».");

    public void CargarPorDefecto()
    {
        string distribuidores = Path.Combine(CarpetaDatos, "distribuidores.csv");
        string conexiones = Path.Combine(CarpetaDatos, "conexiones.csv");
        if (!File.Exists(distribuidores) || !File.Exists(conexiones))
        {
            mensaje($"Grafo: coloque distribuidores.csv y conexiones.csv en {CarpetaDatos} o use «Cargar CSV...».");
            return;
        }
        try { CargarDe(CarpetaDatos); }
        catch (Exception excepcion) { mensaje("GRAFO: " + excepcion.Message); }
    }

    private void CargarManual()
    {
        using FolderBrowserDialog dialogo = new FolderBrowserDialog
        {
            Description = "Elija la carpeta que contiene distribuidores.csv y conexiones.csv",
            SelectedPath = Directory.Exists(CarpetaDatos) ? CarpetaDatos : AppContext.BaseDirectory
        };
        if (dialogo.ShowDialog() == DialogResult.OK) CargarDe(dialogo.SelectedPath);
    }

    private void CargarDe(string carpeta)
    {
        Grafo grafo = CargadorCsv.Cargar(Path.Combine(carpeta, "distribuidores.csv"), Path.Combine(carpeta, "conexiones.csv"));
        GrafoActual = grafo;
        en_ruta = null;
        en_arbol = null;

        cmbOrigen.Items.Clear();
        cmbDestino.Items.Clear();
        for (int i = 0; i < grafo.Cantidad; i++)
        {
            cmbOrigen.Items.Add(grafo.Vertice(i).Nombre);
            cmbDestino.Items.Add(grafo.Vertice(i).Nombre);
        }
        cmbOrigen.SelectedIndex = 0;
        cmbDestino.SelectedIndex = grafo.Cantidad > 1 ? grafo.Cantidad - 1 : 0;

        txtSalida.Text = $"Grafo cargado: {grafo.Cantidad} distribuidores y {grafo.CantidadAristas} conexiones." + Environment.NewLine + Environment.NewLine + TextoMatrizYLista(grafo);
        mensaje($"Grafo cargado desde {carpeta}: {grafo.Cantidad} distribuidores, {grafo.CantidadAristas} conexiones.");
        lienzo.Invalidate();
        GrafoCambiado?.Invoke();
    }

    private static string TextoMatrizYLista(Grafo grafo) =>
        "MATRIZ DE ADYACENCIA (km; ∞ = sin conexión directa)" + Environment.NewLine + grafo.TextoMatrizAdyacencia() +
        Environment.NewLine + "LISTA DE ADYACENCIA" + Environment.NewLine + grafo.TextoListaAdyacencia();

    private void MostrarMatriz()
    {
        Grafo grafo = Requerir();
        txtSalida.Text = TextoMatrizYLista(grafo);
    }

    private void MostrarFloyd()
    {
        Grafo grafo = Requerir();
        grafo.Floyd();
        txtSalida.Text = "DISTANCIAS MÍNIMAS ENTRE TODOS LOS PARES (Floyd-Warshall, km)" + Environment.NewLine + grafo.TextoMatrizFloyd();
    }

    private void Ruta()
    {
        Grafo grafo = Requerir();
        int a = cmbOrigen.SelectedIndex;
        int b = cmbDestino.SelectedIndex;
        if (a < 0 || b < 0) throw new InvalidOperationException("Elija el origen y el destino.");

        int[]? camino = grafo.Ruta(a, b);
        if (camino == null)
        {
            en_ruta = null;
            lienzo.Invalidate();
            txtSalida.Text = $"No hay ruta entre {grafo.Vertice(a).Nombre} y {grafo.Vertice(b).Nombre}.";
            return;
        }
        txtSalida.Text = "RUTA MÁS CORTA (Floyd-Warshall)" + Environment.NewLine +
                         string.Join(" → ", camino.Select(i => grafo.Vertice(i).Nombre)) + Environment.NewLine +
                         "Distancia: " + grafo.Distancia(a, b).ToString("0.##", CultureInfo.InvariantCulture) + " km";
        Resaltar(camino);
    }

    private void MostrarPrim()
    {
        Grafo grafo = Requerir();
        int inicio = cmbOrigen.SelectedIndex < 0 ? 0 : cmbOrigen.SelectedIndex;
        ResultadoPrim resultado = grafo.Prim(inicio);

        en_arbol = new bool[grafo.Cantidad, grafo.Cantidad];
        StringBuilder texto = new StringBuilder();
        texto.AppendLine($"ÁRBOL DE EXPANSIÓN MÍNIMA (Prim, desde {grafo.Vertice(inicio).Nombre})");
        foreach (Arista arista in resultado.Aristas)
        {
            en_arbol[arista.Origen, arista.Destino] = true;
            en_arbol[arista.Destino, arista.Origen] = true;
            texto.AppendLine($"  {grafo.Vertice(arista.Origen).Nombre} — {grafo.Vertice(arista.Destino).Nombre} : {arista.Km.ToString("0.##", CultureInfo.InvariantCulture)} km");
        }
        texto.AppendLine("Total: " + resultado.Total.ToString("0.##", CultureInfo.InvariantCulture) + " km");
        if (!resultado.Conexo) texto.AppendLine("AVISO: el grafo no es conexo; el árbol solo cubre lo alcanzable desde el origen.");
        txtSalida.Text = texto.ToString();
        lienzo.Invalidate();
    }

    private void QuitarResaltado()
    {
        en_ruta = null;
        en_arbol = null;
        lienzo.Invalidate();
    }

    public void Resaltar(int[] vertices)
    {
        if (GrafoActual == null) return;
        int n = GrafoActual.Cantidad;
        en_ruta = new bool[n, n];
        for (int i = 0; i + 1 < vertices.Length; i++)
        {
            en_ruta[vertices[i], vertices[i + 1]] = true;
            en_ruta[vertices[i + 1], vertices[i]] = true;
        }
        lienzo.Invalidate();
    }

    private PointF[] Posiciones(int n)
    {
        float cx = lienzo.Width / 2f;
        float cy = lienzo.Height / 2f;
        float radio = Math.Max(40f, Math.Min(cx, cy) - 55f);
        PointF[] puntos = new PointF[n];
        for (int i = 0; i < n; i++)
        {
            double angulo = 2 * Math.PI * i / n - Math.PI / 2;
            puntos[i] = new PointF(cx + (float)(radio * Math.Cos(angulo)), cy + (float)(radio * Math.Sin(angulo)));
        }
        return puntos;
    }

    private void Dibujar(object? remitente, PaintEventArgs argumentos)
    {
        Graphics grafico = argumentos.Graphics;
        grafico.SmoothingMode = SmoothingMode.AntiAlias;
        using Font fuente = new Font("Segoe UI", 9f);

        if (GrafoActual == null || GrafoActual.Cantidad == 0)
        {
            grafico.DrawString("Cargue distribuidores.csv y conexiones.csv con «Cargar CSV...».", fuente, Brushes.Gray, 12, 12);
            return;
        }

        Grafo grafo = GrafoActual;
        PointF[] posicion = Posiciones(grafo.Cantidad);
        using Pen lapiz_normal = new Pen(Color.Gray, 1.5f);
        using Pen lapiz_ruta = new Pen(Color.Crimson, 4f);
        using Pen lapiz_arbol = new Pen(Color.SeaGreen, 3f);

        foreach (Arista arista in grafo.Aristas)
        {
            Pen lapiz = lapiz_normal;
            if (en_arbol != null && en_arbol[arista.Origen, arista.Destino]) lapiz = lapiz_arbol;
            if (en_ruta != null && en_ruta[arista.Origen, arista.Destino]) lapiz = lapiz_ruta;
            PointF p1 = posicion[arista.Origen];
            PointF p2 = posicion[arista.Destino];
            grafico.DrawLine(lapiz, p1, p2);

            string etiqueta = arista.Km.ToString("0.##", CultureInfo.InvariantCulture) + " km";
            SizeF tamano = grafico.MeasureString(etiqueta, fuente);
            float mx = (p1.X + p2.X) / 2f - tamano.Width / 2f;
            float my = (p1.Y + p2.Y) / 2f - tamano.Height / 2f;
            grafico.FillRectangle(Brushes.White, mx, my, tamano.Width, tamano.Height);
            grafico.DrawString(etiqueta, fuente, Brushes.Black, mx, my);
        }

        for (int i = 0; i < grafo.Cantidad; i++)
        {
            Distribuidor distribuidor = grafo.Vertice(i);
            RectangleF caja = new RectangleF(posicion[i].X - RadioNodo, posicion[i].Y - RadioNodo, RadioNodo * 2, RadioNodo * 2);
            grafico.FillEllipse(distribuidor.AceptaFiado ? Brushes.LightSkyBlue : Brushes.Orange, caja);
            grafico.DrawEllipse(Pens.Black, caja);

            SizeF tam_numero = grafico.MeasureString((i + 1).ToString(), fuente);
            grafico.DrawString((i + 1).ToString(), fuente, Brushes.Black, posicion[i].X - tam_numero.Width / 2f, posicion[i].Y - tam_numero.Height / 2f);

            SizeF tam_nombre = grafico.MeasureString(distribuidor.Nombre, fuente);
            grafico.DrawString(distribuidor.Nombre, fuente, Brushes.Black, posicion[i].X - tam_nombre.Width / 2f, posicion[i].Y + RadioNodo + 2f);
        }
    }
}