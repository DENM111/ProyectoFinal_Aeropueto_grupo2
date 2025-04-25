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
using ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN;

namespace ProyectoFinal_Aeropueto_grupo2.GUI.Admin
{
    public partial class frmGestion_Empleados : Form
    {
        List<Empleado> empleados = new List<Empleado>();
        string rutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Empleados\Empleados.json";

        public frmGestion_Empleados()
        {
            InitializeComponent();
        }

        private void frmGestion_Empleados_Load(object sender, EventArgs e)
        {
            CrearArchivoSiNoExiste();
            CargarEmpleados();
            ConfigurarDataGridView();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Menu_Admin f16 = new Menu_Admin();
            this.Hide();
            f16.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                int id = empleados.Count > 0 ? empleados.Max(emp => emp.IdEmpleado) + 1 : 1;
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string dni = txtDNI.Text.Trim();
                string puesto = txtPuesto.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                decimal salario = 15000;
                DateTime fechaContratacion = DateTime.Now;

                Empleado nuevoEmpleado = new Empleado(id, nombre, apellido, dni, puesto, salario, fechaContratacion, correo, telefono);
                empleados.Add(nuevoEmpleado);

                GuardarEmpleados();
                RefrescarLista();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (listaEmpleados.CurrentRow != null)
            {
                Empleado seleccionado = listaEmpleados.CurrentRow.DataBoundItem as Empleado;
                if (seleccionado != null)
                {
                    empleados.Remove(seleccionado);
                    GuardarEmpleados();
                    RefrescarLista();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un empleado para eliminar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (listaEmpleados.CurrentRow != null)
            {
                Empleado seleccionado = listaEmpleados.CurrentRow.DataBoundItem as Empleado;
                if (seleccionado != null)
                {
                    try
                    {
                        seleccionado.ActualizarCargo(txtPuesto.Text.Trim());
                        seleccionado.CambiarEstado(true);
                        seleccionado.ActualizarSalario(15000);

                        GuardarEmpleados();
                        RefrescarLista();
                        LimpiarCampos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al modificar empleado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un empleado para modificar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CrearArchivoSiNoExiste()
        {
            string carpeta = Path.GetDirectoryName(rutaArchivo);

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            if (!File.Exists(rutaArchivo))
            {
                File.WriteAllText(rutaArchivo, "[]");
            }
        }

        private void CargarEmpleados()
        {
            try
            {
                if (File.Exists(rutaArchivo))
                {
                    string json = File.ReadAllText(rutaArchivo);
                    empleados = JsonConvert.DeserializeObject<List<Empleado>>(json) ?? new List<Empleado>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empleados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                empleados = new List<Empleado>();
            }

            RefrescarLista();
        }

        private void GuardarEmpleados()
        {
            try
            {
                string json = JsonConvert.SerializeObject(empleados, Formatting.Indented);
                File.WriteAllText(rutaArchivo, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar empleados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarLista()
        {
            listaEmpleados.DataSource = null;
            listaEmpleados.DataSource = empleados;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            txtPuesto.Clear();
            txtCorreo.Clear();
            txtTelefono.Clear();
        }

        private void ConfigurarDataGridView()
        {
            listaEmpleados.AutoGenerateColumns = true;
            listaEmpleados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            listaEmpleados.MultiSelect = false;
        }
    }
}
