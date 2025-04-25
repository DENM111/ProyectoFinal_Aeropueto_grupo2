using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoFinal_Aeropueto_grupo2.GUI.Admin;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Cliente
{
    public partial class frmReservas_Vuelo : Form
    {
        public frmReservas_Vuelo()
        {
            InitializeComponent();
        }

        private void frmReservas_Vuelo_Load(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Menu_Cliente f23 = new Menu_Cliente();
            this.Hide();
            f23.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
