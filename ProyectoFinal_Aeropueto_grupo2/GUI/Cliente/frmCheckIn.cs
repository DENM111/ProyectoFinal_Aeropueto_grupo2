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
using ProyectoFinal_Aeropueto_grupo2.GUI.Admin;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Cliente
{
    public partial class frmCheckIn : Form
    {
        public frmCheckIn()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Menu_Cliente f20 = new Menu_Cliente();
            this.Hide();
            f20.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombres.Text.Trim();
            string apellido = txtApellidos.Text.Trim();
            string strMaletas = txtMaletas.Text.Trim();
            string strPeso = txtPesoMaletas.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(strMaletas) || string.IsNullOrWhiteSpace(strPeso))
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }

            if (!int.TryParse(strMaletas, out int numeroMaletas) || numeroMaletas < 0)
            {
                MessageBox.Show("Número de maletas inválido.");
                return;
            }

            if (!double.TryParse(strPeso, out double pesoMaletas) || pesoMaletas < 0)
            {
                MessageBox.Show("Peso total inválido.");
                return;
            }

            Boleto nuevoBoleto = new Boleto
            {
                CodigoBoleto = $"BOL-{DateTime.Now:yyyyMMddHHmmss}",
                NombrePasajero = nombre,
                ApellidosPasajero = apellido,
                NumeroMaletas = numeroMaletas,
                PesoTotalMaletas = pesoMaletas,
                FechaEmision = DateTime.Now,
                Estado = "Check-In"
            };

            string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Boletos\Boletos.JSON";

            List<Boleto> listaBoletos = new List<Boleto>();

            if (File.Exists(ruta))
            {
                string jsonExistente = File.ReadAllText(ruta);
                listaBoletos = JsonConvert.DeserializeObject<List<Boleto>>(jsonExistente) ?? new List<Boleto>();
            }

            listaBoletos.Add(nuevoBoleto);

            string json = JsonConvert.SerializeObject(listaBoletos, Formatting.Indented);
            File.WriteAllText(ruta, json);

            MessageBox.Show("Check-in guardado exitosamente");
        }
    }
}
