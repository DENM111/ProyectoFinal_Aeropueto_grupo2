using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public class Servicio
{
    public string CodigoServicio { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public decimal Precio { get; private set; }
    public bool Disponible { get; private set; }
    public string Categoria { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaDesactivacion { get; private set; }

    public Servicio(string nombre, string descripcion, decimal precio, string categoria)
    {
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);
        ValidarPrecio(precio);
        ValidarCategoria(categoria);

        CodigoServicio = GenerarCodigoServicio();
        Nombre = nombre;
        Descripcion = descripcion;
        Precio = precio;
        Categoria = categoria;
        Disponible = true;
        FechaCreacion = DateTime.Now;
    }
    private void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del servicio no puede estar vacío");

        if (nombre.Length < 5 || nombre.Length > 50)
            throw new ArgumentException("El nombre debe tener entre 5 y 50 caracteres");

        if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-0-9]+$"))
            throw new ArgumentException("El nombre contiene caracteres inválidos");
    }

    private void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción no puede estar vacía");

        if (descripcion.Length < 10 || descripcion.Length > 200)
            throw new ArgumentException("La descripción debe tener entre 10 y 200 caracteres");
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

    private void ValidarCategoria(string categoria)
    {
        var categoriasPermitidas = new List<string> { "Comida", "Entretenimiento", "Confort", "Transporte", "Otros" };

        if (string.IsNullOrWhiteSpace(categoria))
            throw new ArgumentException("La categoría no puede estar vacía");

        if (!categoriasPermitidas.Contains(categoria))
            throw new ArgumentException($"Categoría inválida. Use: {string.Join(", ", categoriasPermitidas)}");
    }

    private string GenerarCodigoServicio()
    {
        return $"SRV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
    }
    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        ValidarPrecio(nuevoPrecio);
        Precio = nuevoPrecio;
    }

    public void CambiarDisponibilidad(bool disponible)
    {
        Disponible = disponible;
        FechaDesactivacion = disponible ? DateTime.MinValue : DateTime.Now;
    }

    public void ActualizarServicio(string nuevoNombre, string nuevaDescripcion, string nuevaCategoria)
    {
        ValidarNombre(nuevoNombre);
        ValidarDescripcion(nuevaDescripcion);
        ValidarCategoria(nuevaCategoria);

        Nombre = nuevoNombre;
        Descripcion = nuevaDescripcion;
        Categoria = nuevaCategoria;
    }
    public static class ServicioRepository
    {
        private static readonly string Ruta = @"C:\Users\50489\Desktop\PROYECTO\ProyectoFinal_Aeropueto_grupo2(Validaciones)\ProyectoFinal_Aeropueto_grupo2(JSON)\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\ProyectoFinal_Aeropueto_grupo2\bin\Debug\DATA\Clientes\Servicios\servicios.json";

        public static void GuardarServicios(List<Servicio> servicios)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Ruta));
            string backup = Ruta.Replace(".json", $"_backup_{DateTime.Now:yyyyMMddHHmmss}.json");

            if (File.Exists(Ruta))
                File.Copy(Ruta, backup);

            string json = JsonConvert.SerializeObject(servicios, Formatting.Indented);
            File.WriteAllText(Ruta, json);
        }

        public static List<Servicio> CargarServicios()
        {
            if (!File.Exists(Ruta))
                return new List<Servicio>();

            try
            {
                string json = File.ReadAllText(Ruta);
                return JsonConvert.DeserializeObject<List<Servicio>>(json);
            }
            catch (JsonException)
            {
                string corrupto = Ruta.Replace(".json", $"_corrupto_{DateTime.Now:yyyyMMddHHmmss}.json");
                File.Move(Ruta, corrupto);
                return new List<Servicio>();
            }
        }
    }
}