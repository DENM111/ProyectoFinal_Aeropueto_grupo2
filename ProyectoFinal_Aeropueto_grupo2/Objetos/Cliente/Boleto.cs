using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ProyectoFinal_Aeropueto_grupo2.Objetos.ADMIN;

public class Boleto
{
    public string CodigoBoleto { get; set; }
    public string CodigoReserva { get; set; }
    public string DNI_Pasajero { get; set; }
    public string NombrePasajero { get; set; }
    public string ApellidosPasajero { get; set; }
    public int NumeroMaletas { get; set; }
    public double PesoTotalMaletas { get; set; }
    public string Vuelo { get; set; }
    public DateTime FechaHoraSalida { get; set; }
    public string Asiento { get; set; }
    public string PuertaEmbarque { get; set; }
    public string QRCode { get; set; }
    public string Estado { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaUso { get; set; }

    public Boleto() { }

    public Boleto(Reserva reserva, Vuelo vuelo, Pasajero pasajero, string asiento)
    {
        ValidarReserva(reserva);
        ValidarVuelo(vuelo);
        ValidarPasajero(pasajero);
        ValidarAsiento(asiento);

        CodigoBoleto = GenerarCodigoBoleto();
        CodigoReserva = reserva.CodigoReserva;
        DNI_Pasajero = pasajero.DNI;
        NombrePasajero = $"{pasajero.Nombre} {pasajero.Apellido}";
        Vuelo = vuelo.CodigoVuelo;
        FechaHoraSalida = vuelo.FechaHoraSalida;
        Asiento = asiento;
        PuertaEmbarque = GenerarPuertaEmbarque();
        Estado = "Activo";
        FechaEmision = DateTime.Now;
    }

    private void ValidarReserva(Reserva reserva)
    {
        if (reserva == null)
            throw new ArgumentNullException("La reserva no puede ser nula");

        if (string.IsNullOrWhiteSpace(reserva.CodigoReserva))
            throw new ArgumentException("Código de reserva inválido");

        if (!Regex.IsMatch(reserva.CodigoReserva, @"^RES-\d{14}$"))
            throw new ArgumentException("Formato de código de reserva inválido");
    }

    private void ValidarVuelo(Vuelo vuelo)
    {
        if (vuelo == null)
            throw new ArgumentNullException("El vuelo no puede ser nulo");

        if (vuelo.FechaHoraSalida < DateTime.Now.AddMinutes(30))
            throw new ArgumentException("El vuelo debe estar programado al menos 30 minutos después");
    }

    private void ValidarPasajero(Pasajero pasajero)
    {
        if (pasajero == null)
            throw new ArgumentNullException("El pasajero no puede ser nulo");

        if (string.IsNullOrWhiteSpace(pasajero.DNI))
            throw new ArgumentException("DNI del pasajero inválido");

        if (pasajero.FechaNacimiento > DateTime.Now.AddYears(-18))
            throw new ArgumentException("El pasajero debe ser mayor de edad");
    }

    private void ValidarAsiento(string asiento)
    {
        if (string.IsNullOrWhiteSpace(asiento))
            throw new ArgumentException("El asiento no puede estar vacío");

        if (!Regex.IsMatch(asiento, @"^[A-Za-z]\d{1,2}$"))
            throw new ArgumentException("Formato de asiento inválido (Ej: A12)");
    }

    private string GenerarCodigoBoleto()
    {
        return $"BOL-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
    }

    private string GenerarPuertaEmbarque()
    {
        return $"G{new Random().Next(1, 30)}";
    }

    public void MarcarComoUsado()
    {
        if (Estado != "Activo")
            throw new InvalidOperationException($"No se puede usar un boleto en estado: {Estado}");

        Estado = "Usado";
        FechaUso = DateTime.Now;
    }

    public void Cancelar(string motivo)
    {
        if (Estado == "Cancelado" || Estado == "Usado")
            throw new InvalidOperationException($"No se puede cancelar un boleto en estado: {Estado}");

        Estado = "Cancelado";
    }

    public bool EsValido()
    {
        return Estado == "Activo" && FechaHoraSalida > DateTime.Now;
    }

    public void Guardar()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Boletos\Boletos.JSON";

        List<Boleto> listaBoletos = CargarTodos();
        var existente = listaBoletos.FirstOrDefault(b => b.CodigoBoleto == this.CodigoBoleto);

        if (existente != null)
            listaBoletos.Remove(existente);

        listaBoletos.Add(this);

        string json = JsonConvert.SerializeObject(listaBoletos, Formatting.Indented);
        File.WriteAllText(ruta, json);
    }

    public static List<Boleto> CargarTodos()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Boletos\Boletos.JSON";

        if (!File.Exists(ruta))
            return new List<Boleto>();

        try
        {
            string json = File.ReadAllText(ruta);
            return JsonConvert.DeserializeObject<List<Boleto>>(json) ?? new List<Boleto>();
        }
        catch (JsonException)
        {
            File.Move(ruta, $"{ruta}.corrupto_{DateTime.Now:yyyyMMddHHmmss}");
            return new List<Boleto>();
        }
    }

    public static Boleto Cargar(string codigoBoleto)
    {
        if (string.IsNullOrWhiteSpace(codigoBoleto))
            throw new ArgumentException("El código de boleto no puede estar vacío");

        var boletos = CargarTodos();
        var boleto = boletos.FirstOrDefault(b => b.CodigoBoleto == codigoBoleto);

        if (boleto == null)
            throw new FileNotFoundException($"No se encontró el boleto con código {codigoBoleto}");

        return boleto;
    }
}