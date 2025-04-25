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
using ProyectoFinal_Aeropueto_grupo2.GUI.Cliente;

namespace ProyectoFinal_Aeropueto_grupo2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Menu_Cliente f2 = new Menu_Cliente();
            this.Hide(); // Oculta el formulario actual
            f2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            Menu_Admin f3 = new Menu_Admin();
            this.Hide(); // Oculta el formulario actual
            f3.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
