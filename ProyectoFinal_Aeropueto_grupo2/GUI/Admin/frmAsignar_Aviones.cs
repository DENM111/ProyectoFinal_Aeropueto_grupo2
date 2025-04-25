using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Admin
{
    public partial class frmAsignar_Aviones : Form
    {
        private List<Avion> listaAviones = new List<Avion>();

        public frmAsignar_Aviones()
        {
            InitializeComponent();
        }

        private void frmAsignar_Aviones_Load(object sender, EventArgs e)
        {
            listaAviones = Avion.AvionRepository.CargarAviones();

            var disponibles = listaAviones
                .Where(a => a.Estado == "Disponible")
                .ToList();

            cmbAvionDisponible.DataSource = disponibles;
            cmbAvionDisponible.DisplayMember = "CodigoIATA";
            cmbAvionDisponible.ValueMember = "IdAvion";

            txtCodigoVuelo.Clear();
            txtModelo.Clear();
            txtCapacidad.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Menu_Admin f15 = new Menu_Admin();
            this.Hide();
            f15.Show();
        }

        private void btnAsignarVuelo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigoVuelo.Text))
            {
                MessageBox.Show("Debe ingresar el código de vuelo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbAvionDisponible.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un avión disponible.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtModelo.Text))
            {
                MessageBox.Show("Debe ingresar el modelo del avión.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCapacidad.Text, out int capacidad) || capacidad <= 0)
            {
                MessageBox.Show("La capacidad debe ser un número válido mayor a 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Avion avionSeleccionado = (Avion)cmbAvionDisponible.SelectedItem;

            try
            {
                avionSeleccionado.Modelo = txtModelo.Text.Trim();
                avionSeleccionado.CapacidadPasajeros = capacidad;

                avionSeleccionado.CambiarEstado("En Vuelo");

                Avion.AvionRepository.GuardarAviones(listaAviones);

                MessageBox.Show("Avión asignado y actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar el avión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbAvionDisponible_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAvionDisponible.SelectedItem is Avion avion)
            {
                txtModelo.Text = avion.Modelo;
                txtCapacidad.Text = avion.CapacidadPasajeros.ToString();
            }
        }
    }
}
