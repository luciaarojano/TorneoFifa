using System;
using System.Collections.Generic;
using System.Text;

namespace TorneoFifa.Enums
{
     /// <summary>
     /// Enumeración con los torneos que se pueden organizar
     /// (pizarra "Parte Funcional - Inscripciones": Navidad, Semana Santa, Fin de Curso).
     /// Se usa un enum en vez de un string para que no se pueda escribir cualquier cosa.
     /// </summary>
    public enum TipoTorneo : byte
    {
        Navidad,
        SemanaSanta,
        FinCurso
    }
}
