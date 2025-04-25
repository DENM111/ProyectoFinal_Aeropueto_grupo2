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
    public partial class frmVer_Boletos : Form
    {
        public frmVer_Boletos()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Menu_Cliente f25 = new Menu_Cliente();
            this.Hide();
            f25.Show();
        }
    }
}
