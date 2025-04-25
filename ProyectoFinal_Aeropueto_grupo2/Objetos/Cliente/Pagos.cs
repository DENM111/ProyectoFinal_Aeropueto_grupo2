using System;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

public class Pago
{
    public string CodigoPago { get; private set; }
    public string CodigoReserva { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime FechaPago { get; private set; }
    public string Metodo { get; private set; }
    public string Estado { get; private set; }
    public string CodigoTransaccion { get; private set; }
    public string DetallesTarjeta { get; private set; }

    public Pago(string codigoReserva, decimal monto, string metodo)
    {
        if (string.IsNullOrWhiteSpace(codigoReserva))
            throw new ArgumentException("El código de reserva no puede estar vacío");

        if (monto <= 0)
            throw new ArgumentException("El monto debe ser un valor positivo");

        if (string.IsNullOrWhiteSpace(metodo))
            throw new ArgumentException("Debe especificar un método de pago");

        var metodosValidos = new[] { "Tarjeta", "Efectivo", "Transferencia", "PayPal" };
        if (!metodosValidos.Contains(metodo))
            throw new ArgumentException($"Método de pago no válido. Los métodos aceptados son: {string.Join(", ", metodosValidos)}");

        CodigoPago = $"PAY-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4)}";
        CodigoReserva = codigoReserva;
        Monto = monto;
        FechaPago = DateTime.Now;
        Metodo = metodo;
        Estado = "Pendiente"; 
    }

    public void SetDetallesTarjeta(string numeroTarjeta, string fechaVencimiento, string cvv, string nombreTitular)
    {
        if (Metodo != "Tarjeta")
            throw new InvalidOperationException("Los detalles de tarjeta solo son válidos para pagos con tarjeta");

        if (string.IsNullOrWhiteSpace(numeroTarjeta) || numeroTarjeta.Length != 16 || !numeroTarjeta.All(char.IsDigit))
            throw new ArgumentException("Número de tarjeta inválido. Debe tener 16 dígitos");

        if (string.IsNullOrWhiteSpace(fechaVencimiento) || !fechaVencimiento.Contains("/"))
            throw new ArgumentException("Formato de fecha de vencimiento inválido. Use MM/YY");

        if (string.IsNullOrWhiteSpace(cvv) || (cvv.Length != 3 && cvv.Length != 4) || !cvv.All(char.IsDigit))
            throw new ArgumentException("CVV inválido. Debe tener 3 o 4 dígitos");

        if (string.IsNullOrWhiteSpace(nombreTitular))
            throw new ArgumentException("El nombre del titular no puede estar vacío");

    }

    public void SetCodigoTransaccion(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código de transacción no puede estar vacío");

        if (codigo.Length < 8 || codigo.Length > 20)
            throw new ArgumentException("El código de transacción debe tener entre 8 y 20 caracteres");

        CodigoTransaccion = codigo;
    }

    public void CambiarEstado(string nuevoEstado)
    {
        var estadosValidos = new[] { "Pendiente", "Completado", "Rechazado", "Reembolsado", "Cancelado" };
        if (!estadosValidos.Contains(nuevoEstado))
            throw new ArgumentException($"Estado de pago no válido. Estados válidos: {string.Join(", ", estadosValidos)}");

        if (Estado == "Completado" && nuevoEstado == "Pendiente")
            throw new InvalidOperationException("No se puede revertir un pago completado a pendiente");

        Estado = nuevoEstado;

        if (nuevoEstado == "Completado")
            FechaPago = DateTime.Now;
    }


    public void ConfirmarPago(string codigoTransaccion)
    {
        if (Estado != "Pendiente")
            throw new InvalidOperationException("Solo se pueden confirmar pagos pendientes");

        SetCodigoTransaccion(codigoTransaccion);
        CambiarEstado("Completado");
    }

    public void Guardar()
    {
        if (Estado == "Completado" && string.IsNullOrEmpty(CodigoTransaccion))
            throw new InvalidOperationException("Falta el código de transacción para un pago completado");

        if (Metodo == "Tarjeta" && string.IsNullOrEmpty(DetallesTarjeta))
            throw new InvalidOperationException("Faltan los detalles de la tarjeta para completar el pago");

        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Pagos\pagos.JSON";

        List<Pago> pagos = CargarTodos();
        var existente = pagos.FirstOrDefault(p => p.CodigoPago == this.CodigoPago);

        if (existente != null)
            pagos.Remove(existente);

        pagos.Add(this);

        string json = JsonConvert.SerializeObject(pagos, Formatting.Indented);
        File.WriteAllText(ruta, json);
    }

    public static List<Pago> CargarTodos()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Pagos\pagos.JSON";

        if (!File.Exists(ruta))
            return new List<Pago>();

        try
        {
            string json = File.ReadAllText(ruta);
            return JsonConvert.DeserializeObject<List<Pago>>(json) ?? new List<Pago>();
        }
        catch (JsonException)
        {
            File.Move(ruta, $"{ruta}.corrupto_{DateTime.Now:yyyyMMddHHmmss}");
            return new List<Pago>();
        }
    }

    public static Pago Cargar(string codigoPago)
    {
        if (string.IsNullOrWhiteSpace(codigoPago))
            throw new ArgumentException("El código de pago no puede estar vacío");

        var pagos = CargarTodos();
        var pago = pagos.FirstOrDefault(p => p.CodigoPago == codigoPago);

        if (pago == null)
            throw new FileNotFoundException($"No se encontró el pago con código {codigoPago}");

        return pago;
    }
}