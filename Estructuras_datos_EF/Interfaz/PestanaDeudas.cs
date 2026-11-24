using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using Estructuras_datos_EP.Servicios;
using System.Globalization;
using System.Text;

namespace Estructuras_datos_EP.Interfaz;

public class PestanaDeudas : TabPage
{
    private readonly Func<GestorTransacciones> obtener_gestor;
    private readonly Func<Grafo?> obtener_grafo;
    private readonly Action<int[]> resaltar;
    private readonly Action refrescar;
    private readonly Action<string> mensaje;
    private readonly ControlDeudas deudas = new ControlDeudas();

    private readonly ListView lvDistribuidores = new ListView();
    private readonly NumericUpDown numMonto = new NumericUpDown();
    private readonly ComboBox cmbInicio = new ComboBox();
    private readonly CheckBox chkRegresar = new CheckBox();
    private readonly TextBox txtSalida = new TextBox();

    public PestanaDeudas(Func<GestorTransacciones> obtener_gestor, Func<Grafo?> obtener_grafo,
                         Action<int[]> resaltar, Action refrescar, Action<string> mensaje)
    {
        this.obtener_gestor = obtener_gestor;
        this.obtener_grafo = obtener_grafo;
        this.resaltar = resaltar;
        this.refrescar = refrescar;
        this.mensaje = mensaje;
        Text = "Deudas";

        lvDistribuidores.Dock = DockStyle.Fill;
        lvDistribuidores.View = View.Details;
        lvDistribuidores.FullRowSelect = true;
        lvDistribuidores.GridLines = true;
        lvDistribuidores.HideSelection = false;
        lvDistribuidores.MultiSelect = false;
        lvDistribuidores.CheckBoxes = true;
        lvDistribuidores.Columns.Add("Distribuidor", 170);
        lvDistribuidores.Columns.Add("Fiado", 90);
        lvDistribuidores.Columns.Add("Total fiado", 85);
        lvDistribuidores.Columns.Add("Pagado", 75);
        lvDistribuidores.Columns.Add("En trámite", 80);
        lvDistribuidores.Columns.Add("Saldo S/", 80);

        numMonto.DecimalPlaces = 2;
        numMonto.Minimum = 0;
        numMonto.Maximum = 100000;
        numMonto.Value = 50;
        numMonto.Width = 85;

        cmbInicio.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbInicio.Width = 150;
        chkRegresar.Text = "Regresar al inicio";
        chkRegresar.AutoSize = true;
        chkRegresar.Checked = true;
        chkRegresar.Margin = new Padding(3, 8, 3, 0);

        txtSalida.Multiline = true;
        txtSalida.ReadOnly = true;
        txtSalida.WordWrap = false;
        txtSalida.ScrollBars = ScrollBars.Both;
        txtSalida.Font = new Font("Consolas", 9f);
        txtSalida.Dock = DockStyle.Fill;

        FlowLayoutPanel fila_deudas = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true };
        fila_deudas.Controls.Add(Etiqueta("Monto S/:"));
        fila_deudas.Controls.Add(numMonto);
        fila_deudas.Controls.Add(Boton("Comprar fiado", Comprar));
        fila_deudas.Controls.Add(Boton("Pagar deuda (va a la cola)", Pagar));
        fila_deudas.Controls.Add(Boton("Marcar los que tienen deuda", MarcarConDeuda));
        fila_deudas.Controls.Add(Etiqueta("Salir desde:"));
        fila_deudas.Controls.Add(cmbInicio);
        fila_deudas.Controls.Add(chkRegresar);
        fila_deudas.Controls.Add(Boton("Plan de visita óptimo", Plan));

        FlowLayoutPanel fila_datos = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true };
        fila_datos.Controls.Add(Boton("Cargar 40 simulados a la cola", Simular40));
        fila_datos.Controls.Add(Boton("Atender toda la cola (1 de cada 5 sin señal)", AtenderTodo));
        fila_datos.Controls.Add(Boton("Probar volumen de 10 000", Volumen));

        TableLayoutPanel tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        tabla.Controls.Add(lvDistribuidores, 0, 0);
        tabla.Controls.Add(fila_deudas, 0, 1);
        tabla.Controls.Add(fila_datos, 0, 2);
        tabla.Controls.Add(txtSalida, 0, 3);
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
            catch (Exception excepcion) { mensaje("AVISO: " + excepcion.Message); }
            refrescar();
        };
        return boton;
    }

    private static string Dinero(decimal valor) => valor.ToString("0.00", CultureInfo.InvariantCulture);

    public void Reiniciar()
    {
        deudas.Limpiar();
        txtSalida.Clear();
    }

    public void Actualizar()
    {
        Grafo? grafo = obtener_grafo();
        GestorTransacciones gestor = obtener_gestor();

        HashSet<string> marcados = new HashSet<string>();
        foreach (ListViewItem item in lvDistribuidores.Items) if (item.Checked) marcados.Add(item.Text);
        string? seleccionado = lvDistribuidores.SelectedItems.Count > 0 ? lvDistribuidores.SelectedItems[0].Text : null;
        int inicio_previo = cmbInicio.SelectedIndex;

        lvDistribuidores.BeginUpdate();
        lvDistribuidores.Items.Clear();
        cmbInicio.Items.Clear();
        if (grafo != null)
        {
            for (int i = 0; i < grafo.Cantidad; i++)
            {
                Distribuidor distribuidor = grafo.Vertice(i);
                cmbInicio.Items.Add(distribuidor.Nombre);

                ListViewItem item = new ListViewItem(distribuidor.Nombre) { Tag = distribuidor, Checked = marcados.Contains(distribuidor.Nombre) };
                item.SubItems.Add(distribuidor.AceptaFiado ? "Sí" : "No (contado)");
                item.SubItems.Add(Dinero(deudas.TotalFiado(distribuidor)));
                item.SubItems.Add(Dinero(deudas.Pagado(gestor, distribuidor)));
                item.SubItems.Add(Dinero(deudas.EnTramite(gestor, distribuidor)));
                item.SubItems.Add(Dinero(deudas.Saldo(gestor, distribuidor)));
                if (!distribuidor.AceptaFiado) item.ForeColor = Color.Gray;
                lvDistribuidores.Items.Add(item);
                if (distribuidor.Nombre == seleccionado) item.Selected = true;
            }
            if (grafo.Cantidad > 0) cmbInicio.SelectedIndex = Math.Min(Math.Max(inicio_previo, 0), grafo.Cantidad - 1);
        }
        lvDistribuidores.EndUpdate();
    }

    private Distribuidor Seleccionado()
    {
        if (lvDistribuidores.SelectedItems.Count == 0)
            throw new InvalidOperationException("Seleccione un distribuidor de la lista (haga clic en su fila).");
        return (Distribuidor)lvDistribuidores.SelectedItems[0].Tag!;
    }

    private void Comprar()
    {
        Distribuidor distribuidor = Seleccionado();
        string? error = deudas.ComprarFiado(distribuidor, numMonto.Value);
        mensaje(error != null
            ? "COMPRA RECHAZADA: " + error
            : $"Compra fiada a {distribuidor.Nombre} por S/ {Dinero(numMonto.Value)}. Total fiado: S/ {Dinero(deudas.TotalFiado(distribuidor))}.");
    }

    private void Pagar()
    {
        Distribuidor distribuidor = Seleccionado();
        List<string> errores = deudas.PagarDeuda(obtener_gestor(), distribuidor, numMonto.Value, out Operacion? operacion);
        if (errores.Count > 0)
        {
            foreach (string error in errores) mensaje("PAGO RECHAZADO: " + error);
            return;
        }
        mensaje($"Pago {operacion!.Codigo} de S/ {Dinero(operacion.Monto)} a {distribuidor.Nombre} entró a la cola. Atiéndalo en «Atender» (con o sin señal).");
    }

    private void MarcarConDeuda()
    {
        GestorTransacciones gestor = obtener_gestor();
        foreach (ListViewItem item in lvDistribuidores.Items)
            item.Checked = deudas.Saldo(gestor, (Distribuidor)item.Tag!) > 0;
    }

    private void Plan()
    {
        Grafo grafo = obtener_grafo() ?? throw new InvalidOperationException("Primero cargue el grafo (pestaña Grafo).");
        if (cmbInicio.SelectedIndex < 0) throw new InvalidOperationException("Elija el punto de partida.");
        int inicio = cmbInicio.SelectedIndex;

        List<int> destinos = new List<int>();
        foreach (ListViewItem item in lvDistribuidores.CheckedItems) destinos.Add(((Distribuidor)item.Tag!).Indice);
        if (destinos.Count == 0) throw new InvalidOperationException("Marque (casilla) al menos un distribuidor para visitar.");

        PlanVisita? plan = grafo.PlanOptimo(inicio, destinos.ToArray(), chkRegresar.Checked);
        if (plan == null)
        {
            txtSalida.Text = "No existe un recorrido: algún distribuidor marcado no es alcanzable desde el punto de partida.";
            return;
        }

        StringBuilder texto = new StringBuilder();
        texto.AppendLine("PLAN DE VISITA ÓPTIMO (distancias de Floyd-Warshall + mejor orden)");
        texto.AppendLine("Orden: " + string.Join(" → ", plan.Orden.Select(i => grafo.Vertice(i).Nombre)));
        texto.AppendLine("Distancia total: " + plan.Km.ToString("0.##", CultureInfo.InvariantCulture) + " km");
        texto.AppendLine();

        List<int> completa = new List<int>();
        for (int i = 0; i + 1 < plan.Orden.Length; i++)
        {
            int[]? tramo = grafo.Ruta(plan.Orden[i], plan.Orden[i + 1]);
            if (tramo == null) continue;
            texto.AppendLine($"  {grafo.Vertice(plan.Orden[i]).Nombre} → {grafo.Vertice(plan.Orden[i + 1]).Nombre}: " +
                             $"{grafo.Distancia(plan.Orden[i], plan.Orden[i + 1]).ToString("0.##", CultureInfo.InvariantCulture)} km, " +
                             "por " + string.Join(" → ", tramo.Select(v => grafo.Vertice(v).Nombre)));
            for (int j = completa.Count == 0 ? 0 : 1; j < tramo.Length; j++) completa.Add(tramo[j]);
        }
        txtSalida.Text = texto.ToString();
        resaltar(completa.ToArray());
    }

    private void Simular40()
    {
        GestorTransacciones gestor = obtener_gestor();
        List<Operacion> operaciones = Simulacion.Generar(40, 2026, ControlDeudas.SiguienteNumero(gestor));
        List<string> errores = gestor.CargarLlegadas(operaciones);
        mensaje($"Simulados: {operaciones.Count - errores.Count} encolados, {errores.Count} rechazados.");
        foreach (string error in errores) mensaje("RECHAZADA " + error);
    }

    private void AtenderTodo()
    {
        GestorTransacciones gestor = obtener_gestor();
        if (gestor.Llegadas.EstaVacia) throw new InvalidOperationException("La cola está vacía: cargue datos primero.");
        foreach (string linea in Simulacion.AtenderTodo(gestor, 5)) mensaje(linea);
    }

    private void Volumen()
    {
        Cursor = Cursors.WaitCursor;
        try
        {
            ResultadoVolumen resultado = Simulacion.ProbarVolumen(10000);
            txtSalida.Text = resultado.ToString();
            mensaje("Volumen: " + resultado.Resumen);
        }
        finally { Cursor = Cursors.Default; }
    }
}