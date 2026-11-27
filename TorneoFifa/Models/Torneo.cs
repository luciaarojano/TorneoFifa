using System;
using System.Collections.Generic;
using System.Text;
using TorneoFifa.Enums;

namespace TorneoFifa.Models
{
    /// <summary>
    /// Clase del modelo de datos que representa un torneo
    /// (campos de la tabla TORNEOS de la pizarra).
    /// Las propiedades VALIDAN los datos: si un dato no es correcto lanzan una
    /// excepción y la página la captura para mostrar el error.
    /// </summary>
    public class Torneo
    {
        // CONSTANTES
        private const int LONGITUD_ANIO = 4;        // El año tiene 4 dígitos
        private const int MIN_PARTICIPANTES = 2;    // Como mínimo una final (2 jugadores)

        // MIEMBROS PRIVADOS
        private TipoTorneo _nombre;                 // Navidad, SemanaSanta o FinCurso
        private string _anio = "";                  // Año como String (pizarra)
        private int _numParticipantes;              // Debe ser potencia de 2 (2^n)
        private int _numRondas;                     // Es la n de 2^n
        private bool _activo;
        private List<Participante> _participantes;  // Relación Torneo 1 -> (2,n) Participante

        // CONSTRUCTORES
        public Torneo(TipoTorneo nombre, string anio, int numParticipantes)
        {
            // Inicialización de los miembros
            // Se asignan a través de las propiedades para que se validen
            Nombre = nombre;
            Anio = anio;
            NumParticipantes = numParticipantes;

            // Un torneo nuevo empieza activo y sin participantes inscritos
            _activo = true;
            _participantes = new List<Participante>();
        }

        // PROPIEDADES
        public TipoTorneo Nombre
        {
            get { return _nombre; }
            private set { _nombre = value; }
        }

        public string Anio
        {
            get { return _anio; }
            private set
            {
                // Validación antes de guardar el valor
                ValidarAnio(value);
                _anio = value;
            }
        }

        public int NumParticipantes
        {
            get { return _numParticipantes; }
            private set
            {
                // Al calcular las rondas se comprueba que es potencia de 2
                _numRondas = CalcularRondas(value);
                _numParticipantes = value;
            }
        }

        /// <summary>
        /// Número de rondas del torneo (solo lectura: se calcula a partir de los participantes)
        /// </summary>
        public int NumRondas
        {
            get { return _numRondas; }
        }

        public bool Activo
        {
            get { return _activo; }
            set { _activo = value; }
        }

        /// <summary>
        /// Lista de participantes inscritos (List&lt;Participante&gt; de la pizarra del modelo de datos)
        /// </summary>
        public List<Participante> Participantes
        {
            get { return _participantes; }
        }

        #region METODOS PRIVADOS

        /// <summary>
        /// Validación del año del torneo: 4 dígitos y año actual o posterior
        /// </summary>
        /// <param name="anio">Año introducido</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        private void ValidarAnio(string anio)
        {
            // 1.- Comprobación de si la cadena es nula o vacía
            if (string.IsNullOrEmpty(anio))
                throw new ArgumentException("ERROR: No ha introducido el año");

            // 2.- Comprobación de la longitud (4 caracteres)
            if (anio.Length != LONGITUD_ANIO)
                throw new FormatException($"ERROR: El año debe tener {LONGITUD_ANIO} dígitos");

            // 3.- Comprobación de que todos los caracteres son dígitos
            foreach (char caracter in anio)
            {
                if (!Char.IsDigit(caracter))
                    throw new FormatException("ERROR: El año solo puede contener dígitos");
            }

            // 4.- Comprobación de que es el año actual o posterior
            //     DateTime.Now.Year devuelve el año de hoy (por ejemplo 2026)
            if (int.Parse(anio) < DateTime.Now.Year)
                throw new ArgumentException($"ERROR: El año debe ser {DateTime.Now.Year} o posterior");
        }

        /// <summary>
        /// Comprueba que el número de participantes es potencia de 2 (2^n)
        /// y devuelve el número de rondas (n)
        /// </summary>
        /// <param name="numParticipantes">Número de participantes</param>
        /// <returns>Número de rondas</returns>
        /// <exception cref="ArgumentException"></exception>
        private int CalcularRondas(int numParticipantes)
        {
            // Recursos
            int rondas = 0;
            int jugadores = numParticipantes;   // Copia para ir dividiendo sin perder el original

            // 1.- Comprobación del mínimo de participantes
            if (numParticipantes < MIN_PARTICIPANTES)
                throw new ArgumentException($"ERROR: Debe haber al menos {MIN_PARTICIPANTES} participantes");

            // 2.- Se divide entre 2 hasta llegar a 1 (el ganador): cada división es una ronda
            //     Ej: 8 -> 4 -> 2 -> 1  => 3 rondas (2^3 = 8)
            while (jugadores > 1)
            {
                // 2.1.- Si en algún momento es impar, no es potencia de 2
                if (jugadores % 2 != 0)
                    throw new ArgumentException("ERROR: El número de participantes debe ser potencia de 2 (2, 4, 8, 16, 32...)");

                // 2.2.- Paso a la siguiente ronda
                jugadores = jugadores / 2;
                rondas++;
            }

            return rondas;
        }

        #endregion
    }
}
