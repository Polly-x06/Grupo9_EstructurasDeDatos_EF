namespace Estructuras_datos_EP
{
    partial class FrmOperacionesBodega
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            numMonto = new NumericUpDown();
            cmbTipo = new ComboBox();
            dtpFechaHora = new DateTimePicker();
            txtDestino = new TextBox();
            txtOrigen = new TextBox();
            txtCodigo = new TextBox();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            groupBox2 = new GroupBox();
            chkSinSenal = new CheckBox();
            btnAtender = new Button();
            btnVerFrente = new Button();
            btnEncolar = new Button();
            groupBox3 = new GroupBox();
            btnRevertir = new Button();
            btnReintentar = new Button();
            numSenalIntento = new NumericUpDown();
            label7 = new Label();
            groupBox4 = new GroupBox();
            btnBuscar = new Button();
            txtBuscar = new TextBox();
            groupBox5 = new GroupBox();
            btnReiniciar = new Button();
            btnPruebas = new Button();
            btnCargarCampo = new Button();
            groupBox7 = new GroupBox();
            lblResumen = new Label();
            lstMensajes = new ListBox();
            tabPage5 = new TabPage();
            lvPila = new ListView();
            tabPage4 = new TabPage();
            lvReintentos = new ListView();
            tabPage3 = new TabPage();
            lvHistorial = new ListView();
            panel1 = new Panel();
            lblActual = new Label();
            btnFinal = new Button();
            btnSiguiente = new Button();
            btnAnterior = new Button();
            btnInicio = new Button();
            tabPage2 = new TabPage();
            lvRegistro = new ListView();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            lvCola = new ListView();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMonto).BeginInit();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSenalIntento).BeginInit();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox7.SuspendLayout();
            tabPage5.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage3.SuspendLayout();
            panel1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(numMonto);
            groupBox1.Controls.Add(cmbTipo);
            groupBox1.Controls.Add(dtpFechaHora);
            groupBox1.Controls.Add(txtDestino);
            groupBox1.Controls.Add(txtOrigen);
            groupBox1.Controls.Add(txtCodigo);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.ForeColor = SystemColors.ButtonFace;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(339, 250);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Nueva operación";
            // 
            // numMonto
            // 
            numMonto.Location = new Point(109, 138);
            numMonto.Name = "numMonto";
            numMonto.Size = new Size(210, 23);
            numMonto.TabIndex = 14;
            // 
            // cmbTipo
            // 
            cmbTipo.FormattingEnabled = true;
            cmbTipo.Location = new Point(110, 99);
            cmbTipo.Name = "cmbTipo";
            cmbTipo.Size = new Size(209, 23);
            cmbTipo.TabIndex = 13;
            // 
            // dtpFechaHora
            // 
            dtpFechaHora.Location = new Point(110, 61);
            dtpFechaHora.Name = "dtpFechaHora";
            dtpFechaHora.Size = new Size(209, 23);
            dtpFechaHora.TabIndex = 12;
            // 
            // txtDestino
            // 
            txtDestino.Location = new Point(109, 217);
            txtDestino.Name = "txtDestino";
            txtDestino.Size = new Size(210, 23);
            txtDestino.TabIndex = 11;
            // 
            // txtOrigen
            // 
            txtOrigen.Location = new Point(109, 178);
            txtOrigen.Name = "txtOrigen";
            txtOrigen.Size = new Size(210, 23);
            txtOrigen.TabIndex = 10;
            // 
            // txtCodigo
            // 
            txtCodigo.Location = new Point(110, 22);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(210, 23);
            txtCodigo.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(23, 220);
            label6.Name = "label6";
            label6.Size = new Size(47, 15);
            label6.TabIndex = 5;
            label6.Text = "Destino";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(23, 181);
            label5.Name = "label5";
            label5.Size = new Size(43, 15);
            label5.TabIndex = 4;
            label5.Text = "Origen";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(22, 140);
            label4.Name = "label4";
            label4.Size = new Size(65, 15);
            label4.TabIndex = 3;
            label4.Text = "Monto (S/)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(23, 102);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 2;
            label3.Text = "Tipo";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 63);
            label2.Name = "label2";
            label2.Size = new Size(74, 15);
            label2.TabIndex = 1;
            label2.Text = "Fecha y hora";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 25);
            label1.Name = "label1";
            label1.Size = new Size(46, 15);
            label1.TabIndex = 0;
            label1.Text = "Código";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chkSinSenal);
            groupBox2.Controls.Add(btnAtender);
            groupBox2.Controls.Add(btnVerFrente);
            groupBox2.Controls.Add(btnEncolar);
            groupBox2.ForeColor = SystemColors.ButtonFace;
            groupBox2.Location = new Point(12, 268);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(339, 170);
            groupBox2.TabIndex = 15;
            groupBox2.TabStop = false;
            groupBox2.Text = "Cola (orden de arribo)";
            // 
            // chkSinSenal
            // 
            chkSinSenal.AutoSize = true;
            chkSinSenal.Location = new Point(22, 146);
            chkSinSenal.Name = "chkSinSenal";
            chkSinSenal.Size = new Size(119, 19);
            chkSinSenal.TabIndex = 3;
            chkSinSenal.Text = "Atender SIN señal";
            chkSinSenal.UseVisualStyleBackColor = true;
            // 
            // btnAtender
            // 
            btnAtender.ForeColor = SystemColors.ActiveCaptionText;
            btnAtender.Location = new Point(21, 112);
            btnAtender.Name = "btnAtender";
            btnAtender.Size = new Size(300, 23);
            btnAtender.TabIndex = 2;
            btnAtender.Text = "Atender (desencolar)";
            btnAtender.UseVisualStyleBackColor = true;
            // 
            // btnVerFrente
            // 
            btnVerFrente.ForeColor = SystemColors.ActiveCaptionText;
            btnVerFrente.Location = new Point(20, 71);
            btnVerFrente.Name = "btnVerFrente";
            btnVerFrente.Size = new Size(300, 23);
            btnVerFrente.TabIndex = 1;
            btnVerFrente.Text = "Ver frente";
            btnVerFrente.UseVisualStyleBackColor = true;
            // 
            // btnEncolar
            // 
            btnEncolar.ForeColor = SystemColors.ActiveCaptionText;
            btnEncolar.Location = new Point(20, 32);
            btnEncolar.Name = "btnEncolar";
            btnEncolar.Size = new Size(300, 23);
            btnEncolar.TabIndex = 0;
            btnEncolar.Text = "Encolar llegada";
            btnEncolar.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnRevertir);
            groupBox3.Controls.Add(btnReintentar);
            groupBox3.Controls.Add(numSenalIntento);
            groupBox3.Controls.Add(label7);
            groupBox3.ForeColor = SystemColors.ButtonFace;
            groupBox3.Location = new Point(12, 444);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(339, 137);
            groupBox3.TabIndex = 16;
            groupBox3.TabStop = false;
            groupBox3.Text = "Reintentos y reversa";
            // 
            // btnRevertir
            // 
            btnRevertir.ForeColor = SystemColors.ActiveCaptionText;
            btnRevertir.Location = new Point(20, 99);
            btnRevertir.Name = "btnRevertir";
            btnRevertir.Size = new Size(298, 23);
            btnRevertir.TabIndex = 3;
            btnRevertir.Text = "Revertir último abono";
            btnRevertir.UseVisualStyleBackColor = true;
            // 
            // btnReintentar
            // 
            btnReintentar.ForeColor = SystemColors.ActiveCaptionText;
            btnReintentar.Location = new Point(20, 65);
            btnReintentar.Name = "btnReintentar";
            btnReintentar.Size = new Size(298, 23);
            btnReintentar.TabIndex = 2;
            btnReintentar.Text = "Ejecutar reintentos";
            btnReintentar.UseVisualStyleBackColor = true;
            // 
            // numSenalIntento
            // 
            numSenalIntento.Location = new Point(139, 28);
            numSenalIntento.Name = "numSenalIntento";
            numSenalIntento.Size = new Size(180, 23);
            numSenalIntento.TabIndex = 1;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 30);
            label7.Name = "label7";
            label7.Size = new Size(129, 15);
            label7.TabIndex = 0;
            label7.Text = "Señal vuelve en intento";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnBuscar);
            groupBox4.Controls.Add(txtBuscar);
            groupBox4.ForeColor = SystemColors.ButtonFace;
            groupBox4.Location = new Point(12, 587);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(339, 92);
            groupBox4.TabIndex = 17;
            groupBox4.TabStop = false;
            groupBox4.Text = "Consulta";
            // 
            // btnBuscar
            // 
            btnBuscar.ForeColor = SystemColors.ActiveCaptionText;
            btnBuscar.Location = new Point(20, 57);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(298, 23);
            btnBuscar.TabIndex = 5;
            btnBuscar.Text = "Buscar operación";
            btnBuscar.UseVisualStyleBackColor = true;
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(20, 22);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(298, 23);
            txtBuscar.TabIndex = 4;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(btnReiniciar);
            groupBox5.Controls.Add(btnPruebas);
            groupBox5.Controls.Add(btnCargarCampo);
            groupBox5.ForeColor = SystemColors.ButtonFace;
            groupBox5.Location = new Point(12, 685);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(339, 132);
            groupBox5.TabIndex = 18;
            groupBox5.TabStop = false;
            groupBox5.Text = "Datos y pruebas";
            // 
            // btnReiniciar
            // 
            btnReiniciar.ForeColor = SystemColors.ActiveCaptionText;
            btnReiniciar.Location = new Point(21, 92);
            btnReiniciar.Name = "btnReiniciar";
            btnReiniciar.Size = new Size(295, 23);
            btnReiniciar.TabIndex = 2;
            btnReiniciar.Text = "Reiniciar";
            btnReiniciar.UseVisualStyleBackColor = true;
            // 
            // btnPruebas
            // 
            btnPruebas.ForeColor = SystemColors.ActiveCaptionText;
            btnPruebas.Location = new Point(20, 58);
            btnPruebas.Name = "btnPruebas";
            btnPruebas.Size = new Size(296, 23);
            btnPruebas.TabIndex = 1;
            btnPruebas.Text = "Ejecutar pruebas";
            btnPruebas.UseVisualStyleBackColor = true;
            // 
            // btnCargarCampo
            // 
            btnCargarCampo.ForeColor = SystemColors.ActiveCaptionText;
            btnCargarCampo.Location = new Point(20, 24);
            btnCargarCampo.Name = "btnCargarCampo";
            btnCargarCampo.Size = new Size(298, 23);
            btnCargarCampo.TabIndex = 0;
            btnCargarCampo.Text = "Cargar datos de campo";
            btnCargarCampo.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(lblResumen);
            groupBox7.Location = new Point(357, 587);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(854, 92);
            groupBox7.TabIndex = 20;
            groupBox7.TabStop = false;
            // 
            // lblResumen
            // 
            lblResumen.AutoSize = true;
            lblResumen.ForeColor = SystemColors.Control;
            lblResumen.Location = new Point(9, 17);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(339, 15);
            lblResumen.TabIndex = 0;
            lblResumen.Text = "Cola: 0 | Registro: 0 | Historial: 0 | Reintentos: 0 | Pila (abonos): 0";
            // 
            // lstMensajes
            // 
            lstMensajes.BackColor = Color.FromArgb(18, 18, 18);
            lstMensajes.ForeColor = SystemColors.Menu;
            lstMensajes.FormattingEnabled = true;
            lstMensajes.ItemHeight = 15;
            lstMensajes.Location = new Point(358, 693);
            lstMensajes.Name = "lstMensajes";
            lstMensajes.Size = new Size(853, 124);
            lstMensajes.TabIndex = 21;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(lvPila);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(845, 530);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Pila";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // lvPila
            // 
            lvPila.Dock = DockStyle.Fill;
            lvPila.Location = new Point(0, 0);
            lvPila.Name = "lvPila";
            lvPila.Size = new Size(845, 530);
            lvPila.TabIndex = 0;
            lvPila.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(lvReintentos);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(845, 530);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Lista circular";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // lvReintentos
            // 
            lvReintentos.Dock = DockStyle.Fill;
            lvReintentos.Location = new Point(0, 0);
            lvReintentos.Name = "lvReintentos";
            lvReintentos.Size = new Size(845, 530);
            lvReintentos.TabIndex = 0;
            lvReintentos.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(lvHistorial);
            tabPage3.Controls.Add(panel1);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(845, 530);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Lista doble";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // lvHistorial
            // 
            lvHistorial.Dock = DockStyle.Fill;
            lvHistorial.Location = new Point(0, 87);
            lvHistorial.Name = "lvHistorial";
            lvHistorial.Size = new Size(845, 443);
            lvHistorial.TabIndex = 1;
            lvHistorial.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblActual);
            panel1.Controls.Add(btnFinal);
            panel1.Controls.Add(btnSiguiente);
            panel1.Controls.Add(btnAnterior);
            panel1.Controls.Add(btnInicio);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(845, 87);
            panel1.TabIndex = 0;
            // 
            // lblActual
            // 
            lblActual.AutoSize = true;
            lblActual.ForeColor = SystemColors.ActiveCaptionText;
            lblActual.Location = new Point(6, 53);
            lblActual.Name = "lblActual";
            lblActual.Size = new Size(85, 15);
            lblActual.TabIndex = 5;
            lblActual.Text = "Historial vacío.";
            // 
            // btnFinal
            // 
            btnFinal.Location = new Point(249, 11);
            btnFinal.Name = "btnFinal";
            btnFinal.Size = new Size(75, 23);
            btnFinal.TabIndex = 3;
            btnFinal.Text = "Final";
            btnFinal.UseVisualStyleBackColor = true;
            // 
            // btnSiguiente
            // 
            btnSiguiente.Location = new Point(168, 11);
            btnSiguiente.Name = "btnSiguiente";
            btnSiguiente.Size = new Size(75, 23);
            btnSiguiente.TabIndex = 2;
            btnSiguiente.Text = "Siguiente";
            btnSiguiente.UseVisualStyleBackColor = true;
            // 
            // btnAnterior
            // 
            btnAnterior.Location = new Point(87, 11);
            btnAnterior.Name = "btnAnterior";
            btnAnterior.Size = new Size(75, 23);
            btnAnterior.TabIndex = 1;
            btnAnterior.Text = "Anterior";
            btnAnterior.UseVisualStyleBackColor = true;
            // 
            // btnInicio
            // 
            btnInicio.Location = new Point(6, 11);
            btnInicio.Name = "btnInicio";
            btnInicio.Size = new Size(75, 23);
            btnInicio.TabIndex = 0;
            btnInicio.Text = "Inicio";
            btnInicio.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(lvRegistro);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(845, 530);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Lista simple";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvRegistro
            // 
            lvRegistro.Dock = DockStyle.Fill;
            lvRegistro.Location = new Point(3, 3);
            lvRegistro.Name = "lvRegistro";
            lvRegistro.Size = new Size(839, 524);
            lvRegistro.TabIndex = 0;
            lvRegistro.UseCompatibleStateImageBehavior = false;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Location = new Point(356, 23);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(853, 558);
            tabControl1.TabIndex = 22;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(lvCola);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(845, 530);
            tabPage1.TabIndex = 5;
            tabPage1.Text = "Cola";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // lvCola
            // 
            lvCola.Dock = DockStyle.Fill;
            lvCola.Location = new Point(0, 0);
            lvCola.Name = "lvCola";
            lvCola.Size = new Size(845, 530);
            lvCola.TabIndex = 0;
            lvCola.UseCompatibleStateImageBehavior = false;
            // 
            // FrmOperacionesBodega
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(18, 18, 18);
            ClientSize = new Size(1224, 840);
            Controls.Add(tabControl1);
            Controls.Add(lstMensajes);
            Controls.Add(groupBox7);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "FrmOperacionesBodega";
            Text = "Bodega Chabelita";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMonto).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSenalIntento).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            tabPage5.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtCodigo;
        private TextBox txtDestino;
        private TextBox txtOrigen;
        private DateTimePicker dtpFechaHora;
        private NumericUpDown numMonto;
        private ComboBox cmbTipo;
        private GroupBox groupBox2;
        private Button btnAtender;
        private Button btnVerFrente;
        private Button btnEncolar;
        private CheckBox chkSinSenal;
        private GroupBox groupBox3;
        private Label label7;
        private NumericUpDown numSenalIntento;
        private Button btnReintentar;
        private Button btnRevertir;
        private GroupBox groupBox4;
        private Button btnBuscar;
        private TextBox txtBuscar;
        private GroupBox groupBox5;
        private Button btnReiniciar;
        private Button btnPruebas;
        private Button btnCargarCampo;
        private GroupBox groupBox7;
        private Label lblResumen;
        private ListBox lstMensajes;
        private TabPage tabPage5;
        private TabPage tabPage4;
        private TabPage tabPage3;
        private TabPage tabPage2;
        private ListView lvRegistro;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private ListView lvCola;
        private ListView lvPila;
        private ListView lvReintentos;
        private Panel panel1;
        private Button btnFinal;
        private Button btnSiguiente;
        private Button btnAnterior;
        private Button btnInicio;
        private Label lblActual;
        private ListView lvHistorial;
    }
}