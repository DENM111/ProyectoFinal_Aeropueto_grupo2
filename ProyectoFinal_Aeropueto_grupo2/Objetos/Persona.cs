using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProyectoFinal_Aeropueto_grupo2.Objetos
{
    public abstract class Persona
    {
        public string Nombre { get; protected set; }
        public string Apellido { get; protected set; }
        public string Email { get; protected set; }
        public string Telefono { get; protected set; }

        protected Persona(string nombre, string apellido, string email, string telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre no puede estar vacío");
            if (nombre.Length > 50)
                throw new ArgumentException("El nombre no puede exceder los 50 caracteres");
            if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new ArgumentException("El nombre solo puede contener letras y espacios");

            if (string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El apellido no puede estar vacío");
            if (apellido.Length > 50)
                throw new ArgumentException("El apellido no puede exceder los 50 caracteres");
            if (!Regex.IsMatch(apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new ArgumentException("El apellido solo puede contener letras y espacios");

            if (!ValidarEmail(email))
                throw new ArgumentException("Email no válido");

            if (!ValidarTelefono(telefono))
                throw new ArgumentException("Teléfono no válido");

            Nombre = nombre.Trim();
            Apellido = apellido.Trim();
            Email = email.ToLower().Trim();
            Telefono = telefono.Trim();
        }

        private bool ValidarEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool ValidarTelefono(string telefono)
        {
            return Regex.IsMatch(telefono, @"^\+?[0-9\s]{9,15}$");
        }

        public virtual void ActualizarContacto(string nuevoEmail, string nuevoTelefono)
        {
            if (!ValidarEmail(nuevoEmail))
                throw new ArgumentException("Email no válido");
            if (!ValidarTelefono(nuevoTelefono))
                throw new ArgumentException("Teléfono no válido");

            Email = nuevoEmail.ToLower().Trim();
            Telefono = nuevoTelefono.Trim();
        }
    }
}
