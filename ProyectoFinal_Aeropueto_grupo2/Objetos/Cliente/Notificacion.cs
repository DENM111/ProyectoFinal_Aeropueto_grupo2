using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Notificacion
{
    public string CodigoNotificacion { get; private set; }
    public string DNI_Pasajero { get; private set; }
    public string Titulo { get; private set; }
    public string Mensaje { get; private set; }
    public DateTime FechaEnvio { get; private set; }
    public bool Leida { get; private set; }
    public string Tipo { get; private set; }
    public string CodigoRelacionado { get; private set; }

    public Notificacion(string dni, string titulo, string mensaje, string tipo, string codigoRelacionado = null)
    {
        if (string.IsNullOrWhiteSpace(dni))
            throw new ArgumentException("El DNI no puede estar vacío");

        if (!Regex.IsMatch(dni, @"^\d{12}$"))
            throw new ArgumentException("DNI inválido. Debe tener 12 dígitos");

        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título no puede estar vacío");

        if (titulo.Length > 100)
            throw new ArgumentException("El título no puede exceder los 100 caracteres");

        if (string.IsNullOrWhiteSpace(mensaje))
            throw new ArgumentException("El mensaje no puede estar vacío");

        if (mensaje.Length > 500)
            throw new ArgumentException("El mensaje no puede exceder los 500 caracteres");

        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("El tipo de notificación no puede estar vacío");

        var tiposValidos = new[] { "Reserva", "Pago", "Vuelo", "CheckIn", "Sistema", "Promocion" };
        if (!tiposValidos.Contains(tipo))
            throw new ArgumentException($"Tipo de notificación no válido. Tipos aceptados: {string.Join(", ", tiposValidos)}");

        if (!string.IsNullOrEmpty(codigoRelacionado) && codigoRelacionado.Length > 50)
            throw new ArgumentException("El código relacionado no puede exceder los 50 caracteres");

        CodigoNotificacion = $"NOT-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4)}";
        DNI_Pasajero = dni;
        Titulo = titulo;
        Mensaje = mensaje;
        FechaEnvio = DateTime.Now;
        Leida = false;
        Tipo = tipo;
        CodigoRelacionado = codigoRelacionado;
    }

    public void MarcarComoLeida()
    {
        if (Leida)
            throw new InvalidOperationException("La notificación ya está marcada como leída");

        Leida = true;
    }


    public void Guardar()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Notificaciones\NOTIFICACIONES.JSON";

        List<Notificacion> notificaciones = CargarTodas();
        var existente = notificaciones.FirstOrDefault(n => n.CodigoNotificacion == this.CodigoNotificacion);

        if (existente != null)
            notificaciones.Remove(existente);

        notificaciones.Add(this);

        string json = JsonConvert.SerializeObject(notificaciones, Formatting.Indented);
        File.WriteAllText(ruta, json);
    }

    public static List<Notificacion> CargarTodas()
    {
        string ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Notificaciones\NOTIFICACIONES.JSON";

        if (!File.Exists(ruta))
            return new List<Notificacion>();

        try
        {
            string json = File.ReadAllText(ruta);
            return JsonConvert.DeserializeObject<List<Notificacion>>(json) ?? new List<Notificacion>();
        }
        catch (JsonException)
        {
            File.Move(ruta, $"{ruta}.corrupto_{DateTime.Now:yyyyMMddHHmmss}");
            return new List<Notificacion>();
        }
    }

    public static Notificacion Cargar(string codigoNotificacion)
    {
        if (string.IsNullOrWhiteSpace(codigoNotificacion))
            throw new ArgumentException("El código de notificación no puede estar vacío");

        var notificaciones = CargarTodas();
        var notificacion = notificaciones.FirstOrDefault(n => n.CodigoNotificacion == codigoNotificacion);

        if (notificacion == null)
            throw new FileNotFoundException($"No se encontró la notificación con código {codigoNotificacion}");

        return notificacion;
    }

    public static Notificacion[] BuscarPorDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni) || !Regex.IsMatch(dni, @"^\d{12}$"))
            throw new ArgumentException("DNI inválido. Debe tener 12 dígitos");

        var todas = CargarTodas();
        return todas.Where(n => n.DNI_Pasajero == dni).ToArray();
    }
}