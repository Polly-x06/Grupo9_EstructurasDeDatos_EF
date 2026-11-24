using Estructuras_datos_EP.Datos;
using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using Estructuras_datos_EP.Pruebas;
using Estructuras_datos_EP.Servicios;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Estructuras_datos_EP;

public partial class FrmOperacionesBodega : Form
{
    // Medidas del dibujo del árbol
    private const int AnchoNodo = 64;
    private const int AltoNodo = 26;
    private const int SeparacionX = 76;
    private const int SeparacionY = 58;
    private const int MargenArbol = 20;
    private const int LimiteDibujo = 80;

    private GestorTransacciones gestor = new();
    private string? codigo_resaltado;

    public FrmOperacionesBodega()
    {
        InitializeComponent();
        ConfigurarControles();
        ConectarEventos();
        CargarCodigoSugerido();
        Refrescar();
        Mensaje("Listo. Cargue los datos de campo o registre una operación nueva.");
    }

    #region Configuración de controles

    private void ConfigurarControles()
    {
        Text = "Prototipo EF - Bodega Chabelita (Estructuras de Datos)";

        cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
        foreach (TipoOperacion tipo in Enum.GetValues<TipoOperacion>()) cmbTipo.Items.Add(tipo);
        cmbTipo.SelectedIndex = 0;

        dtpFechaHora.Format = DateTimePickerFormat.Custom;
        dtpFechaHora.CustomFormat = "dd/MM/yyyy HH:mm";
        dtpFechaHora.Value = DateTime.Now;

        numMonto.DecimalPlaces = 2;
        numMonto.Minimum = -100000;
        numMonto.Maximum = 100000;
        numMonto.Value = 5;

        numSenalIntento.Minimum = 0;
        numSenalIntento.Maximum = 10;
        numSenalIntento.Value = 2;

        txtOrigen.Text = "Cliente 01";
        txtDestino.Text = "Bodega Chabelita";

        chkSinSenal.Text = "Atender SIN señal (va a reintentos)";
        btnLimpiarCola.Text = "Limpiar cola";
        btnLimpiarPila.Text = "Limpiar pila";
        btnAnular.Text = "Anular operación";

        ConfigurarLista(lvCola);
        ConfigurarLista(lvRegistro);
        ConfigurarLista(lvHistorial);
        ConfigurarLista(lvReintentos);
        ConfigurarLista(lvPila);

        TabPage.Text = "Árbol";
        btnInOrden.Text = "Inorden";
        btnPreOrden.Text = "Preorden";
        btnPostOrden.Text = "Postorden";
        btnComparar.Text = "Comparar lista vs árbol";
        txtRecorrido.Multiline = true;
        txtRecorrido.ReadOnly = true;
        txtRecorrido.ScrollBars = ScrollBars.Vertical;
        pnlArbol.BackColor = Color.White;
        pnlArbol.AutoScroll = true;
        typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(pnlArbol, true);
    }

    private static void ConfigurarLista(ListView lista_vista)
    {
        lista_vista.View = View.Details;
        lista_vista.FullRowSelect = true;
        lista_vista.GridLines = true;
        lista_vista.HideSelection = false;
        lista_vista.Columns.Clear();
        lista_vista.Columns.Add("Código", 80);
        lista_vista.Columns.Add("Fecha y hora", 120);
        lista_vista.Columns.Add("Tipo", 90);
        lista_vista.Columns.Add("Monto S/", 70);
        lista_vista.Columns.Add("Origen", 100);
        lista_vista.Columns.Add("Destino", 130);
        lista_vista.Columns.Add("Estado", 80);
        lista_vista.Columns.Add("Intentos", 60);
    }

    private void ConectarEventos()
    {
        // Cola y atención
        btnEncolar.Click += (remitente, argumentos) => Ejecutar(Encolar);
        btnVerFrente.Click += (remitente, argumentos) => Ejecutar(VerFrente);
        btnAtender.Click += (remitente, argumentos) => Ejecutar(Atender);
        btnLimpiarCola.Click += (remitente, argumentos) => Ejecutar(LimpiarCola);

        // Reintentos y reversa
        btnReintentar.Click += (remitente, argumentos) => Ejecutar(Reintentar);
        btnRevertir.Click += (remitente, argumentos) => Ejecutar(Revertir);
        btnLimpiarPila.Click += (remitente, argumentos) => Ejecutar(LimpiarPila);

        // Consulta
        btnBuscar.Click += (remitente, argumentos) => Ejecutar(Buscar);
        btnAnular.Click += (remitente, argumentos) => Ejecutar(Anular);

        // Datos y pruebas
        btnCargarCampo.Click += (remitente, argumentos) => Ejecutar(CargarCampo);
        btnPruebas.Click += (remitente, argumentos) => Ejecutar(EjecutarPruebas);
        btnReiniciar.Click += (remitente, argumentos) => Ejecutar(Reiniciar);

        // Historial (lista doble)
        btnInicio.Click += (remitente, argumentos) => Ejecutar(IrAlInicio);
        btnAnterior.Click += (remitente, argumentos) => Ejecutar(IrAnterior);
        btnSiguiente.Click += (remitente, argumentos) => Ejecutar(IrSiguiente);
        btnFinal.Click += (remitente, argumentos) => Ejecutar(IrAlFinal);

        // Árbol
        btnInOrden.Click += (remitente, argumentos) => Ejecutar(() => MostrarRecorrido("Inorden", gestor.Indice.InOrden()));
        btnPreOrden.Click += (remitente, argumentos) => Ejecutar(() => MostrarRecorrido("Preorden", gestor.Indice.PreOrden()));
        btnPostOrden.Click += (remitente, argumentos) => Ejecutar(() => MostrarRecorrido("Postorden", gestor.Indice.PostOrden()));
        btnComparar.Click += (remitente, argumentos) => Ejecutar(Comparar);
        pnlArbol.Paint += DibujarArbol;
    }

    // Todas las acciones pasan por aquí: los avisos de las estructuras se muestran y la pantalla se refresca.
    private void Ejecutar(Action accion)
    {
        try { accion(); }
        catch (InvalidOperationException excepcion) { Mensaje("AVISO: " + excepcion.Message); }
        catch (Exception excepcion) { Mensaje("ERROR: " + excepcion.Message); }
        Refrescar();
    }

    #endregion

    #region Cola y atención

    private void Encolar()
    {
        Operacion operacion = new Operacion
        {
            Codigo = txtCodigo.Text,
            FechaHora = dtpFechaHora.Value,
            Tipo = (TipoOperacion)cmbTipo.SelectedItem!,
            Monto = numMonto.Value,
            Origen = txtOrigen.Text,
            Destino = txtDestino.Text
        };

        List<string> errores = gestor.Llegar(operacion);
        if (errores.Count > 0)
        {
            foreach (string error in errores) Mensaje("INVÁLIDO: " + error);
            MessageBox.Show(string.Join("\n", errores), "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Mensaje($"Encolada {operacion.Codigo} (posición {gestor.Llegadas.Cantidad} en la cola).");
        CargarCodigoSugerido();
    }

    private void VerFrente()
    {
        Operacion frente = gestor.ConsultarFrente();
        Mensaje("FRENTE de la cola: " + frente);
    }

    private void Atender()
    {
        bool hay_senal = !chkSinSenal.Checked;
        Operacion operacion = gestor.Atender(hay_senal);
        Mensaje(hay_senal
            ? $"Atendida {operacion.Codigo}: procesada al intento {operacion.Intentos}."
            : $"Atendida {operacion.Codigo}: sin señal, pasa a la lista de reintentos.");
    }

    private void LimpiarCola()
    {
        int descartadas = gestor.LimpiarCola();
        Mensaje(descartadas == 0
            ? "La cola ya estaba vacía."
            : $"Cola limpiada: se descartaron {descartadas} operaciones pendientes (no llegaron a registrarse).");
    }

    #endregion

    #region Reintentos y reversa

    private void Reintentar()
    {
        if (gestor.Reintentos.EstaVacia)
        {
            Mensaje("No hay operaciones pendientes de reintento.");
            return;
        }
        int intento_con_senal = (int)numSenalIntento.Value;
        List<string> lista_eventos = gestor.Reintentar(operacion => intento_con_senal > 0 && operacion.Intentos >= intento_con_senal);
        foreach (string evento in lista_eventos) Mensaje(evento);
    }

    private void Revertir()
    {
        Operacion operacion = gestor.RevertirUltimoAbono();
        Mensaje($"Revertido el último abono: {operacion.Codigo} por S/ {operacion.Monto:0.00}.");
    }

    private void LimpiarPila()
    {
        int confirmados = gestor.LimpiarPila();
        Mensaje(confirmados == 0
            ? "La pila ya estaba vacía."
            : $"Pila limpiada: {confirmados} abonos confirmados; ya no se podrán revertir.");
    }

    #endregion

    #region Historial (lista doble)

    private bool HistorialVacio()
    {
        if (!gestor.Historial.EstaVacia) return false;
        Mensaje("El historial está vacío.");
        return true;
    }

    private void IrAlInicio() { if (!HistorialVacio()) gestor.Historial.IrAlInicio(); }

    private void IrAlFinal() { if (!HistorialVacio()) gestor.Historial.IrAlFinal(); }

    private void IrAnterior()
    {
        if (!HistorialVacio() && !gestor.Historial.Anterior()) Mensaje("Ya está en la primera operación del historial.");
    }

    private void IrSiguiente()
    {
        if (!HistorialVacio() && !gestor.Historial.Siguiente()) Mensaje("Ya está en la última operación del historial.");
    }

    #endregion

    #region Consulta y árbol

    private void Buscar()
    {
        Operacion? operacion = gestor.BuscarPorCodigo(txtBuscar.Text);
        if (operacion == null)
        {
            codigo_resaltado = null;
            string detalle = Operacion.TryObtenerNumero(txtBuscar.Text, out _)
                ? $" (árbol: {gestor.Indice.Comparaciones} comparaciones)"
                : " (no tiene formato de código)";
            Mensaje($"No existe una operación con el código '{txtBuscar.Text.Trim()}'{detalle}.");
            return;
        }

        codigo_resaltado = operacion.Codigo;
        bool en_arbol = gestor.Indice.Buscar(operacion.Numero) != null;
        Mensaje(en_arbol
            ? $"ENCONTRADA en el árbol ({gestor.Indice.Comparaciones} comparaciones): {operacion}"
            : $"ENCONTRADA en la cola (aún no entra al árbol): {operacion}");
    }

    private void Comparar()
    {
        (Operacion? encontrada, int en_lista, int en_arbol) = gestor.CompararBusqueda(txtBuscar.Text);
        if (encontrada == null)
        {
            Mensaje($"Comparar: '{txtBuscar.Text.Trim()}' no está registrada (lista simple: {en_lista}, árbol: {en_arbol} comparaciones).");
            return;
        }
        codigo_resaltado = encontrada.Codigo;
        Mensaje($"Comparar {encontrada.Codigo}: lista simple {en_lista} comparaciones, árbol {en_arbol} comparaciones.");
    }

    private void Anular()
    {
        Operacion operacion = gestor.AnularOperacion(txtBuscar.Text);
        if (codigo_resaltado == operacion.Codigo) codigo_resaltado = null;
        Mensaje($"Anulada {operacion.Codigo} ({operacion.Estado}): se eliminó del registro, del historial y del árbol.");
    }

    private void MostrarRecorrido(string nombre, ListaSimple<Operacion> recorrido)
    {
        if (gestor.Indice.EstaVacio)
        {
            txtRecorrido.Text = "El árbol está vacío.";
            Mensaje("El árbol está vacío: no hay nada que recorrer.");
            return;
        }
        txtRecorrido.Text = $"{nombre} ({recorrido.Cantidad} nodos): " + string.Join(" → ", recorrido.Select(operacion => operacion.Codigo));
        Mensaje($"Recorrido {nombre} del árbol: {recorrido.Cantidad} nodos.");
    }

    #endregion

    #region Datos y pruebas

    private void CargarCampo()
    {
        List<Operacion> operaciones = DatosCampo.Obtener();
        List<string> errores = gestor.CargarLlegadas(operaciones);
        Mensaje($"Datos de campo: {operaciones.Count - errores.Count} encoladas, {errores.Count} rechazadas.");
        foreach (string error in errores) Mensaje("RECHAZADA " + error);
        CargarCodigoSugerido();
    }

    private void EjecutarPruebas()
    {
        EjecutorPruebas ejecutor = new EjecutorPruebas();
        string texto = ejecutor.Correr();
        string ruta = Path.Combine(AppContext.BaseDirectory, "resultados_pruebas.txt");
        File.WriteAllText(ruta, texto, Encoding.UTF8);
        Mensaje($"Pruebas: {ejecutor.Ok} OK, {ejecutor.Falla} con falla. Guardado en: {ruta}");

        using Form ventana = new Form
        {
            Text = "Resultados de las pruebas",
            Width = 900,
            Height = 600,
            StartPosition = FormStartPosition.CenterParent
        };
        TextBox cuadro_texto = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10),
            ScrollBars = ScrollBars.Vertical,
            Text = texto
        };
        ventana.Controls.Add(cuadro_texto);
        cuadro_texto.Select(0, 0);
        ventana.ShowDialog(this);
    }

    private void Reiniciar()
    {
        gestor = new GestorTransacciones();
        codigo_resaltado = null;
        txtRecorrido.Clear();
        CargarCodigoSugerido();
        Mensaje("Estructuras reiniciadas: todas quedaron vacías.");
    }

    #endregion

    #region Presentación

    // El código sugerido es siempre el siguiente al mayor número en uso (en el registro o en la cola).
    private void CargarCodigoSugerido()
    {
        int mayor_numero = gestor.Registro.Concat(gestor.Llegadas).Select(operacion => operacion.Numero).DefaultIfEmpty(0).Max();
        txtCodigo.Text = $"OP-{mayor_numero + 1:000}";
    }

    private void Mensaje(string texto)
    {
        lstMensajes.Items.Add($"[{DateTime.Now:HH:mm:ss}] {texto}");
        lstMensajes.TopIndex = lstMensajes.Items.Count - 1;
    }

    private void Refrescar()
    {
        Llenar(lvCola, gestor.Llegadas);
        Llenar(lvRegistro, gestor.Registro);
        Llenar(lvHistorial, gestor.Historial);
        Llenar(lvReintentos, gestor.Reintentos);
        Llenar(lvPila, gestor.Abonos);

        Operacion? operacion_actual = gestor.Historial.Actual;
        lblActual.Text = operacion_actual == null ? "Historial vacío." : "Actual: " + operacion_actual;
        lblResumen.Text = $"Cola: {gestor.Llegadas.Cantidad}  |  Registro: {gestor.Registro.Cantidad}  |  " +
                          $"Historial: {gestor.Historial.Cantidad}  |  Reintentos: {gestor.Reintentos.Cantidad}  |  " +
                          $"Pila (abonos): {gestor.Abonos.Cantidad}";
        RefrescarArbol();
    }

    private void Llenar(ListView lista_vista, IEnumerable<Operacion> datos)
    {
        lista_vista.BeginUpdate();
        lista_vista.Items.Clear();
        Operacion? operacion_actual = ReferenceEquals(lista_vista, lvHistorial) ? gestor.Historial.Actual : null;
        foreach (Operacion operacion in datos)
        {
            ListViewItem item_lista = new ListViewItem(new[]
            {
                operacion.Codigo,
                operacion.FechaHora.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                operacion.Tipo.ToString(),
                operacion.Monto.ToString("0.00", CultureInfo.InvariantCulture),
                operacion.Origen,
                operacion.Destino,
                operacion.Estado.ToString(),
                operacion.Intentos.ToString()
            });
            item_lista.ForeColor = ColorDeEstado(operacion.Estado);
            if (operacion_actual != null && ReferenceEquals(operacion, operacion_actual)) item_lista.BackColor = Color.LightSkyBlue;
            lista_vista.Items.Add(item_lista);
        }
        lista_vista.EndUpdate();
    }

    private static Color ColorDeEstado(EstadoOperacion estado) => estado switch
    {
        EstadoOperacion.Procesada => Color.DarkGreen,
        EstadoOperacion.Pendiente => Color.DarkGoldenrod,
        EstadoOperacion.Fallida => Color.Firebrick,
        EstadoOperacion.Revertida => Color.Gray,
        _ => Color.Black
    };

    #endregion

    #region Dibujo del árbol

    private void RefrescarArbol()
    {
        int altura = gestor.Indice.AsignarDisposicion();
        lblArbolInfo.Text = gestor.Indice.EstaVacio
            ? "Árbol vacío."
            : $"Nodos: {gestor.Indice.Cantidad}  |  Altura: {altura}  |  Raíz: {gestor.Indice.Raiz!.Dato.Codigo}  |  Comparaciones de la última consulta: {gestor.Indice.Comparaciones}";

        pnlArbol.AutoScrollMinSize = !gestor.Indice.EstaVacio && gestor.Indice.Cantidad <= LimiteDibujo
            ? new Size(MargenArbol * 2 + gestor.Indice.Cantidad * SeparacionX, MargenArbol * 2 + altura * SeparacionY)
            : Size.Empty;
        pnlArbol.Invalidate();
    }

    private static Point CentroNodo(NodoArbol<Operacion> nodo) =>
        new Point(MargenArbol + nodo.Columna * SeparacionX + AnchoNodo / 2,
                  MargenArbol + nodo.Nivel * SeparacionY + AltoNodo / 2);

    private void DibujarArbol(object? remitente, PaintEventArgs argumentos)
    {
        Graphics grafico = argumentos.Graphics;
        grafico.SmoothingMode = SmoothingMode.AntiAlias;
        using Font fuente = new Font("Segoe UI", 8.5f);
        using StringFormat centrado = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        if (gestor.Indice.EstaVacio)
        {
            grafico.DrawString("Árbol vacío: se llena al atender operaciones.", fuente, Brushes.Gray, 12, 12);
            return;
        }
        if (gestor.Indice.Cantidad > LimiteDibujo)
        {
            grafico.DrawString($"El árbol tiene {gestor.Indice.Cantidad} nodos: es demasiado grande para dibujarlo (límite {LimiteDibujo}). Use los recorridos.", fuente, Brushes.Gray, 12, 12);
            return;
        }

        grafico.TranslateTransform(pnlArbol.AutoScrollPosition.X, pnlArbol.AutoScrollPosition.Y);
        ListaSimple<NodoArbol<Operacion>> nodos = gestor.Indice.NodosPreOrden();

        using Pen lapiz_enlace = new Pen(Color.DimGray, 1.5f);
        foreach (NodoArbol<Operacion> nodo in nodos)
        {
            Point origen = CentroNodo(nodo);
            if (nodo.Izquierdo != null) grafico.DrawLine(lapiz_enlace, origen, CentroNodo(nodo.Izquierdo));
            if (nodo.Derecho != null) grafico.DrawLine(lapiz_enlace, origen, CentroNodo(nodo.Derecho));
        }

        foreach (NodoArbol<Operacion> nodo in nodos)
        {
            Point centro = CentroNodo(nodo);
            Rectangle caja = new Rectangle(centro.X - AnchoNodo / 2, centro.Y - AltoNodo / 2, AnchoNodo, AltoNodo);
            bool resaltado = nodo.Dato.Codigo == codigo_resaltado;
            Color color_estado = ColorDeEstado(nodo.Dato.Estado);
            using SolidBrush relleno = new SolidBrush(resaltado ? Color.Gold : Color.White);
            using SolidBrush texto = new SolidBrush(color_estado);
            using Pen borde = new Pen(color_estado, 2f);
            grafico.FillRectangle(relleno, caja);
            grafico.DrawRectangle(borde, caja);
            grafico.DrawString(nodo.Dato.Codigo, fuente, texto, caja, centrado);
        }
    }

    #endregion

}