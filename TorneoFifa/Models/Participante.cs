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
        // CONSTANTES
        public const string DOMINIO_EDUCAAND = "@g.educaand.es";   // Igual para todos los alumnos
        private const int LETRAS_USUARIO = 7;       // Usuario educaand: 7 letras...
        private const int NUMEROS_USUARIO = 3;      // ...seguidas de 3 números (ej. lrojbuj850)
        private const int MIN_LETRAS_NOMBRE = 2;    // Nombre y apellidos: mínimo 2 letras
        private const int MAX_LETRAS_NOMBRE = 40;   // y máximo 40 caracteres

        // MIEMBROS PRIVADOS
        private string _geducaand = "";     // Correo completo de educaand: usuario + dominio (UNIQUE en la BD)
        private string _nombre = "";
        private string _apellidos = "";
        private string _curso = "";

        // CONSTRUCTORES
        /// <summary>
        /// Crea un participante
        /// </summary>
        /// <param name="usuario">Usuario de educaand SIN el @g.educaand.es (ej. lrojbuj850)</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="apellidos">Apellidos</param>
        /// <param name="curso">Curso elegido en el desplegable</param>
        public Participante(string usuario, string nombre, string apellidos, string curso)
        {
            // Inicialización de los miembros
            // Se asignan a través de las propiedades para que se validen
            Geducaand = usuario;
            Nombre = nombre;
            Apellidos = apellidos;
            Curso = curso;
        }

        // PROPIEDADES
        /// <summary>
        /// Correo completo de educaand (ej. lrojbuj850@g.educaand.es).
        /// Al asignarlo se recibe solo el usuario: se le quitan los espacios,
        /// se pasa a minúsculas y se le añade el dominio.
        /// </summary>
        public string Geducaand
        {
            get { return _geducaand; }
            private set
            {
                ValidarUsuario(value);
                _geducaand = value.Replace(" ", "").ToLower() + DOMINIO_EDUCAAND;
            }
        }

        /// <summary>
        /// Usuario de educaand sin el dominio (para mostrarlo en la lista)
        /// </summary>
        public string Usuario
        {
            get { return _geducaand.Replace(DOMINIO_EDUCAAND, ""); }
        }

        public string Nombre
        {
            get { return _nombre; }
            private set
            {
                ValidarNombre(value, "el nombre");
                _nombre = PonerMayusculas(QuitarEspaciosSobrantes(value));
            }
        }

        public string Apellidos
        {
            get { return _apellidos; }
            private set
            {
                ValidarNombre(value, "los apellidos");
                _apellidos = PonerMayusculas(QuitarEspaciosSobrantes(value));
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

        /// <summary>
        /// Nombre y apellidos juntos (solo lectura)
        /// </summary>
        public string NombreCompleto
        {
            get { return $"{_nombre} {_apellidos}"; }
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

        /// <summary>
        /// Comprueba el usuario de educaand. Tiene que seguir el formato de educaand:
        /// 7 letras seguidas de 3 números (ej. lrojbuj850)
        /// </summary>
        /// <param name="usuario">Usuario escrito (sin el dominio)</param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidarUsuario(string usuario)
        {
            // Recursos
            string limpio;

            // 1.- Comprobación de que se ha escrito algo
            ValidarTexto(usuario, "el usuario de educaand");
            limpio = usuario.Replace(" ", "");

            // 2.- El dominio ya lo pone la app: no se debe escribir el @
            if (limpio.Contains("@"))
                throw new ArgumentException($"ERROR: Escriba solo el usuario, sin {DOMINIO_EDUCAAND}");

            // 3.- Comprobación de la longitud (7 + 3 = 10 caracteres)
            if (limpio.Length != LETRAS_USUARIO + NUMEROS_USUARIO)
                throw new ArgumentException($"ERROR: El usuario de educaand tiene {LETRAS_USUARIO} letras y {NUMEROS_USUARIO} números (ej. lrojbuj850)");

            // 4.- Comprobación carácter a carácter: primero letras y al final números
            for (int i = 0; i < limpio.Length; i++)
            {
                // 4.1.- Las 7 primeras posiciones tienen que ser letras
                if (i < LETRAS_USUARIO && !Char.IsLetter(limpio[i]))
                    throw new ArgumentException($"ERROR: El usuario de educaand empieza por {LETRAS_USUARIO} letras (ej. lrojbuj850)");

                // 4.2.- Las 3 últimas tienen que ser números
                if (i >= LETRAS_USUARIO && !Char.IsDigit(limpio[i]))
                    throw new ArgumentException($"ERROR: El usuario de educaand termina en {NUMEROS_USUARIO} números (ej. lrojbuj850)");
            }
        }

        /// <summary>
        /// Comprueba el nombre o los apellidos: solo letras y espacios,
        /// con un mínimo y un máximo de caracteres
        /// </summary>
        /// <param name="texto">Nombre o apellidos</param>
        /// <param name="campo">Nombre del campo para el mensaje de error</param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidarNombre(string texto, string campo)
        {
            // 1.- Comprobación de que se ha escrito algo
            ValidarTexto(texto, campo);

            // 2.- Solo letras (con tildes y ñ) y espacios: nada de números ni símbolos
            foreach (char caracter in texto)
            {
                if (!Char.IsLetter(caracter) && caracter != ' ')
                    throw new ArgumentException($"ERROR: En {campo} solo puede haber letras");
            }

            // 3.- Comprobación del mínimo de letras (sin contar los espacios)
            if (texto.Replace(" ", "").Length < MIN_LETRAS_NOMBRE)
                throw new ArgumentException($"ERROR: En {campo} debe haber al menos {MIN_LETRAS_NOMBRE} letras");

            // 4.- Comprobación del máximo de caracteres
            if (texto.Trim().Length > MAX_LETRAS_NOMBRE)
                throw new ArgumentException($"ERROR: En {campo} puede haber como máximo {MAX_LETRAS_NOMBRE} caracteres");
        }

        /// <summary>
        /// Quita los espacios del principio y del final, y deja un solo espacio entre palabras
        /// (ej. "  Lucia   Rojano " -> "Lucia Rojano")
        /// </summary>
        /// <param name="texto">Texto original</param>
        /// <returns>Texto sin espacios de sobra</returns>
        private string QuitarEspaciosSobrantes(string texto)
        {
            // Recursos
            string resultado = texto.Trim();

            // Mientras haya dos espacios seguidos, se cambian por uno
            while (resultado.Contains("  "))
                resultado = resultado.Replace("  ", " ");

            return resultado;
        }

        /// <summary>
        /// Pone la primera letra de cada palabra en mayúscula y el resto en minúscula
        /// (ej. "lucia rojano" -> "Lucia Rojano")
        /// </summary>
        /// <param name="texto">Texto sin espacios de sobra</param>
        /// <returns>Texto con mayúsculas</returns>
        private string PonerMayusculas(string texto)
        {
            // Recursos
            string[] palabras = texto.Split(' ');   // Se separa en palabras por los espacios
            string resultado = "";

            foreach (string palabra in palabras)
            {
                // Primera letra en mayúscula + resto en minúscula + espacio
                resultado += palabra.Substring(0, 1).ToUpper() + palabra.Substring(1).ToLower() + " ";
            }

            // Se quita el último espacio
            return resultado.Trim();
        }

        #endregion
    }
}