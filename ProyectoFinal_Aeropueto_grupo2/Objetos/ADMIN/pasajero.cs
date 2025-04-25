using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN
{
    public class Pasajero : Persona
    {
        public string RTN { get; private set; }
        public DateTime FechaNacimiento { get; private set; }
        public string Nacionalidad { get; private set; }
        public string NumeroPasaporte { get; private set; }
        public List<string> VuelosReservados { get; private set; }
        public string DNI { get; internal set; }

        public Pasajero(string rtn, string nombre, string apellido, DateTime fechaNacimiento,
                        string nacionalidad, string email, string telefono, string pasaporte)
            : base(nombre, apellido, email, telefono)
        {
            if (!ValidarRTN(rtn))
                throw new ArgumentException("RTN no válido. Debe tener 14 dígitos");

            if (fechaNacimiento > DateTime.Now.AddYears(-2))
                throw new ArgumentException("El pasajero debe tener al menos 2 años");

            if (fechaNacimiento < DateTime.Now.AddYears(-120))
                throw new ArgumentException("Fecha de nacimiento no válida");

            if (string.IsNullOrWhiteSpace(nacionalidad))
                throw new ArgumentException("Nacionalidad requerida");

            if (!string.IsNullOrWhiteSpace(pasaporte) && !ValidarPasaporte(pasaporte))
                throw new ArgumentException("Pasaporte no válido. Formato: 2 letras + 7 dígitos");

            RTN = rtn;
            FechaNacimiento = fechaNacimiento;
            Nacionalidad = nacionalidad.Trim();
            NumeroPasaporte = pasaporte?.ToUpper().Trim();
            VuelosReservados = new List<string>();
        }

        private bool ValidarRTN(string rtn)
        {
            return Regex.IsMatch(rtn, @"^\d{14}$");
        }

        private bool ValidarPasaporte(string pasaporte)
        {
            return Regex.IsMatch(pasaporte, @"^[A-Z]{2}\d{7}$");
        }

        public void AgregarVuelo(string codigoVuelo)
        {
            if (string.IsNullOrWhiteSpace(codigoVuelo) || !Regex.IsMatch(codigoVuelo, @"^[A-Z]{2}\d{4}$"))
                throw new ArgumentException("Código de vuelo inválido");

            if (VuelosReservados.Contains(codigoVuelo))
                throw new InvalidOperationException("El pasajero ya tiene reservado este vuelo");

            VuelosReservados.Add(codigoVuelo);
        }

        public void EliminarVuelo(string codigoVuelo)
        {
            if (!VuelosReservados.Contains(codigoVuelo))
                throw new InvalidOperationException("El pasajero no tiene reservado este vuelo");

            VuelosReservados.Remove(codigoVuelo);
        }

        public static class PasajeroRepository
        {
            private static readonly string RutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Pasajero\PASAJERO.JSON";

            public static void GuardarPasajeros(List<Pasajero> pasajeros)
            {
                string json = JsonConvert.SerializeObject(pasajeros, Formatting.Indented);
                File.WriteAllText(RutaArchivo, json);
            }

            public static List<Pasajero> CargarPasajeros()
            {
                if (!File.Exists(RutaArchivo))
                    return new List<Pasajero>();

                string json = File.ReadAllText(RutaArchivo);
                return JsonConvert.DeserializeObject<List<Pasajero>>(json);
            }
        }
    }

}