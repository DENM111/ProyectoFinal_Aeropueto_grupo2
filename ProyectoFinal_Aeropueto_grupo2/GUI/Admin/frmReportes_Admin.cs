using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Admin
{
    public partial class frmReportes_Admin : Form
    {
        private List<Reporte> reportes = new List<Reporte>();
        private readonly string rutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Reporte\Reporte.JSON";

        public frmReportes_Admin()
        {
            InitializeComponent();
        }

        private void frmReportes_Admin_Load(object sender, EventArgs e)
        {
            CrearArchivoSiNoExiste();
            CargarReportes();
            ConfigurarControles();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbTipoReporte.SelectedItem == null)
                    throw new ArgumentException("Seleccione un tipo de reporte");

                DateTime fechaInicio = dtpFechaInicio.Value.Date;
                DateTime fechaFin = dtpFechaFin.Value.Date;

                if (fechaInicio > fechaFin)
                    throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha fin");

                string tipo = cmbTipoReporte.SelectedItem.ToString();
                string generadoPor = "Administrador"; 

                int id = reportes.Count > 0 ? reportes[reportes.Count - 1].IdReporte + 1 : 1;

                Reporte nuevoReporte = new Reporte(id, tipo, fechaInicio, fechaFin, generadoPor);

                nuevoReporte.AgregarDatoNumerico("Total de registros", new Random().Next(10, 100));
                nuevoReporte.AgregarDatoTexto("Observación", "Reporte generado correctamente.");
                nuevoReporte.GenerarResumen();

                reportes.Add(nuevoReporte);
                GuardarReportes();

                txtTablaResultados.Text = nuevoReporte.Resumen;
                MessageBox.Show("Reporte generado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Menu_Admin f19 = new Menu_Admin();
            this.Hide();
            f19.Show();
        }

        private void CrearArchivoSiNoExiste()
        {
            string carpeta = Path.GetDirectoryName(rutaArchivo);

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            if (!File.Exists(rutaArchivo))
                File.WriteAllText(rutaArchivo, "[]");
        }

        private void GuardarReportes()
        {
            try
            {
                string json = JsonConvert.SerializeObject(reportes, Formatting.Indented);
                File.WriteAllText(rutaArchivo, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar reportes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarReportes()
        {
            try
            {
                if (File.Exists(rutaArchivo))
                {
                    string json = File.ReadAllText(rutaArchivo);
                    reportes = JsonConvert.DeserializeObject<List<Reporte>>(json) ?? new List<Reporte>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reportes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                reportes = new List<Reporte>();
            }
        }

        private void ConfigurarControles()
        {
            cmbTipoReporte.Items.Clear();
            cmbTipoReporte.Items.AddRange(new string[] { "Vuelos", "Pasajeros", "Financiero", "Mantenimiento", "Operaciones" });
            cmbTipoReporte.SelectedIndex = 0;
        }
    }
}
