using Microsoft.Maui.Controls.Shapes;   // Para RoundRectangle (bordes redondeados creados desde código)
using TorneoFifa.Enums;                 // Para poder usar la enumeración TipoTorneo
using TorneoFifa.Models;                // Para poder usar Torneo y Participante
using TorneoFifa.Services;              // Para poder usar GestionTorneos

namespace TorneoFifa.Views;

public partial class InscripcionTorneo : ContentPage
{
    // CONSTANTES
    private const string COLOR_TEXTO_SECUNDARIO = "#ABA9C2";
    private const string COLOR_CAMPO = "#110F1B";
    private const string COLOR_BORDE = "#29273F";

    // MIEMBROS PRIVADOS
    private Torneo? _torneo;    // Torneo actual (el "?" indica que puede estar vacío si no hay ninguno)

    // Colores para los círculos con las iniciales de cada jugador
    private string[] _coloresAvatar = { "#6C5CE7", "#E8792B", "#0F9C8C", "#D9468F", "#2B3FC7" };

    // CONSTRUCTORES
    public InscripcionTorneo()
    {
        InitializeComponent();
    }

    #region MANEJADORES DE EVENTOS

    /// <summary>
    /// NUEVO: OnAppearing se ejecuta CADA VEZ que se muestra la página
    /// (por ejemplo al volver desde el menú). Así se carga el torneo
    /// que se acaba de registrar en la página de Registro.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarTorneoActual();
    }

    /// <summary>
    /// Ocurre al pulsar "Inscribir jugador". Crea el participante y lo inscribe
    /// en el torneo actual; si algo no es correcto, se muestra el error.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    /// <summary>
    /// Ocurre al pulsar "Inscribir jugador". Crea el participante y lo inscribe
    /// en el torneo actual; si algo no es correcto, se muestra el error.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InscribirClick(object sender, EventArgs e)
    {
        // Recursos
        Participante participante;
        string curso;
        bool hayError = false;
        string mensajeError = "";

        try
        {
            // 1.- Comprobación de que hay un torneo
            if (_torneo == null)
                throw new ArgumentException("ERROR: Primero debe registrar un torneo");

            // 2.- Comprobación de que se ha elegido un curso en el desplegable
            //     SelectedIndex vale -1 cuando no hay nada elegido
            if (pCurso.SelectedIndex == -1)
                throw new ArgumentException("ERROR: No ha elegido el curso");

            // 2.1.- Texto de la opción elegida
            curso = pCurso.Items[pCurso.SelectedIndex];

            // 3.- Creación del participante: la clase Participante valida los datos
            //     y añade @g.educaand.es al usuario
            participante = new Participante(eGeducaand.Text, eNombre.Text, eApellidos.Text, curso);

            // 4.- Inscripción: el torneo comprueba plazas, que esté activo y que no esté repetido
            _torneo.InscribirParticipante(participante);

            // 5.- Se vacían los campos y se actualiza la lista
            eGeducaand.Text = "";
            eNombre.Text = "";
            eApellidos.Text = "";
            pCurso.SelectedIndex = -1;      // El desplegable vuelve a "Elige el curso"
            ActualizarLista();
        }
        catch (Exception error)
        {
            // Detección del Error
            hayError = true;
            mensajeError = error.Message;
        }
        finally
        {
            // Retroalimentación: se muestra u oculta el aviso de error
            MostrarError(hayError, mensajeError);
        }
    }

    /// <summary>
    /// Ocurre al pulsar el botón ✕ de un jugador de la lista (control centralizado).
    /// El StyleId del botón guarda el usuario de educaand del jugador a quitar.
    /// </summary>
    /// <param name="sender">Botón ✕ pulsado</param>
    /// <param name="e"></param>
    private void EliminarClick(object sender, EventArgs e)
    {
        // Recursos
        Button boton = (Button)sender;     // Captura del botón que genera el evento
        bool hayError = false;
        string mensajeError = "";

        try
        {
            if (_torneo == null)
                throw new ArgumentException("ERROR: Primero debe registrar un torneo");

            // Se quita al jugador cuyo usuario está en el StyleId del botón
            _torneo.EliminarParticipante(boton.StyleId);
            ActualizarLista();
        }
        catch (Exception error)
        {
            hayError = true;
            mensajeError = error.Message;
        }
        finally
        {
            MostrarError(hayError, mensajeError);
        }
    }

    #endregion

    #region METODOS PRIVADOS

    /// <summary>
    /// Pide a GestionTorneos el torneo actual y muestra la página según haya torneo o no
    /// </summary>
    private void CargarTorneoActual()
    {
        try
        {
            // 1.- Se obtiene el torneo actual (lanza excepción si no hay ninguno)
            _torneo = GestionTorneos.ObtenerTorneoActual();

            // 2.- Hay torneo: se muestra el contenido
            brdSinTorneo.IsVisible = false;
            gridContenido.IsVisible = true;
            lNombreTorneo.Text = $"{NombreTipo(_torneo.Nombre)} {_torneo.Anio}";
            ActualizarLista();
        }
        catch (Exception)
        {
            // 3.- No hay torneo: se muestra el aviso y se oculta el contenido
            _torneo = null;
            brdSinTorneo.IsVisible = true;
            gridContenido.IsVisible = false;
        }
    }

    /// <summary>
    /// Vuelve a generar la lista de inscritos y actualiza las plazas
    /// </summary>
    private void ActualizarLista()
    {
        // Recursos
        int inscritos;
        int posicion = 0;

        if (_torneo == null)
            return;

        inscritos = _torneo.Participantes.Count;

        // 1.- Plazas: texto, contador y barra de progreso (de 0 a 1)
        lPlazas.Text = $"{inscritos} / {_torneo.NumParticipantes}";
        lContador.Text = $"{inscritos} / {_torneo.NumParticipantes}";
        lPlazasLibres.Text = $"{_torneo.PlazasLibres} plazas libres · {_torneo.NumRondas} rondas";
        pbPlazas.Progress = (double)inscritos / _torneo.NumParticipantes;

        // 2.- Si está completo, se desactiva el botón de inscribir
        if (_torneo.PlazasLibres == 0)
        {
            bInscribir.IsEnabled = false;
            bInscribir.Text = "Torneo completo";
        }
        else
        {
            bInscribir.IsEnabled = true;
            bInscribir.Text = "Inscribir jugador  →";
        }

        // 3.- Lista de jugadores
        slInscritos.Children.Clear();

        if (inscritos == 0)
        {
            slInscritos.Children.Add(new Label
            {
                Text = "Todavía no hay jugadores inscritos",
                TextColor = Color.FromArgb(COLOR_TEXTO_SECUNDARIO),
                FontSize = 13
            });
        }

        foreach (Participante participante in _torneo.Participantes)
        {
            slInscritos.Children.Add(CrearFilaJugador(participante, posicion));
            posicion++;
        }
    }

    /// <summary>
    /// Crea desde código la fila de un jugador: iniciales | nombre y curso | botón ✕
    /// </summary>
    /// <param name="participante">Jugador inscrito</param>
    /// <param name="posicion">Posición en la lista (para elegir el color)</param>
    /// <returns>Border con la fila</returns>
    private Border CrearFilaJugador(Participante participante, int posicion)
    {
        // Recursos
        Grid fila;
        Border avatar;
        VerticalStackLayout datos;
        Button bQuitar;
        string iniciales;
        string colorAvatar;

        // 1.- Iniciales (primera letra del nombre y del apellido) y color según la posición
        iniciales = participante.Nombre.Substring(0, 1).ToUpper() + participante.Apellidos.Substring(0, 1).ToUpper();
        colorAvatar = _coloresAvatar[posicion % _coloresAvatar.Length];

        // 2.- Círculo con las iniciales
        avatar = new Border
        {
            WidthRequest = 40,
            HeightRequest = 40,
            StrokeThickness = 0,
            BackgroundColor = Color.FromArgb(colorAvatar),
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            Content = new Label
            {
                Text = iniciales,
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

        // 3.- Nombre completo y debajo curso · usuario
        datos = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };
        datos.Children.Add(new Label
        {
            Text = $"{participante.Nombre} {participante.Apellidos}",
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            FontSize = 14
        });
        datos.Children.Add(new Label
        {
            Text = $"{participante.Curso} · {participante.Usuario}",
            TextColor = Color.FromArgb(COLOR_TEXTO_SECUNDARIO),
            FontSize = 12
        });

        // 4.- Botón ✕ para quitar al jugador. Su StyleId guarda el usuario de educaand
        bQuitar = new Button
        {
            Text = "✕",
            StyleId = participante.Geducaand,
            WidthRequest = 40,
            HeightRequest = 40,
            Padding = 0,
            CornerRadius = 20,
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb(COLOR_TEXTO_SECUNDARIO)
        };
        // Suscripción del evento (como en la Relación 2: b7.Clicked += DigitosClick)
        bQuitar.Clicked += EliminarClick;

        // 5.- Grid de la fila: 3 columnas (Auto | * | Auto)
        fila = new Grid { ColumnSpacing = 12 };
        fila.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        fila.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        fila.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        fila.Add(avatar, 0, 0);
        fila.Add(datos, 1, 0);
        fila.Add(bQuitar, 2, 0);

        // 6.- La fila va dentro de un Border redondeado
        return new Border
        {
            BackgroundColor = Color.FromArgb(COLOR_CAMPO),
            Stroke = new SolidColorBrush(Color.FromArgb(COLOR_BORDE)),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Padding = new Thickness(10, 8),
            Content = fila
        };
    }

    /// <summary>
    /// Muestra u oculta el aviso de error
    /// </summary>
    /// <param name="hayError">true si ha habido error</param>
    /// <param name="mensaje">Mensaje del error</param>
    private void MostrarError(bool hayError, string mensaje)
    {
        lError.Text = mensaje;
        brdError.IsVisible = hayError;
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

    #endregion
}