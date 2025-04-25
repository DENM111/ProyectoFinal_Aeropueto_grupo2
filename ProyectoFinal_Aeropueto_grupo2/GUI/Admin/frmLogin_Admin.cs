using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Admin
{
    public partial class frmLogin_Admin : Form
    {
        public frmLogin_Admin()
        {
            InitializeComponent();
        }

        private void frmLogin_Admin_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Menu_Admin f18 = new Menu_Admin();
            this.Hide();
            f18.Show();
        }
    }
}
