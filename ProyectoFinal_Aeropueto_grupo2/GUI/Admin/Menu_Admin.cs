using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoFinal_Aeropueto_grupo2.GUI.Cliente;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Admin
{
    public partial class Menu_Admin : Form
    {
        public Menu_Admin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAsignar_Aviones f9 = new frmAsignar_Aviones();
            this.Hide(); 
            f9.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmGenerar_Factura f10 = new frmGenerar_Factura();
            this.Hide();
            f10.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            frmGestion_Empleados f11 = new frmGestion_Empleados();
            this.Hide();
            f11.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            frmGestion_Vuelos f12 = new frmGestion_Vuelos();
            this.Hide();
            f12.Show();
        }

        private void Menu_Admin_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 f13 = new Form1();
            this.Hide(); 
            f13.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmReportes_Admin f25 = new frmReportes_Admin();
            this.Hide();
            f25.Show();
        }
    }
}
