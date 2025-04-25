using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public class Equipaje
{
    public string CodigoEquipaje { get; private set; }
    public string CodigoReserva { get; private set; }
    public decimal Peso { get; private set; } 
    public string Tipo { get; private set; } 
    public string Estado { get; private set; } 
    public string Descripcion { get; private set; }
    public DateTime FechaRegistro { get; private set; }
    public string CodigoEmpleado { get; private set; }
    public string Observaciones { get; private set; }

    public Equipaje(string codigoReserva, decimal peso, string tipo, string descripcion)
    {
        ValidarCodigoReserva(codigoReserva);
        ValidarPeso(peso);
        ValidarTipo(tipo);
        ValidarDescripcion(descripcion);

        CodigoEquipaje = GenerarCodigoEquipaje();
        CodigoReserva = codigoReserva;
        Peso = peso;
        Tipo = tipo;
        Descripcion = descripcion;
        Estado = "Registrado";
        FechaRegistro = DateTime.Now;
    }

    private void ValidarCodigoReserva(string codigoReserva)
    {
        if (string.IsNullOrWhiteSpace(codigoReserva))
            throw new ArgumentException("El código de reserva no puede estar vacío");

        if (!Regex.IsMatch(codigoReserva, @"^RES-\d{14}$"))
            throw new ArgumentException("Formato de código de reserva inválido (Ej: RES-20240520120000)");
    }

    private void ValidarPeso(decimal peso)
    {
        if (peso <= 0)
            throw new ArgumentException("El peso debe ser mayor a cero");

        if (peso > 50)
            throw new ArgumentException("El peso máximo permitido es 50kg");

        if (decimal.Round(peso, 2) != peso)
            throw new ArgumentException("El peso debe tener máximo 2 decimales");
    }

    private void ValidarTipo(string tipo)
    {
        var tiposPermitidos = new List<string> { "Mano", "Bodega", "Especial" };

        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("El tipo de equipaje no puede estar vacío");

        if (!tiposPermitidos.Contains(tipo))
            throw new ArgumentException($"Tipo inválido. Use: {string.Join(", ", tiposPermitidos)}");
    }

    private void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción no puede estar vacía");

        if (descripcion.Length < 5 || descripcion.Length > 100)
            throw new ArgumentException("La descripción debe tener entre 5 y 100 caracteres");

        if (Regex.IsMatch(descripcion, @"[^\w\sáéíóúÁÉÍÓÚñÑ\-.,]"))
            throw new ArgumentException("La descripción contiene caracteres inválidos");
    }

    private string GenerarCodigoEquipaje()
    {
        return $"EQ-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(100, 999)}";
    }

    public void ActualizarEstado(string nuevoEstado, string codigoEmpleado, string observaciones = null)
    {
        ValidarEstado(nuevoEstado);
        ValidarCodigoEmpleado(codigoEmpleado);

        Estado = nuevoEstado;
        CodigoEmpleado = codigoEmpleado;
        Observaciones = observaciones;
    }

    private void ValidarEstado(string estado)
    {
        var estadosPermitidos = new List<string> { "Registrado", "Embarcado", "Perdido", "Entregado" };

        if (!estadosPermitidos.Contains(estado))
            throw new ArgumentException($"Estado inválido. Use: {string.Join(", ", estadosPermitidos)}");
    }

    private void ValidarCodigoEmpleado(string codigoEmpleado)
    {
        if (string.IsNullOrWhiteSpace(codigoEmpleado))
            throw new ArgumentException("El código de empleado es requerido");

        if (!Regex.IsMatch(codigoEmpleado, @"^EMP-\d{4}$"))
            throw new ArgumentException("Formato de código de empleado inválido (Ej: EMP-1234)");
    }

    public void ActualizarPeso(decimal nuevoPeso, string codigoEmpleado)
    {
        ValidarPeso(nuevoPeso);
        ValidarCodigoEmpleado(codigoEmpleado);

        Peso = nuevoPeso;
        CodigoEmpleado = codigoEmpleado;
        Observaciones = $"Peso actualizado el {DateTime.Now:yyyy-MM-dd}";
    }

    public void Guardar()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Equipajes\Equipajes.json";

        List<Equipaje> listaEquipajes = CargarTodos();
        var existente = listaEquipajes.FirstOrDefault(e => e.CodigoEquipaje == this.CodigoEquipaje);

        if (existente != null)
            listaEquipajes.Remove(existente);

        listaEquipajes.Add(this);

        string json = JsonConvert.SerializeObject(listaEquipajes, Formatting.Indented);
        File.WriteAllText(ruta, json);
    }

    public static List<Equipaje> CargarTodos()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Equipajes\Equipajes.json";

        if (!File.Exists(ruta))
            return new List<Equipaje>();

        try
        {
            string json = File.ReadAllText(ruta);
            return JsonConvert.DeserializeObject<List<Equipaje>>(json) ?? new List<Equipaje>();
        }
        catch (JsonException)
        {
            File.Move(ruta, $"{ruta}.corrupto_{DateTime.Now:yyyyMMddHHmmss}");
            return new List<Equipaje>();
        }
    }

    public static Equipaje Cargar(string codigoEquipaje)
    {
        if (string.IsNullOrWhiteSpace(codigoEquipaje))
            throw new ArgumentException("El código de equipaje no puede estar vacío");

        var equipajes = CargarTodos();
        var equipaje = equipajes.FirstOrDefault(e => e.CodigoEquipaje == codigoEquipaje);

        if (equipaje == null)
            throw new FileNotFoundException($"No se encontró el equipaje con código {codigoEquipaje}");

        return equipaje;
    }
}