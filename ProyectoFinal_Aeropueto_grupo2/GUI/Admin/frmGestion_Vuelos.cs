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
    public partial class frmGestion_Vuelos : Form
    {
        private List<Vuelo> vuelos = new List<Vuelo>();
        private readonly string rutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Vuelos\Vuelos.JSON";

        public frmGestion_Vuelos()
        {
            InitializeComponent();
        }

        private void frmGestion_Vuelos_Load(object sender, EventArgs e)
        {
            CrearArchivoSiNoExiste();
            CargarVuelos();
            ConfigurarDataGridView();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Menu_Admin f17 = new Menu_Admin();
            this.Hide();
            f17.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                string codigo = txtCodigoVuelo.Text.Trim();
                string origen = txtOrigen.Text.Trim();
                string destino = txtDestino.Text.Trim();
                DateTime fechaVuelo = dtpFechaVuelo.Value.Date;
                string horarioTexto = txtHorario.Text.Trim();

                if (!TimeSpan.TryParse(horarioTexto, out TimeSpan horario))
                    throw new ArgumentException("Horario inválido (debe ser formato HH:mm)");

                DateTime fechaHoraSalida = fechaVuelo.Add(horario);
                DateTime fechaHoraLlegada = fechaHoraSalida.AddHours(2);

                if (!int.TryParse(txtAvionAsignado.Text.Trim(), out int idAvion))
                    throw new ArgumentException("ID de avión inválido (debe ser número)");

                if (!int.TryParse(txtCapacidad.Text.Trim(), out int capacidad))
                    throw new ArgumentException("Capacidad inválida (debe ser número)");

                decimal precioBase = 2000;

                Vuelo nuevoVuelo = new Vuelo(codigo, origen, destino, fechaHoraSalida, fechaHoraLlegada, idAvion, capacidad, precioBase);

                vuelos.Add(nuevoVuelo);
                GuardarVuelos();
                RefrescarLista();
                LimpiarCampos();

                MessageBox.Show("Vuelo agregado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar vuelo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (listaVuelos.CurrentRow != null)
            {
                Vuelo vueloSeleccionado = listaVuelos.CurrentRow.DataBoundItem as Vuelo;
                if (vueloSeleccionado != null)
                {
                    try
                    {
                        vueloSeleccionado.ActualizarEstado("Programado");
                        vueloSeleccionado.ActualizarPrecio(3000);
                        GuardarVuelos();
                        RefrescarLista();
                        LimpiarCampos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al actualizar vuelo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (listaVuelos.CurrentRow != null)
            {
                Vuelo vueloSeleccionado = listaVuelos.CurrentRow.DataBoundItem as Vuelo;
                if (vueloSeleccionado != null)
                {
                    vuelos.Remove(vueloSeleccionado);
                    GuardarVuelos();
                    RefrescarLista();
                }
            }
        }

        private void CrearArchivoSiNoExiste()
        {
            string carpeta = Path.GetDirectoryName(rutaArchivo);

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            if (!File.Exists(rutaArchivo))
                File.WriteAllText(rutaArchivo, "[]");
        }

        private void GuardarVuelos()
        {
            try
            {
                string json = JsonConvert.SerializeObject(vuelos, Formatting.Indented);
                File.WriteAllText(rutaArchivo, json);
                MessageBox.Show("Vuelos guardados correctamente en JSON");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar vuelos: " + ex.Message);
            }
        }

        private void CargarVuelos()
        {
            try
            {
                if (File.Exists(rutaArchivo))
                {
                    string json = File.ReadAllText(rutaArchivo);
                    vuelos = JsonConvert.DeserializeObject<List<Vuelo>>(json) ?? new List<Vuelo>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar vuelos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                vuelos = new List<Vuelo>();
            }

            RefrescarLista();
        }

        private void RefrescarLista()
        {
            listaVuelos.DataSource = null;
            listaVuelos.DataSource = vuelos;
        }

        private void LimpiarCampos()
        {
            txtCodigoVuelo.Clear();
            txtOrigen.Clear();
            txtDestino.Clear();
            txtHorario.Clear();
            txtAvionAsignado.Clear();
            txtCapacidad.Clear();
            dtpFechaVuelo.Value = DateTime.Now;
        }

        private void ConfigurarDataGridView()
        {
            listaVuelos.AutoGenerateColumns = true;
            listaVuelos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            listaVuelos.MultiSelect = false;
        }
    }
}
