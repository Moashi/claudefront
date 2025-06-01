namespace FrontFitLife.Pages.UserControl;

public partial class UserMenuPage : ContentPage
{
    public UserMenuPage()
    {
        InitializeComponent();
    }

    private void OnRutinaClicked(object sender, EventArgs e)
    {
        // Cambiar el contenido principal
        ActualizarContenido("Rutina", "Aquí puedes ver y gestionar tu rutina de ejercicios personalizada.");

        // Actualizar estado visual del menú
        ActualizarEstadoMenu("Rutina");
    }

    private void OnAsistenciaClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Asistencia", "Registra y revisa tu asistencia al gimnasio.");
        ActualizarEstadoMenu("Asistencia");
    }

    private void OnProgresoClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Mi Progreso", "Visualiza tu progreso y estadísticas de entrenamiento.");
        ActualizarEstadoMenu("Progreso");
    }

    private void OnPerfilClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Mi Perfil", "Gestiona tu información personal y configuración de cuenta.");
        ActualizarEstadoMenu("Perfil");
    }

    private void OnReportesClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Mis Reportes", "Consulta tus reportes personales de entrenamiento.");
        ActualizarEstadoMenu("Reportes");
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Cerrar Sesión",
            "¿Estás seguro de que quieres cerrar sesión?",
            "Sí", "No");

        if (confirmar)
        {
            // Navegar de vuelta al login
            await Shell.Current.GoToAsync("//login");
        }
    }

    // Cambia la llamada en ActualizarContenido:
    private async void ActualizarContenido(string titulo, string descripcion)
    {
        ContenidoPrincipal.Children.Clear();

        var tituloLabel = new Label
        {
            Text = titulo,
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1F2937")
        };

        var descripcionLabel = new Label
        {
            Text = descripcion,
            FontSize = 16,
            TextColor = Color.FromArgb("#6B7280"),
            Margin = new Thickness(0, 10, 0, 0)
        };

        var contenidoEspecifico = await CrearContenidoEspecifico(titulo);

        ContenidoPrincipal.Children.Add(tituloLabel);
        ContenidoPrincipal.Children.Add(descripcionLabel);
        ContenidoPrincipal.Children.Add(contenidoEspecifico);
    }

    private async Task<View> CrearContenidoEspecifico(string seccion)
    {
        var stackLayout = new StackLayout { Margin = new Thickness(0, 20, 0, 0) };

        switch (seccion)
        {
            case "Rutina":
                var rutinaFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 20,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var rutinaStack = new StackLayout();
                rutinaStack.Children.Add(new Label
                {
                    Text = "🏋️‍♀️ Rutina de Hoy: Pecho y Tríceps",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                var ejercicios = new string[]
                {
                    "• Press de banca: 3 series x 12 rep",
                    "• Press inclinado: 3 series x 10 rep",
                    "• Fondos: 3 series x 15 rep",
                    "• Extensiones de tríceps: 3 series x 12 rep"
                };

                foreach (var ejercicio in ejercicios)
                {
                    rutinaStack.Children.Add(new Label
                    {
                        Text = ejercicio,
                        FontSize = 14,
                        Margin = new Thickness(10, 2, 0, 2)
                    });
                }

                rutinaFrame.Content = rutinaStack;
                stackLayout.Children.Add(rutinaFrame);
                break;

            case "Asistencia":
                var asistenciaFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 20,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var asistenciaStack = new StackLayout();
                asistenciaStack.Children.Add(new Label
                {
                    Text = "📊 Asistencia este mes: 12 días",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                asistenciaStack.Children.Add(new Label
                {
                    Text = "• Última visita: Ayer 18:30",
                    FontSize = 14,
                    Margin = new Thickness(10, 2, 0, 2)
                });

                asistenciaStack.Children.Add(new Label
                {
                    Text = "• Promedio semanal: 3 días",
                    FontSize = 14,
                    Margin = new Thickness(10, 2, 0, 2)
                });

                asistenciaStack.Children.Add(new Label
                {
                    Text = "• Meta mensual: 16 días",
                    FontSize = 14,
                    Margin = new Thickness(10, 2, 0, 2)
                });

                asistenciaFrame.Content = asistenciaStack;
                stackLayout.Children.Add(asistenciaFrame);
                break;

            case "Mi Progreso":
                var progresoFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 20,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var progresoStack = new StackLayout();
                progresoStack.Children.Add(new Label
                {
                    Text = "📈 Progreso General",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                var progresos = new string[]
                {
                    "• Peso actual: 70 kg",
                    "• Meta: 75 kg",
                    "• Progreso: +2 kg este mes",
                    "• IMC: 22.5 (Normal)",
                    "• Grasa corporal: 15%"
                };

                foreach (var progreso in progresos)
                {
                    progresoStack.Children.Add(new Label
                    {
                        Text = progreso,
                        FontSize = 14,
                        Margin = new Thickness(10, 2, 0, 2)
                    });
                }

                progresoFrame.Content = progresoStack;
                stackLayout.Children.Add(progresoFrame);
                break;

            case "Mi Perfil":
                await Shell.Current.GoToAsync("profile");
                break;

            default:
                stackLayout.Children.Add(new Label
                {
                    Text = $"Contenido de {seccion} en desarrollo...",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Italic,
                    TextColor = Color.FromArgb("#9CA3AF")
                });
                break;
        }

        return stackLayout;
    }

    private void ActualizarEstadoMenu(string opcionSeleccionada)
    {
        // Limpiar estados anteriores
        RutinaOption.BackgroundColor = Colors.Transparent;
        AsistenciaOption.BackgroundColor = Colors.Transparent;
        ProgresoOption.BackgroundColor = Colors.Transparent;
        PerfilOption.BackgroundColor = Colors.Transparent;
        ReportesOption.BackgroundColor = Colors.Transparent;

        // Marcar la opción seleccionada
        var colorSeleccionado = Color.FromArgb("#5B21B6");

        switch (opcionSeleccionada)
        {
            case "Rutina":
                RutinaOption.BackgroundColor = colorSeleccionado;
                break;
            case "Asistencia":
                AsistenciaOption.BackgroundColor = colorSeleccionado;
                break;
            case "Progreso":
                ProgresoOption.BackgroundColor = colorSeleccionado;
                break;
            case "Perfil":
                PerfilOption.BackgroundColor = colorSeleccionado;
                break;
            case "Reportes":
                ReportesOption.BackgroundColor = colorSeleccionado;
                break;
        }
    }
}