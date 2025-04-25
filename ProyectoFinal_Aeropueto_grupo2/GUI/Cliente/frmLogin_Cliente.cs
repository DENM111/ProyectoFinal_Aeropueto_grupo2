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
    public partial class frmLogin_Cliente : Form
    {
        public frmLogin_Cliente()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Menu_Cliente f22 = new Menu_Cliente();
            this.Hide();
            f22.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
