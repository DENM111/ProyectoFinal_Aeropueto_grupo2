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
    public partial class frmHistorial_Cliente : Form
    {
        public frmHistorial_Cliente()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Menu_Cliente f21 = new Menu_Cliente();
            this.Hide();
            f21.Show(); 
        }
    }
}
