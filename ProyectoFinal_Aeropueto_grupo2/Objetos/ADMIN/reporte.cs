using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN
{
    public class Reporte
    {
        public int IdReporte { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFin { get; set; }
        public Dictionary<string, int> DatosNumericos { get; set; }
        public Dictionary<string, string> DatosTexto { get; set; }
        public string Resumen { get; set; }
        public string GeneradoPor { get; set; }

        public Reporte(int id, string tipo, DateTime inicio, DateTime fin, string generadoPor)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del reporte debe ser positivo");

            if (string.IsNullOrWhiteSpace(tipo))
                throw new ArgumentException("El tipo de reporte es requerido");

            var tiposValidos = new[] { "Vuelos", "Pasajeros", "Financiero", "Mantenimiento", "Operaciones" };
            if (!Array.Exists(tiposValidos, t => t.Equals(tipo, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Tipo de reporte no válido. Tipos permitidos: {string.Join(", ", tiposValidos)}");

            if (inicio > fin)
                throw new ArgumentException("La fecha de inicio no puede ser posterior a la fecha fin");

            if (inicio < new DateTime(2025, 1, 1)) 
                throw new ArgumentException("La fecha de inicio no puede ser anterior al año 2020");

            if (fin > DateTime.Now.AddDays(1)) 
                throw new ArgumentException("La fecha fin no puede ser futura");

            if (string.IsNullOrWhiteSpace(generadoPor))
                throw new ArgumentException("El nombre de quien genera el reporte es requerido");

            if (generadoPor.Length > 100)
                throw new ArgumentException("El nombre no puede exceder los 100 caracteres");

            IdReporte = id;
            Tipo = tipo;
            FechaGeneracion = DateTime.Now;
            PeriodoInicio = inicio;
            PeriodoFin = fin;
            DatosNumericos = new Dictionary<string, int>();
            DatosTexto = new Dictionary<string, string>();
            Resumen = string.Empty;
            GeneradoPor = generadoPor;
        }

        public void AgregarDatoNumerico(string clave, int valor)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del dato numérico no puede estar vacía");

            if (clave.Length > 50)
                throw new ArgumentException("La clave no puede exceder los 50 caracteres");

            if (valor < 0)
                throw new ArgumentException("El valor numérico no puede ser negativo");

            DatosNumericos[clave] = valor;
        }

        public void AgregarDatoTexto(string clave, string valor)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del dato de texto no puede estar vacía");

            if (clave.Length > 50)
                throw new ArgumentException("La clave no puede exceder los 50 caracteres");

            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("El valor de texto no puede estar vacío");

            if (valor.Length > 500)
                throw new ArgumentException("El texto no puede exceder los 500 caracteres");

            DatosTexto[clave] = valor;
        }

        public void GenerarResumen()
        {
            string formatoFecha = "dd/MM/yyyy";

            Resumen = $"REPORTE DE {Tipo.ToUpper()}\n";
            Resumen += $"Período: Del {PeriodoInicio.ToString(formatoFecha)} al {PeriodoFin.ToString(formatoFecha)}\n";
            Resumen += $"Generado por: {GeneradoPor}\n";
            Resumen += $"Fecha de generación: {FechaGeneracion.ToString(formatoFecha)}\n\n";

            if (DatosNumericos.Count > 0)
            {
                Resumen += "ESTADÍSTICAS:\n";
                foreach (var dato in DatosNumericos)
                {
                    Resumen += $"- {dato.Key}: {dato.Value.ToString("N0")}\n"; 
                }
                Resumen += "\n";
            }

            if (DatosTexto.Count > 0)
            {
                Resumen += "OBSERVACIONES:\n";
                foreach (var dato in DatosTexto)
                {
                    Resumen += $"- {dato.Key}: {dato.Value}\n";
                }
            }

            if (Tipo.Equals("Financiero", StringComparison.OrdinalIgnoreCase))
            {
                Resumen = Resumen.Replace("Total:", "Total: L.");
            }
        }

        public static class ReporteRepository
        {
            private static readonly string RutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Reporte\Reporte.JSON";

            public static void GuardarReportes(List<Reporte> reportes)
            {
                string json = JsonConvert.SerializeObject(reportes, Formatting.Indented);
                File.WriteAllText(RutaArchivo, json);
            }

            public static List<Reporte> CargarReportes()
            {
                if (!File.Exists(RutaArchivo))
                    return new List<Reporte>();

                string json = File.ReadAllText(RutaArchivo);
                return JsonConvert.DeserializeObject<List<Reporte>>(json);
            }
        }
    }
}