using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public class Reserva
{
    public string CodigoReserva { get; private set; }
    public string DNI_Pasajero { get; private set; }
    public string CodigoVuelo { get; private set; }
    public DateTime FechaReserva { get; private set; }
    public string Estado { get; private set; } 
    public string TipoAsiento { get; private set; } 
    public decimal PrecioTotal { get; private set; }
    public List<string> ServiciosAdicionales { get; private set; }
    public bool CheckInRealizado { get; private set; }
    public string CodigoEmpleadoCheckIn { get; private set; }
    public DateTime? FechaCheckIn { get; private set; }

    public Reserva(string dni, string codigoVuelo, string tipoAsiento, decimal precio)
    {
        ValidarDNI(dni);
        ValidarCodigoVuelo(codigoVuelo);
        ValidarTipoAsiento(tipoAsiento);
        ValidarPrecio(precio);

        CodigoReserva = GenerarCodigoReserva();
        DNI_Pasajero = dni;
        CodigoVuelo = codigoVuelo;
        FechaReserva = DateTime.Now;
        Estado = "Confirmada";
        TipoAsiento = tipoAsiento;
        PrecioTotal = precio;
        ServiciosAdicionales = new List<string>();
        CheckInRealizado = false;
    }

    private void ValidarDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            throw new ArgumentException("El DNI no puede estar vacío");

        if (!Regex.IsMatch(dni, @"^[0-9]{8}[A-Za-z]$"))
            throw new ArgumentException("Formato de DNI inválido (8 dígitos + letra)");
    }

    private void ValidarCodigoVuelo(string codigoVuelo)
    {
        if (string.IsNullOrWhiteSpace(codigoVuelo))
            throw new ArgumentException("El código de vuelo no puede estar vacío");

        if (!Regex.IsMatch(codigoVuelo, @"^[A-Z]{2}\d{3,4}$"))
            throw new ArgumentException("Formato de vuelo inválido (Ej: AV123 o AV1234)");
    }

    private void ValidarTipoAsiento(string tipoAsiento)
    {
        var tiposPermitidos = new List<string> { "Turista", "Ejecutivo", "Primera" };

        if (string.IsNullOrWhiteSpace(tipoAsiento))
            throw new ArgumentException("El tipo de asiento no puede estar vacío");

        if (!tiposPermitidos.Contains(tipoAsiento))
            throw new ArgumentException($"Tipo de asiento inválido. Use: {string.Join(", ", tiposPermitidos)}");
    }

    private void ValidarPrecio(decimal precio)
    {
        if (precio <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero");

        if (precio > 10000)
            throw new ArgumentException("El precio no puede exceder $10,000");

        if (decimal.Round(precio, 2) != precio)
            throw new ArgumentException("El precio debe tener máximo 2 decimales");
    }

    private string GenerarCodigoReserva()
    {
        return $"RES-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(100, 999)}";
    }
    public void CancelarReserva(string motivo = null)
    {
        if (Estado == "Cancelada")
            throw new InvalidOperationException("La reserva ya está cancelada");

        if (Estado == "Completada")
            throw new InvalidOperationException("No se puede cancelar una reserva completada");

        Estado = "Cancelada";
    }

    public void RealizarCheckIn(string codigoEmpleado)
    {
        if (Estado != "Confirmada")
            throw new InvalidOperationException($"No se puede hacer check-in en estado: {Estado}");

        if (string.IsNullOrWhiteSpace(codigoEmpleado))
            throw new ArgumentException("Código de empleado requerido");

        CheckInRealizado = true;
        Estado = "CheckIn";
        CodigoEmpleadoCheckIn = codigoEmpleado;
        FechaCheckIn = DateTime.Now;
    }

    public void AgregarServicio(string servicio, decimal precioExtra)
    {
        if (Estado == "Cancelada" || Estado == "Completada")
            throw new InvalidOperationException($"No se pueden agregar servicios en estado: {Estado}");

        if (string.IsNullOrWhiteSpace(servicio))
            throw new ArgumentException("El nombre del servicio no puede estar vacío");

        ValidarPrecio(precioExtra);

        ServiciosAdicionales.Add(servicio);
        PrecioTotal += precioExtra;
    }

    public void CompletarReserva()
    {
        if (Estado != "CheckIn")
            throw new InvalidOperationException($"Solo se puede completar una reserva en estado CheckIn");

        Estado = "Completada";
    }
    public void Guardar()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Reservas\RESERVAS.JSON";

        List<Reserva> reservas = CargarTodas();
        var existente = reservas.FirstOrDefault(r => r.CodigoReserva == this.CodigoReserva);

        if (existente != null)
            reservas.Remove(existente);

        reservas.Add(this);

        string json = JsonConvert.SerializeObject(reservas, Formatting.Indented);
        File.WriteAllText(ruta, json);
    }

    public static List<Reserva> CargarTodas()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Reservas\RESERVAS.JSON";

        if (!File.Exists(ruta))
            return new List<Reserva>();

        try
        {
            string json = File.ReadAllText(ruta);
            return JsonConvert.DeserializeObject<List<Reserva>>(json) ?? new List<Reserva>();
        }
        catch (JsonException)
        {
            File.Move(ruta, $"{ruta}.corrupto_{DateTime.Now:yyyyMMddHHmmss}");
            return new List<Reserva>();
        }
    }

    public static Reserva Cargar(string codigoReserva)
    {
        var reservas = CargarTodas();
        var reserva = reservas.FirstOrDefault(r => r.CodigoReserva == codigoReserva);

        if (reserva == null)
            throw new FileNotFoundException("Reserva no encontrada");

        return reserva;
    }

    public static List<Reserva> CargarPorPasajero(string dni)
    {
        return CargarTodas().Where(r => r.DNI_Pasajero == dni).ToList();
    }
}