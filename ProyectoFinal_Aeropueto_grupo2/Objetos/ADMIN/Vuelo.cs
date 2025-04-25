using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN
{
    public class Vuelo
    {
        public string CodigoVuelo { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public DateTime FechaHoraLlegada { get; set; }
        public int IdAvion { get; set; }
        public int AsientosDisponibles { get; set; }
        public int AsientosTotales { get; set; }
        public string Estado { get; set; }
        public decimal PrecioBase { get; set; }

        public Vuelo(string codigo, string origen, string destino, DateTime salida,
                    DateTime llegada, int idAvion, int asientosTotales, decimal precioBase)
        {
            if (string.IsNullOrWhiteSpace(codigo) || !Regex.IsMatch(codigo, @"^[A-Z]{2}\d{4}$"))
                throw new ArgumentException("Código de vuelo inválido. Formato: 2 letras + 4 dígitos (ej: LH1234)");

            if (string.IsNullOrWhiteSpace(origen) || origen.Length != 3)
                throw new ArgumentException("Código de origen inválido. Debe ser código IATA de 3 letras");

            if (string.IsNullOrWhiteSpace(destino) || destino.Length != 3)
                throw new ArgumentException("Código de destino inválido. Debe ser código IATA de 3 letras");

            if (salida >= llegada)
                throw new ArgumentException("La hora de salida debe ser anterior a la de llegada");

            if (salida < DateTime.Now.AddHours(-1))
                throw new ArgumentException("No se puede programar vuelos en el pasado");

            if (salida > DateTime.Now.AddYears(1))
                throw new ArgumentException("No se puede programar vuelos con más de 1 año de anticipación");

            if (idAvion <= 0)
                throw new ArgumentException("ID de avión inválido");

            if (asientosTotales < 20 || asientosTotales > 500)
                throw new ArgumentException("La capacidad debe estar entre 20 y 500 asientos");

            if (precioBase <= 0)
                throw new ArgumentException("El precio debe ser positivo");

            if (precioBase > 50000) 
                throw new ArgumentException("El precio no puede exceder L. 50,000");

            CodigoVuelo = codigo.ToUpper();
            Origen = origen.ToUpper();
            Destino = destino.ToUpper();
            FechaHoraSalida = salida;
            FechaHoraLlegada = llegada;
            IdAvion = idAvion;
            AsientosTotales = asientosTotales;
            AsientosDisponibles = asientosTotales;
            Estado = "Programado";
            PrecioBase = precioBase;
        }

        public void ActualizarEstado(string nuevoEstado)
        {
            string[] estadosValidos = { "Programado", "En curso", "Cancelado", "Completado", "Retrasado" };

            if (Array.IndexOf(estadosValidos, nuevoEstado) == -1)
                throw new ArgumentException($"Estado no válido. Estados permitidos: {string.Join(", ", estadosValidos)}");

            if (Estado == "Completado" && nuevoEstado != "Completado")
                throw new InvalidOperationException("No se puede modificar un vuelo completado");

            if (Estado == "Cancelado" && nuevoEstado != "Cancelado")
                throw new InvalidOperationException("No se puede modificar un vuelo cancelado");

            Estado = nuevoEstado;
        }

        public bool ReservarAsientos(int cantidad)
        {
            if (Estado != "Programado")
                throw new InvalidOperationException("Solo se pueden reservar asientos en vuelos programados");

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser positiva");

            if (AsientosDisponibles < cantidad)
                return false;

            AsientosDisponibles -= cantidad;
            return true;
        }

        public void LiberarAsientos(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser positiva");

            AsientosDisponibles = Math.Min(AsientosDisponibles + cantidad, AsientosTotales);
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (Estado != "Programado")
                throw new InvalidOperationException("Solo se puede actualizar precio en vuelos programados");

            if (nuevoPrecio <= 0 || nuevoPrecio > 50000)
                throw new ArgumentException("Precio inválido. Debe estar entre L. 1 y L. 50,000");

            PrecioBase = nuevoPrecio;
        }

        public static class VueloRepository
        {
            private static readonly string RutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Vuelos\Vuelos.JSON";

            public static void GuardarVuelos(List<Vuelo> vuelos)
            {
                string json = JsonConvert.SerializeObject(vuelos, Formatting.Indented);
                File.WriteAllText(RutaArchivo, json);
            }

            public static List<Vuelo> CargarVuelos()
            {
                if (!File.Exists(RutaArchivo))
                    return new List<Vuelo>();

                string json = File.ReadAllText(RutaArchivo);
                return JsonConvert.DeserializeObject<List<Vuelo>>(json);
            }
        }
    }
}