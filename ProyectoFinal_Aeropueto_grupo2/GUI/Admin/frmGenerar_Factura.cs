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
    public partial class frmGenerar_Factura : Form
    {
        private List<Factura> listaFacturas = new List<Factura>();

        public frmGenerar_Factura()
        {
            InitializeComponent();
        }

        private void frmGenerar_Factura_Load(object sender, EventArgs e)
        {
            listaFacturas = Factura.FacturaRepository.CargarFacturas();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Menu_Admin f16 = new Menu_Admin();
            this.Hide();
            f16.Show();
        }

        

        private void LimpiarFormulario()
        {
            txtCodigoReserva.Clear();
            txtNombreCliente.Clear();
            txtDetalle.Clear();
            txtMontoTotal.Clear();
            dtpFechaEmision.Value = DateTime.Now;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnGenerarFactura_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Obtener valores ingresados
                string numeroFactura = txtCodigoReserva.Text.Trim();
                string nombrePasajero = txtNombreCliente.Text.Trim();
                string detalleVuelo = txtDetalle.Text.Trim();
                string montoTexto = txtMontoTotal.Text.Trim();
                DateTime fechaSeleccionada = dtpFechaEmision.Value;

                if (string.IsNullOrWhiteSpace(numeroFactura))
                    throw new ArgumentException("Debe ingresar el número de factura.");

                if (string.IsNullOrWhiteSpace(nombrePasajero))
                    throw new ArgumentException("Debe ingresar el nombre del pasajero.");

                if (string.IsNullOrWhiteSpace(detalleVuelo))
                    throw new ArgumentException("Debe ingresar el código de vuelo.");

                if (!decimal.TryParse(montoTexto, out decimal subtotal) || subtotal <= 0)
                    throw new ArgumentException("Debe ingresar un monto válido y positivo.");

                List<string> vuelos = new List<string> { detalleVuelo };

                Factura nuevaFactura = new Factura(
                    numero: numeroFactura,
                    dni: "000000000000",
                    nombre: nombrePasajero,
                    vuelos: vuelos,
                    subtotal: subtotal,
                    metodoPago: "Efectivo"
                );

                typeof(Factura)
                    .GetProperty("FechaEmision")
                    .SetValue(nuevaFactura, fechaSeleccionada);

                listaFacturas.Add(nuevaFactura);
                Factura.FacturaRepository.GuardarFacturas(listaFacturas);

                MessageBox.Show("Factura generada y guardada exitosamente.", "Factura", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar factura: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
