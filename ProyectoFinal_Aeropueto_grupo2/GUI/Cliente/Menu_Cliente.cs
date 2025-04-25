using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Cliente
{
    public partial class Menu_Cliente : Form
    {
        public Menu_Cliente()
        {
            InitializeComponent();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 f14 = new Form1();
            this.Hide(); // Oculta Form2
            f14.Show();   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmCheckIn f4 = new frmCheckIn();
            this.Hide(); 
            f4.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmHistorial_Cliente f5 = new frmHistorial_Cliente();
            this.Hide(); 
            f5.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmReservas_Vuelo  f6 = new frmReservas_Vuelo();
            this.Hide();
            f6.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmServicios f7 = new frmServicios();
            this.Hide();
            f7.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmVer_Boletos f8 = new frmVer_Boletos();
            this.Hide();
            f8.Show();
        }

        private void Menu_Cliente_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
