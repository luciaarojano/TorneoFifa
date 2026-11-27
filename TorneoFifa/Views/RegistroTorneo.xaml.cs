using Microsoft.Maui.Controls.Shapes;
using TorneoFifa.Enums;
using TorneoFifa.Models;

namespace TorneoFifa.Views;

public partial class RegistroTorneo : ContentPage
{
    // CONSTANTES
    // Colores que se cambian desde el código (los mismos que en Colors.xaml)
    private const string COLOR_BORDE = "#29273F";
    private const string COLOR_ERROR = "#F0475A";
    private const string COLOR_DORADO = "#F5C451";
    private const string COLOR_VERDE = "#34D399";
    private const string COLOR_VIOLETA = "#6C5CE7";
    private const string COLOR_TEXTO_SECUNDARIO = "#ABA9C2";

    // MIEMBROS PRIVADOS
    private TipoTorneo _tipo;      // Tipo de torneo elegido en las tarjetas
    private bool _hayTipo;         // false hasta que se pulsa una de las tarjetas

    // CONSTRUCTORES
    public RegistroTorneo()
    {
        InitializeComponent();

        // Inicialización de los miembros
        _hayTipo = false;

        // Vista previa inicial (todavía sin datos)
        ActualizarVistaPrevia();
    }

    #region MANEJADORES DE EVENTOS

    /// <summary>
    /// Ocurre al pulsar una de las 3 tarjetas de tipo de torneo (control centralizado).
    /// Se sabe qué tarjeta se ha pulsado por su StyleId.
    /// </summary>
    /// <param name="sender">Border (tarjeta) pulsado</param>
    /// <param name="e"></param>
    private void TipoTapped(object sender, TappedEventArgs e)
    {
        // Recursos
        Border tarjeta = (Border)sender;   // Captura de la tarjeta que genera el evento

        // 1.- Conversión de la tarjeta pulsada al valor del enum
        switch (tarjeta.StyleId)
        {
            case "Navidad":
                _tipo = TipoTorneo.Navidad;
                break;
            case "SemanaSanta":
                _tipo = TipoTorneo.SemanaSanta;
                break;
            case "FinCurso":
                _tipo = TipoTorneo.FinCurso;
                break;
        }
        _hayTipo = true;

        // 2.- Resaltar la tarjeta elegida y actualizar la vista previa
        MarcarTipo(tarjeta);
        ActualizarVistaPrevia();
    }

    /// <summary>
    /// Ocurre cada vez que cambia el texto del año o del número de participantes.
    /// Sirve para que la vista previa se actualice mientras se escribe.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DatosTextChanged(object sender, TextChangedEventArgs e)
    {
        ActualizarVistaPrevia();
    }

    /// <summary>
    /// Ocurre al pulsar "Registrar torneo". Crea un objeto Torneo con los datos;
    /// si algún dato no es correcto, se muestra el error en el aviso rojo.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RegistrarClick(object sender, EventArgs e)
    {
        // Recursos
        Torneo torneo;
        int numParticipantes;
        bool hayError = false;
        string mensajeError = "";

        // Se quitan los errores del intento anterior
        OcultarError();

        try
        {
            // 1.- Comprobación de que se ha elegido un torneo
            if (!_hayTipo)
                throw new ArgumentException("ERROR: Debe seleccionar el tipo de torneo");

            // 2.- Conversión del número de participantes de texto a entero
            numParticipantes = ConvertirNumero(eNumParticipantes.Text);

            // 3.- Creación del torneo: la clase Torneo valida el año y los participantes
            torneo = new Torneo(_tipo, eAnio.Text, numParticipantes);

            // 4.- Retroalimentación: se muestra la tarjeta de torneo registrado
            //     (más adelante aquí se llamará a la API para guardarlo en la BD)
            MostrarExito(torneo);
        }
        catch (Exception error)
        {
            // Detección del Error
            hayError = true;
            mensajeError = error.Message;
        }
        finally
        {
            if (hayError)
            {
                // Retroalimentación del error: aviso rojo
                lError.Text = mensajeError;
                brdError.IsVisible = true;

                // Se marca en rojo el campo del error (se mira qué palabra contiene el mensaje)
                if (mensajeError.Contains("año"))
                    brdAnio.Stroke = new SolidColorBrush(Color.FromArgb(COLOR_ERROR));
                if (mensajeError.Contains("participantes"))
                    brdNum.Stroke = new SolidColorBrush(Color.FromArgb(COLOR_ERROR));
            }
        }
    }

    /// <summary>
    /// Ocurre al pulsar "Inscribir participantes" (se hará en la siguiente parte del proyecto)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void InscribirClick(object sender, EventArgs e)
    {
        await DisplayAlert("Inscripción", "La inscripción de participantes se hará en la siguiente parte del proyecto.", "Aceptar");
    }

    /// <summary>
    /// Ocurre al pulsar "Registrar otro": deja el formulario vacío para empezar de nuevo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OtroTorneoClick(object sender, EventArgs e)
    {
        // 1.- Limpieza de los datos
        eAnio.Text = "";
        eNumParticipantes.Text = "";
        _hayTipo = false;
        QuitarSeleccion();
        OcultarError();

        // 2.- Se vuelve a mostrar el formulario
        brdFormulario.IsVisible = true;
        brdExito.IsVisible = false;

        // 3.- Estado "Borrador" en la vista previa y en la inscripción
        lEstado.Text = "Borrador";
        lEstado.TextColor = Color.FromArgb(COLOR_DORADO);
        lEstadoInscripcion.Text = "Se abre al registrar el torneo";

        ActualizarVistaPrevia();
    }

    #endregion

    #region METODOS PRIVADOS

    /// <summary>
    /// Actualiza la tarjeta de vista previa (nombre, marcador) y las rondas del cuadro
    /// </summary>
    private void ActualizarVistaPrevia()
    {
        // Recursos
        Torneo prueba;
        int numParticipantes;

        // 1.- Nombre del torneo
        if (_hayTipo)
            lNombreTorneo.Text = $"{NombreTipo(_tipo)} {eAnio.Text}";
        else
            lNombreTorneo.Text = "Elige un torneo";

        // 2.- Participantes y rondas
        try
        {
            numParticipantes = ConvertirNumero(eNumParticipantes.Text);

            // Torneo de prueba con el año actual SOLO para que la clase Torneo
            // calcule las rondas (así no se repite aquí la lógica de la potencia de 2)
            prueba = new Torneo(TipoTorneo.Navidad, DateTime.Now.Year.ToString(), numParticipantes);

            lMarcador.Text = $"{prueba.NumParticipantes} · {prueba.NumRondas}";
            lInscritos.Text = $"0 / {prueba.NumParticipantes} inscritos";
            MostrarRondas(prueba.NumParticipantes);
        }
        catch (Exception)
        {
            // Mientras el número no sea válido, la vista previa se queda vacía
            lMarcador.Text = "– · –";
            lInscritos.Text = "0 / – inscritos";
            slRondas.Children.Clear();
            slRondas.Children.Add(new Label
            {
                Text = "Introduce un número de participantes válido (2, 4, 8, 16…)",
                TextColor = Color.FromArgb(COLOR_TEXTO_SECUNDARIO),
                FontSize = 13
            });
        }
    }

    /// <summary>
    /// Genera una fila por cada ronda del cuadro (igual que la lista de nombres de la Relación 10)
    /// </summary>
    /// <param name="numParticipantes">Número de participantes (potencia de 2)</param>
    private void MostrarRondas(int numParticipantes)
    {
        // Recursos
        int jugadores = numParticipantes;   // Jugadores que quedan en cada ronda
        int numRonda = 1;

        // 1.- Se vacía la lista anterior
        slRondas.Children.Clear();

        // 2.- En cada ronda se juegan jugadores/2 partidos y pasa la mitad
        //     Ej: 8 -> Cuartos (4 partidos) -> Semifinal (2) -> Final (1)
        while (jugadores > 1)
        {
            slRondas.Children.Add(CrearFilaRonda(numRonda, NombreRonda(jugadores), jugadores / 2));
            jugadores = jugadores / 2;
            numRonda++;
        }
    }

    /// <summary>
    /// Crea desde código la fila de una ronda: número | nombre | partidos
    /// </summary>
    /// <param name="numero">Número de ronda</param>
    /// <param name="nombre">Nombre de la ronda (Cuartos de final, Semifinal...)</param>
    /// <param name="partidos">Partidos que se juegan en la ronda</param>
    /// <returns>Grid con la fila</returns>
    private Grid CrearFilaRonda(int numero, string nombre, int partidos)
    {
        // Recursos
        Grid fila;
        Border insignia;
        Label lNombre;
        Label lPartidos;
        string colorInsignia = "#262438";
        string textoPartidos = $"{partidos} partidos";

        // 1.- La final se resalta en violeta y en singular
        if (partidos == 1)
        {
            colorInsignia = COLOR_VIOLETA;
            textoPartidos = "1 partido";
        }

        // 2.- Instanciación del Grid de la fila: 3 columnas (Auto | * | Auto)
        fila = new Grid
        {
            ColumnSpacing = 12,
            Padding = new Thickness(0, 8)
        };
        fila.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        fila.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        fila.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // 3.- Insignia con el número de la ronda
        insignia = new Border
        {
            WidthRequest = 28,
            HeightRequest = 28,
            StrokeThickness = 0,
            BackgroundColor = Color.FromArgb(colorInsignia),
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Content = new Label
            {
                Text = numero.ToString(),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

        // 4.- Nombre de la ronda y número de partidos
        lNombre = new Label
        {
            Text = nombre,
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            VerticalOptions = LayoutOptions.Center
        };
        lPartidos = new Label
        {
            Text = textoPartidos,
            TextColor = Color.FromArgb(COLOR_TEXTO_SECUNDARIO),
            FontSize = 13,
            VerticalOptions = LayoutOptions.Center
        };

        // 5.- Adición de los componentes al Grid (columna, fila)
        fila.Add(insignia, 0, 0);
        fila.Add(lNombre, 1, 0);
        fila.Add(lPartidos, 2, 0);

        return fila;
    }

    /// <summary>
    /// Devuelve el nombre de la ronda según los jugadores que quedan
    /// </summary>
    /// <param name="jugadores">Jugadores al empezar la ronda</param>
    /// <returns>Nombre de la ronda</returns>
    private string NombreRonda(int jugadores)
    {
        switch (jugadores)
        {
            case 2:
                return "Final";
            case 4:
                return "Semifinal";
            case 8:
                return "Cuartos de final";
            case 16:
                return "Octavos de final";
            default:
                return $"Ronda de {jugadores}";
        }
    }

    /// <summary>
    /// Devuelve el nombre del tipo de torneo para mostrarlo (con espacios)
    /// </summary>
    /// <param name="tipo">Valor del enum</param>
    /// <returns>Texto para la interfaz</returns>
    private string NombreTipo(TipoTorneo tipo)
    {
        switch (tipo)
        {
            case TipoTorneo.SemanaSanta:
                return "Semana Santa";
            case TipoTorneo.FinCurso:
                return "Fin de Curso";
            default:
                return tipo.ToString();
        }
    }

    /// <summary>
    /// Resalta la tarjeta elegida (borde blanco) y atenúa las otras
    /// </summary>
    /// <param name="seleccionada">Tarjeta pulsada</param>
    private void MarcarTipo(Border seleccionada)
    {
        // Recursos
        Border[] tarjetas = { brdNavidad, brdSemanaSanta, brdFinCurso };

        foreach (Border tarjeta in tarjetas)
        {
            if (tarjeta == seleccionada)
            {
                tarjeta.Opacity = 1;
                tarjeta.Stroke = new SolidColorBrush(Colors.White);
                tarjeta.StrokeThickness = 3;
            }
            else
            {
                tarjeta.Opacity = 0.5;
                tarjeta.StrokeThickness = 0;
            }
        }
    }

    /// <summary>
    /// Deja las 3 tarjetas como al principio (ninguna elegida)
    /// </summary>
    private void QuitarSeleccion()
    {
        // Recursos
        Border[] tarjetas = { brdNavidad, brdSemanaSanta, brdFinCurso };

        foreach (Border tarjeta in tarjetas)
        {
            tarjeta.Opacity = 1;
            tarjeta.StrokeThickness = 0;
        }
    }

    /// <summary>
    /// Oculta el aviso de error y devuelve los campos a su color normal
    /// </summary>
    private void OcultarError()
    {
        lError.Text = "";
        brdError.IsVisible = false;
        brdAnio.Stroke = new SolidColorBrush(Color.FromArgb(COLOR_BORDE));
        brdNum.Stroke = new SolidColorBrush(Color.FromArgb(COLOR_BORDE));
    }

    /// <summary>
    /// Muestra la tarjeta de torneo registrado con los datos del torneo
    /// </summary>
    /// <param name="torneo">Torneo recién creado</param>
    private void MostrarExito(Torneo torneo)
    {
        // 1.- Se cambia el formulario por la tarjeta de éxito
        brdFormulario.IsVisible = false;
        brdExito.IsVisible = true;

        // 2.- Datos del torneo
        lTituloExito.Text = $"¡{NombreTipo(torneo.Nombre)} {torneo.Anio} está listo!";
        lTextoExito.Text = $"El siguiente paso es inscribir a los {torneo.NumParticipantes} participantes del cuadro.";
        lExitoParticipantes.Text = torneo.NumParticipantes.ToString();
        lExitoRondas.Text = torneo.NumRondas.ToString();

        // 3.- Estado "Activo" en la vista previa y en la inscripción
        lEstado.Text = "Activo";
        lEstado.TextColor = Color.FromArgb(COLOR_VERDE);
        lEstadoInscripcion.Text = "Inscripciones abiertas";
    }

    /// <summary>
    /// Convierte el texto del número de participantes en un entero,
    /// comprobando antes que solo contiene dígitos
    /// </summary>
    /// <param name="texto">Texto introducido</param>
    /// <returns>Número entero</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="OverflowException"></exception>
    private int ConvertirNumero(string texto)
    {
        // 1.- Comprobación de si la cadena es nula o vacía
        if (string.IsNullOrEmpty(texto))
            throw new ArgumentException("ERROR: No ha introducido el número de participantes");

        // 2.- Comprobación de que todos los caracteres son dígitos
        foreach (char caracter in texto)
        {
            if (!Char.IsDigit(caracter))
                throw new FormatException("ERROR: El número de participantes solo puede contener dígitos");
        }

        // 3.- Comprobación de la longitud (máximo 3 cifras: hasta 999 participantes)
        if (texto.Length > 3)
            throw new OverflowException("ERROR: El número de participantes es demasiado grande");

        // 4.- Conversión a entero
        return int.Parse(texto);
    }

    #endregion
}