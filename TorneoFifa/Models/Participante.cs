using System;
using System.Collections.Generic;
using System.Text;

namespace TorneoFifa.Models
{
    /// <summary>
    /// Clase del modelo de datos que representa a un participante del torneo
    /// (campos de la tabla PARTICIPANTES de la pizarra)
    /// </summary>
    public class Participante
    {
        // MIEMBROS PRIVADOS
        private string _geducaand = "";     // Usuario de educaand: identifica al alumno (UNIQUE en la BD)
        private string _nombre = "";
        private string _apellidos = "";
        private string _curso = "";

        // CONSTRUCTORES
        public Participante(string geducaand, string nombre, string apellidos, string curso)
        {
            // Inicialización de los miembros
            // Se asignan a través de las propiedades para que se validen
            Geducaand = geducaand;
            Nombre = nombre;
            Apellidos = apellidos;
            Curso = curso;
        }

        // PROPIEDADES
        public string Geducaand
        {
            get { return _geducaand; }
            private set
            {
                ValidarTexto(value, "el usuario de educaand");
                _geducaand = value.Trim();
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            private set
            {
                ValidarTexto(value, "el nombre");
                _nombre = value.Trim();
            }
        }

        public string Apellidos
        {
            get { return _apellidos; }
            private set
            {
                ValidarTexto(value, "los apellidos");
                _apellidos = value.Trim();
            }
        }

        public string Curso
        {
            get { return _curso; }
            private set
            {
                ValidarTexto(value, "el curso");
                _curso = value.Trim();
            }
        }

        #region METODOS PRIVADOS

        /// <summary>
        /// Comprueba que se ha introducido un texto
        /// </summary>
        /// <param name="texto">Texto a comprobar</param>
        /// <param name="campo">Nombre del campo para el mensaje de error</param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidarTexto(string texto, string campo)
        {
            // 1.- Comprobación de si la cadena es nula, vacía o solo tiene espacios
            if (string.IsNullOrEmpty(texto) || string.IsNullOrEmpty(texto.Trim()))
                throw new ArgumentException($"ERROR: No ha introducido {campo}");
        }

        #endregion
    }
}
