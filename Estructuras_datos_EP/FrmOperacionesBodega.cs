using Estructuras_datos_EP.Datos;
using Estructuras_datos_EP.Estructuras;
using Estructuras_datos_EP.Modelos;
using Estructuras_datos_EP.Pruebas;
using Estructuras_datos_EP.Servicios;
using System.Globalization;
using System.Text;

namespace Estructuras_datos_EP;

public partial class FrmOperacionesBodega : Form
{
    private GestorTransacciones gestor = new();
    private int siguiente_numero = 1;

    public FrmOperacionesBodega()
    {
        InitializeComponent();
        ConfigurarControles();
        ConectarEventos();
        CargarCodigoSugerido();
        Refrescar();
        Mensaje("Listo. Cargue los datos de campo o registre una operación nueva.");
    }

    private void ConfigurarControles()
    {
        Text = "Prototipo EP - Bodega Chabelita (Estructuras de Datos)";

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

        ConfigurarLista(lvCola);
        ConfigurarLista(lvRegistro);
        ConfigurarLista(lvHistorial);
        ConfigurarLista(lvReintentos);
        ConfigurarLista(lvPila);
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
        btnEncolar.Click += (remitente, argumentos) => Ejecutar(Encolar);
        btnVerFrente.Click += (remitente, argumentos) => Ejecutar(VerFrente);
        btnAtender.Click += (remitente, argumentos) => Ejecutar(Atender);
        btnReintentar.Click += (remitente, argumentos) => Ejecutar(Reintentar);
        btnRevertir.Click += (remitente, argumentos) => Ejecutar(Revertir);
        btnBuscar.Click += (remitente, argumentos) => Ejecutar(Buscar);
        btnCargarCampo.Click += (remitente, argumentos) => Ejecutar(CargarCampo);
        btnPruebas.Click += (remitente, argumentos) => Ejecutar(EjecutarPruebas);
        btnReiniciar.Click += (remitente, argumentos) => Ejecutar(Reiniciar);

        btnInicio.Click += (remitente, argumentos) => Ejecutar(IrAlInicio);
        btnAnterior.Click += (remitente, argumentos) => Ejecutar(IrAnterior);
        btnSiguiente.Click += (remitente, argumentos) => Ejecutar(IrSiguiente);
        btnFinal.Click += (remitente, argumentos) => Ejecutar(IrAlFinal);
    }

    private void Ejecutar(Action accion)
    {
        try { accion(); }
        catch (InvalidOperationException excepcion) { Mensaje("AVISO: " + excepcion.Message); }
        catch (Exception excepcion) { Mensaje("ERROR: " + excepcion.Message); }
        Refrescar();
    }

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
        siguiente_numero++;
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

    private void Buscar()
    {
        Operacion? operacion = gestor.BuscarPorCodigo(txtBuscar.Text);
        Mensaje(operacion == null
            ? $"No existe una operación con el código '{txtBuscar.Text.Trim()}'."
            : "ENCONTRADA: " + operacion);
    }

    private void IrAlInicio()
    {
        if (gestor.Historial.EstaVacia) { Mensaje("El historial está vacío."); return; }
        gestor.Historial.IrAlInicio();
    }

    private void IrAlFinal()
    {
        if (gestor.Historial.EstaVacia) { Mensaje("El historial está vacío."); return; }
        gestor.Historial.IrAlFinal();
    }

    private void IrAnterior()
    {
        if (gestor.Historial.EstaVacia) { Mensaje("El historial está vacío."); return; }
        if (!gestor.Historial.Anterior()) Mensaje("Ya está en la primera operación del historial.");
    }

    private void IrSiguiente()
    {
        if (gestor.Historial.EstaVacia) { Mensaje("El historial está vacío."); return; }
        if (!gestor.Historial.Siguiente()) Mensaje("Ya está en la última operación del historial.");
    }

    private void CargarCampo()
    {
        List<Operacion> operaciones = DatosCampo.Obtener();
        List<string> errores = gestor.CargarLlegadas(operaciones);
        Mensaje($"Datos de campo: {operaciones.Count - errores.Count} encoladas, {errores.Count} rechazadas.");
        foreach (string error in errores) Mensaje("RECHAZADA " + error);
        siguiente_numero = gestor.Llegadas.Count() + gestor.Registro.Cantidad + gestor.Reintentos.Cantidad + 1;
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
        siguiente_numero = 1;
        CargarCodigoSugerido();
        Mensaje("Estructuras reiniciadas: todas quedaron vacías.");
    }

    private void CargarCodigoSugerido() => txtCodigo.Text = $"OP-{siguiente_numero:000}";

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
            item_lista.ForeColor = operacion.Estado switch
            {
                EstadoOperacion.Procesada => Color.DarkGreen,
                EstadoOperacion.Pendiente => Color.DarkGoldenrod,
                EstadoOperacion.Fallida => Color.Firebrick,
                EstadoOperacion.Revertida => Color.Gray,
                _ => Color.Black
            };
            if (operacion_actual != null && ReferenceEquals(operacion, operacion_actual)) item_lista.BackColor = Color.LightSkyBlue;
            lista_vista.Items.Add(item_lista);
        }
        lista_vista.EndUpdate();
    }

}