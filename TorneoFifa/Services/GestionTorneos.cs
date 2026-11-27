using System;
using System.Collections.Generic;
using System.Text;
using TorneoFifa.Models;    // Para poder usar la clase Torneo

namespace TorneoFifa.Services
{
    /// <summary>
    /// Clase estática que guarda los torneos registrados mientras la app está abierta
    /// y cuál es el TORNEO ACTUAL (el último registrado).
    /// Así todas las páginas (Registro, Inscripción...) trabajan con el mismo torneo.
    /// Es "static" para que haya una sola lista para toda la app.
    /// Más adelante, estos métodos guardarán los datos en la BD (API-Gestión BD).
    /// </summary>
    public static class GestionTorneos
    {
        // MIEMBROS PRIVADOS
        private static List<Torneo> _torneos = new List<Torneo>();    // Torneos registrados
        private static Torneo? _torneoActual = null;                   // El "?" indica que puede estar vacío

        // PROPIEDADES
        /// <summary>
        /// Lista de todos los torneos registrados
        /// </summary>
        public static List<Torneo> Torneos
        {
            get { return _torneos; }
        }

        #region METODOS PUBLICOS

        /// <summary>
        /// Registra un torneo nuevo y lo deja como torneo actual
        /// (la pizarra lo llama RegistrarTorneo(Torneo obj))
        /// </summary>
        /// <param name="torneo">Torneo ya creado y validado</param>
        /// <exception cref="ArgumentException"></exception>
        public static void RegistrarTorneo(Torneo torneo)
        {
            // 1.- Comprobación de que no existe ya un torneo con el mismo nombre y año
            foreach (Torneo registrado in _torneos)
            {
                if (registrado.Nombre == torneo.Nombre && registrado.Anio == torneo.Anio)
                    throw new ArgumentException($"ERROR: Ya existe el torneo {torneo.Nombre} {torneo.Anio}");
            }

            // 2.- Se añade a la lista y pasa a ser el torneo actual
            //     (más adelante: guardar también en la BD)
            _torneos.Add(torneo);
            _torneoActual = torneo;
        }

        /// <summary>
        /// Devuelve el torneo actual (el último registrado)
        /// </summary>
        /// <returns>Torneo actual</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Torneo ObtenerTorneoActual()
        {
            // 1.- Comprobación de que hay algún torneo registrado
            if (_torneoActual == null)
                throw new ArgumentException("ERROR: Primero debe registrar un torneo");

            return _torneoActual;
        }

        #endregion
    }
}