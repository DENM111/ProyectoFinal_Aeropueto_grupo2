using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN
{
    public class Avion
    {
        public int IdAvion { get; private set; }
        public string Modelo { get; set; }
        public string Fabricante { get; private set; }
        public int CapacidadPasajeros { get; set; }
        public int CapacidadCombustible { get; private set; } 
        public DateTime FechaFabricacion { get; private set; }
        public DateTime UltimoMantenimiento { get; private set; }
        public string Estado { get; private set; }
        public string CodigoIATA { get; private set; }

        public Avion(int id, string modelo, string fabricante, int capacidadPasajeros,
                    int capacidadCombustible, DateTime fechaFabricacion, string codigoIATA)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del avión debe ser positivo");

            if (string.IsNullOrWhiteSpace(modelo))
                throw new ArgumentException("El modelo es requerido");

            if (modelo.Length > 50)
                throw new ArgumentException("El modelo no puede exceder los 50 caracteres");

            if (string.IsNullOrWhiteSpace(fabricante))
                throw new ArgumentException("El fabricante es requerido");

            if (fabricante.Length > 50)
                throw new ArgumentException("El fabricante no puede exceder los 50 caracteres");

            if (capacidadPasajeros <= 0)
                throw new ArgumentException("La capacidad de pasajeros debe ser positiva");

            if (capacidadPasajeros > 1000)
                throw new ArgumentException("La capacidad de pasajeros no puede exceder 1000");

            if (capacidadCombustible <= 0)
                throw new ArgumentException("La capacidad de combustible debe ser positiva");

            if (capacidadCombustible > 500000) 
                throw new ArgumentException("La capacidad de combustible no puede exceder 500,000 litros");

            if (fechaFabricacion > DateTime.Now)
                throw new ArgumentException("La fecha de fabricación no puede ser futura");

            if (fechaFabricacion < new DateTime(1950, 1, 1))
                throw new ArgumentException("La fecha de fabricación no puede ser anterior a 1950");

            if (string.IsNullOrWhiteSpace(codigoIATA))
                throw new ArgumentException("El código IATA es requerido");

            if (!Regex.IsMatch(codigoIATA, @"^[A-Z]{3}$"))
                throw new ArgumentException("El código IATA debe tener exactamente 3 letras mayúsculas");

            IdAvion = id;
            Modelo = modelo;
            Fabricante = fabricante;
            CapacidadPasajeros = capacidadPasajeros;
            CapacidadCombustible = capacidadCombustible;
            FechaFabricacion = fechaFabricacion;
            UltimoMantenimiento = DateTime.Now;
            Estado = "Disponible";
            CodigoIATA = codigoIATA;
        }

        public void ActualizarMantenimiento()
        {
            if (Estado == "En Vuelo")
                throw new InvalidOperationException("No se puede realizar mantenimiento a un avión en vuelo");

            UltimoMantenimiento = DateTime.Now;
            Estado = "Disponible";
        }

        public void CambiarEstado(string nuevoEstado)
        {
            string[] estadosValidos = { "Disponible", "Mantenimiento", "Reparación", "En Vuelo", "Retirado" };

            if (Array.IndexOf(estadosValidos, nuevoEstado) == -1)
                throw new ArgumentException($"Estado no válido. Estados permitidos: {string.Join(", ", estadosValidos)}");

            if (Estado == "Retirado" && nuevoEstado != "Retirado")
                throw new InvalidOperationException("No se puede cambiar el estado de un avión retirado");

            if (Estado == "En Vuelo" && (nuevoEstado == "Mantenimiento" || nuevoEstado == "Reparación"))
                throw new InvalidOperationException("No se puede poner en mantenimiento/reparación un avión en vuelo");

            Estado = nuevoEstado;
        }

        public void RetirarAvion()
        {
            if (Estado == "En Vuelo")
                throw new InvalidOperationException("No se puede retirar un avión en vuelo");

            Estado = "Retirado";
            UltimoMantenimiento = DateTime.Now;
        }

        public void ActualizarCapacidad(int nuevaCapacidadPasajeros, int nuevaCapacidadCombustible)
        {
            if (Estado == "En Vuelo")
                throw new InvalidOperationException("No se puede modificar la capacidad de un avión en vuelo");

            if (nuevaCapacidadPasajeros <= 0 || nuevaCapacidadPasajeros > 1000)
                throw new ArgumentException("Capacidad de pasajeros inválida");

            if (nuevaCapacidadCombustible <= 0 || nuevaCapacidadCombustible > 500000)
                throw new ArgumentException("Capacidad de combustible inválida");

            CapacidadPasajeros = nuevaCapacidadPasajeros;
            CapacidadCombustible = nuevaCapacidadCombustible;
        }

        public static class AvionRepository
        {
            private static readonly string RutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\DATA\ADMIN\Aviones\Aviones.JSON";

            public static void GuardarAviones(List<Avion> aviones)
            {
                string json = JsonConvert.SerializeObject(aviones, Formatting.Indented);
                File.WriteAllText(RutaArchivo, json);
            }

            public static List<Avion> CargarAviones()
            {
                if (!File.Exists(RutaArchivo))
                    return new List<Avion>();

                string json = File.ReadAllText(RutaArchivo);
                return JsonConvert.DeserializeObject<List<Avion>>(json);
            }
        }
    }
}