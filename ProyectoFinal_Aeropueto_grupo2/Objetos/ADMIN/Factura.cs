using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN
{
    public class Factura
    {
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; private set; }
        public string DNI_Pasajero { get; private set; }
        public string NombrePasajero { get; set; }
        public List<string> Vuelos { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal Impuestos { get; private set; }
        public decimal Total { get; private set; }
        public string MetodoPago { get; private set; }
        public string Estado { get; private set; }
        public string Numero { get; private set; }
        public string Nombre { get; private set; }

        public Factura(string numero, string dni, string nombre, List<string> vuelos,
                      decimal subtotal, string metodoPago)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("El número de factura es requerido");

            if (!Regex.IsMatch(numero, @"^\d{3}-\d{3}-\d{2}-\d{8}$"))
                throw new ArgumentException("Formato de factura inválido. Debe ser: 000-000-00-00000000");

            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El RTN (DNI) del pasajero es requerido");

            if (!Regex.IsMatch(dni, @"^\d{12}$"))
                throw new ArgumentException("RTN (DNI) inválido. Debe tener 12 dígitos");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del pasajero es requerido");

            if (nombre.Length > 100)
                throw new ArgumentException("El nombre no puede exceder los 100 caracteres");

            if (vuelos == null || vuelos.Count == 0)
                throw new ArgumentException("Debe incluir al menos un vuelo en la factura");

            foreach (var vuelo in vuelos)
            {
                if (string.IsNullOrWhiteSpace(vuelo) || !Regex.IsMatch(vuelo, @"^[A-Z]{2}\d{4}$"))
                    throw new ArgumentException($"Código de vuelo inválido: {vuelo}. Formato correcto: 2 letras + 4 dígitos");
            }

            if (subtotal <= 0)
                throw new ArgumentException("El subtotal debe ser positivo");

            if (subtotal > 1000000)
                throw new ArgumentException("El subtotal no puede exceder los 1,000,000 HNL");

            if (string.IsNullOrWhiteSpace(metodoPago))
                throw new ArgumentException("El método de pago es requerido");

            var metodosValidos = new[] { "Tarjeta", "Efectivo", "Transferencia", "PayPal", "Depósito" };
            if (!Array.Exists(metodosValidos, m => m.Equals(metodoPago, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Método de pago no válido. Métodos aceptados: {string.Join(", ", metodosValidos)}");

            NumeroFactura = numero;
            FechaEmision = DateTime.Now;
            DNI_Pasajero = dni;
            NombrePasajero = nombre.Trim();
            Vuelos = vuelos;
            Subtotal = subtotal;
            Impuestos = CalcularImpuestosHonduras(subtotal);
            Total = Subtotal + Impuestos;
            MetodoPago = metodoPago;
            Estado = "Pendiente";
        }

        public Factura(string numero, string nombre, List<string> vuelos, decimal subtotal, string metodoPago, string NumeroFactura, string NombrePasajero)
        {
            Numero = numero;
            Nombre = nombre;
            Vuelos = vuelos;
            Subtotal = subtotal;
            MetodoPago = metodoPago;
        }

        private decimal CalcularImpuestosHonduras(decimal subtotal)
        {
            const decimal isvGeneral = 0.15m;
            const decimal isvTurismo = 0.015m;

            return subtotal * (isvGeneral + isvTurismo);
        }

        public void PagarFactura()
        {
            if (Estado == "Pagada")
                throw new InvalidOperationException("La factura ya está pagada");

            if (Estado == "Cancelada")
                throw new InvalidOperationException("No se puede pagar una factura cancelada");

            Estado = "Pagada";
            FechaEmision = DateTime.Now;
        }

        public void CancelarFactura()
        {
            if (Estado == "Pagada")
                throw new InvalidOperationException("No se puede cancelar una factura ya pagada");

            if (Estado == "Cancelada")
                throw new InvalidOperationException("La factura ya está cancelada");

            Estado = "Cancelada";
        }

        public void AgregarVuelo(string codigoVuelo, decimal precio)
        {
            if (Estado != "Pendiente")
                throw new InvalidOperationException("Solo se pueden agregar vuelos a facturas pendientes");

            if (string.IsNullOrWhiteSpace(codigoVuelo) || !Regex.IsMatch(codigoVuelo, @"^[A-Z]{2}\d{4}$"))
                throw new ArgumentException($"Código de vuelo inválido. Formato correcto: 2 letras + 4 dígitos");

            if (precio <= 0)
                throw new ArgumentException("El precio debe ser positivo");

            Vuelos.Add(codigoVuelo);
            Subtotal += precio;
            Impuestos = CalcularImpuestosHonduras(Subtotal);
            Total = Subtotal + Impuestos;
        }

        public void ActualizarMetodoPago(string nuevoMetodo)
        {
            if (Estado != "Pendiente")
                throw new InvalidOperationException("Solo se puede cambiar el método de pago en facturas pendientes");

            var metodosValidos = new[] { "Tarjeta", "Efectivo", "Transferencia", "PayPal", "Depósito" };
            if (!Array.Exists(metodosValidos, m => m.Equals(nuevoMetodo, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Método de pago no válido. Métodos aceptados: {string.Join(", ", metodosValidos)}");

            MetodoPago = nuevoMetodo;
        }

        public static class FacturaRepository
        {
            private static readonly string RutaArchivo = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\ADMIN\Factura\Factura.json";

            public static void GuardarFacturas(List<Factura> facturas)
            {
                string json = JsonConvert.SerializeObject(facturas, Formatting.Indented);
                File.WriteAllText(RutaArchivo, json);
            }

            public static List<Factura> CargarFacturas()
            {
                try
                {
                    if (!File.Exists(RutaArchivo))
                        return new List<Factura>();

                    string json = File.ReadAllText(RutaArchivo);
                    var lista = JsonConvert.DeserializeObject<List<Factura>>(json);

                    return lista ?? new List<Factura>(); 
                }
                catch
                {
                    return new List<Factura>();
                }
            }
        }
    }
}