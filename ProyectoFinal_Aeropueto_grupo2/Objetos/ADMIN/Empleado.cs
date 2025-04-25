using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN
{
    public class Empleado : Persona
    {
        public int IdEmpleado { get; private set; }
        public string DNI { get; private set; }
        public string Cargo { get; private set; }
        public decimal Salario { get; private set; }
        public DateTime FechaContratacion { get; private set; }
        public bool Activo { get; private set; }

        public Empleado(int id, string nombre, string apellido, string dni, string cargo,
                        decimal salario, DateTime fechaContratacion, string email, string telefono)
            : base(nombre, apellido, email, telefono)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del empleado debe ser positivo");

            if (!ValidarDNI(dni))
                throw new ArgumentException("DNI no válido. Formato correcto: 8 dígitos + letra");

            if (string.IsNullOrWhiteSpace(cargo))
                throw new ArgumentException("El cargo no puede estar vacío");

            var cargosValidos = new[] { "Piloto", "Copiloto", "Azafata", "Técnico", "Administrativo", "Seguridad" };
            if (!Array.Exists(cargosValidos, c => c.Equals(cargo, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Cargo no válido. Cargos permitidos: {string.Join(", ", cargosValidos)}");

            if (salario <= 0 || salario > 100000)
                throw new ArgumentException("El salario debe ser positivo y no exceder los 100,000");

            if (fechaContratacion > DateTime.Now)
                throw new ArgumentException("La fecha de contratación no puede ser futura");

            if (fechaContratacion < new DateTime(2000, 1, 1))
                throw new ArgumentException("La fecha de contratación no puede ser anterior al año 2000");

            IdEmpleado = id;
            DNI = dni.ToUpper();
            Cargo = cargo.Trim();
            Salario = salario;
            FechaContratacion = fechaContratacion;
            Activo = true;
        }

        public string DisplayInfo
        {
            get
            {
                return $"{Nombre} {Apellido} - {Cargo}";
            }
        }

        private bool ValidarDNI(string dni)
        {
            return Regex.IsMatch(dni, @"^[0-9]{12}[A-Za-z]$");
        }

        public void ActualizarSalario(decimal nuevoSalario)
        {
            if (nuevoSalario <= 0 || nuevoSalario > 100000)
                throw new ArgumentException("El salario debe ser positivo y no exceder los 100,000");

            if (nuevoSalario < Salario && !Activo)
                throw new InvalidOperationException("No se puede reducir el salario de un empleado inactivo");

            Salario = nuevoSalario;
        }

        public void CambiarEstado(bool activo)
        {
            if (Activo == activo)
                throw new InvalidOperationException($"El empleado ya está {(activo ? "activo" : "inactivo")}");

            Activo = activo;
        }

        public void ActualizarCargo(string nuevoCargo)
        {
            if (string.IsNullOrWhiteSpace(nuevoCargo))
                throw new ArgumentException("El cargo no puede estar vacío");

            var cargosValidos = new[] { "Piloto", "Copiloto", "Azafata", "Técnico", "Administrativo", "Seguridad" };
            if (!Array.Exists(cargosValidos, c => c.Equals(nuevoCargo, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Cargo no válido. Cargos permitidos: {string.Join(", ", cargosValidos)}");

            if (!Activo && (nuevoCargo.Equals("Piloto", StringComparison.OrdinalIgnoreCase) ||
                            nuevoCargo.Equals("Copiloto", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("No se puede asignar cargo de vuelo a empleado inactivo");

            Cargo = nuevoCargo.Trim();
        }

        public static class EmpleadoRepository
        {
            private static readonly string RutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Empleados\Empleados.json";

            public static void GuardarEmpleados(List<Empleado> empleados)
            {
                string json = JsonConvert.SerializeObject(empleados, Formatting.Indented);
                File.WriteAllText(RutaArchivo, json);
            }

            public static List<Empleado> CargarEmpleados()
            {
                if (!File.Exists(RutaArchivo))
                    return new List<Empleado>();

                string json = File.ReadAllText(RutaArchivo);
                return JsonConvert.DeserializeObject<List<Empleado>>(json);
            }
        }
    }

}