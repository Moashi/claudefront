namespace FrontFitLife.Pages.UserControl;

public partial class UserMenuPage : ContentPage
{
    private string _opcionActualSeleccionada = "";

    public UserMenuPage()
    {
        InitializeComponent();
        // Configurar efectos hover para los elementos del menú
        ConfigurarEfectosMenu();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Mostrar pantalla de bienvenida al cargar
        MostrarPantallaBienvenida();
    }

    private void ConfigurarEfectosMenu()
    {
        // Configurar gestos para efectos hover en cada opción del menú
        ConfigurarHoverEffect(RutinaOption);
        ConfigurarHoverEffect(AsistenciaOption);
        ConfigurarHoverEffect(ProgresoOption);
        ConfigurarHoverEffect(PerfilOption);
        ConfigurarHoverEffect(ReportesOption);
    }

    private void ConfigurarHoverEffect(Grid menuOption)
    {
        // Simular efectos hover con gestos de entrada y salida del puntero
        var pointerGesture = new PointerGestureRecognizer();

        pointerGesture.PointerEntered += (s, e) =>
        {
            if (menuOption.BackgroundColor == Colors.Transparent)
            {
                menuOption.BackgroundColor = Color.FromArgb("#4157E8"); // Hover effect
            }
        };

        pointerGesture.PointerExited += (s, e) =>
        {
            if (menuOption.BackgroundColor != Color.FromArgb("#4F63FF")) // Si no está seleccionado
            {
                menuOption.BackgroundColor = Colors.Transparent;
            }
        };

        menuOption.GestureRecognizers.Add(pointerGesture);
    }

    private void OnRutinaClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Mi Rutina", "Rutina", "Tu plan de entrenamiento personalizado");
        ActualizarEstadoMenu("Rutina");
    }

    private void OnAsistenciaClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Asistencia", "Asistencia", "Registra y revisa tu asistencia al gimnasio");
        ActualizarEstadoMenu("Asistencia");
    }

    private void OnProgresoClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Mi Progreso", "Progreso", "Visualiza tu evolución y estadísticas de entrenamiento");
        ActualizarEstadoMenu("Progreso");
    }

    private void OnPerfilClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Mi Perfil", "Perfil", "Gestiona tu información personal y configuración");
        ActualizarEstadoMenu("Perfil");
    }

    private void OnReportesClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Mis Reportes", "Reportes", "Consulta tus reportes personales de entrenamiento");
        ActualizarEstadoMenu("Reportes");
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        // Mostrar confirmación con diseño mejorado
        bool confirmar = await DisplayAlert(
            "Cerrar Sesión",
            "¿Estás seguro de que deseas cerrar sesión?",
            "Sí, cerrar sesión",
            "Cancelar");

        if (confirmar)
        {
            // Limpiar datos de usuario
            try
            {
                SecureStorage.RemoveAll();
                Preferences.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error limpiando datos: {ex.Message}");
            }

            // Navegar al login
            await Shell.Current.GoToAsync("//login");
        }
    }

    private void MostrarPantallaBienvenida()
    {
        ContenidoPrincipal.Children.Clear();
        _opcionActualSeleccionada = "";
        ActualizarEstadoMenu(""); // Limpiar selección

        // Header del contenido
        var headerStack = new StackLayout
        {
            Spacing = 10,
            Margin = new Thickness(0, 0, 0, 30)
        };

        headerStack.Children.Add(new Label
        {
            Text = "Bienvenido a FitLife",
            FontSize = 32,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1F2937")
        });

        headerStack.Children.Add(new Label
        {
            Text = "Selecciona una opción del menú para comenzar tu entrenamiento",
            FontSize = 18,
            TextColor = Color.FromArgb("#6B7280"),
            LineHeight = 1.5
        });

        ContenidoPrincipal.Children.Add(headerStack);

        // Cards de bienvenida
        var cardsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnSpacing = 20,
            RowSpacing = 20
        };

        // Card 1: Rutina
        var rutinaCard = CrearCardBienvenida(
            "&#xe82a;", "#3352FF", "Tu Rutina",
            "Accede a tu plan de entrenamiento personalizado");
        Grid.SetColumn(rutinaCard, 0);
        Grid.SetRow(rutinaCard, 0);

        // Card 2: Progreso  
        var progresoCard = CrearCardBienvenida(
            "&#xe8e5;", "#10B981", "Tu Progreso",
            "Visualiza tu evolución y estadísticas");
        Grid.SetColumn(progresoCard, 1);
        Grid.SetRow(progresoCard, 0);

        // Card 3: Asistencia
        var asistenciaCard = CrearCardBienvenida(
            "&#xe935;", "#F59E0B", "Asistencia",
            "Registra tus visitas al gimnasio");
        Grid.SetColumn(asistenciaCard, 0);
        Grid.SetRow(asistenciaCard, 1);

        // Card 4: Perfil
        var perfilCard = CrearCardBienvenida(
            "&#xe7fd;", "#8B5CF6", "Mi Perfil",
            "Gestiona tu información personal");
        Grid.SetColumn(perfilCard, 1);
        Grid.SetRow(perfilCard, 1);

        cardsGrid.Children.Add(rutinaCard);
        cardsGrid.Children.Add(progresoCard);
        cardsGrid.Children.Add(asistenciaCard);
        cardsGrid.Children.Add(perfilCard);

        ContenidoPrincipal.Children.Add(cardsGrid);
    }

    private Frame CrearCardBienvenida(string icono, string colorIcono, string titulo, string descripcion)
    {
        var frame = new Frame
        {
            BackgroundColor = Colors.White,
            CornerRadius = 15,
            HasShadow = true,
            Padding = 25
        };

        var stack = new StackLayout { Spacing = 12 };

        stack.Children.Add(new Label
        {
            Text = icono,
            FontFamily = "MaterialSymbols",
            FontSize = 28,
            TextColor = Color.FromArgb(colorIcono),
            HorizontalOptions = LayoutOptions.Start
        });

        stack.Children.Add(new Label
        {
            Text = titulo,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1F2937")
        });

        stack.Children.Add(new Label
        {
            Text = descripcion,
            FontSize = 14,
            TextColor = Color.FromArgb("#6B7280"),
            LineHeight = 1.4
        });

        frame.Content = stack;
        return frame;
    }

    private async void ActualizarContenido(string titulo, string seccion, string descripcion)
    {
        ContenidoPrincipal.Children.Clear();
        _opcionActualSeleccionada = seccion;

        // Header de la sección
        var headerStack = new StackLayout { Spacing = 10, Margin = new Thickness(0, 0, 0, 25) };

        headerStack.Children.Add(new Label
        {
            Text = titulo,
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1F2937")
        });

        headerStack.Children.Add(new Label
        {
            Text = descripcion,
            FontSize = 16,
            TextColor = Color.FromArgb("#6B7280"),
            Margin = new Thickness(0, 5, 0, 0)
        });

        ContenidoPrincipal.Children.Add(headerStack);

        // Contenido específico de la sección
        var contenidoEspecifico = await CrearContenidoEspecifico(seccion);
        ContenidoPrincipal.Children.Add(contenidoEspecifico);
    }

    private async Task<View> CrearContenidoEspecifico(string seccion)
    {
        var stackLayout = new StackLayout { Margin = new Thickness(0, 10, 0, 0) };

        switch (seccion)
        {
            case "Rutina":
                var rutinaFrame = CrearFrameContenido();
                var rutinaStack = new StackLayout { Spacing = 15 };

                rutinaStack.Children.Add(CrearTituloSeccion("🏋️‍♀️ Rutina de Hoy: Pecho y Tríceps"));

                var ejercicios = new string[]
                {
                    "Press de banca: 3 series x 12 repeticiones",
                    "Press inclinado: 3 series x 10 repeticiones",
                    "Fondos en paralelas: 3 series x 15 repeticiones",
                    "Extensiones de tríceps: 3 series x 12 repeticiones"
                };

                foreach (var ejercicio in ejercicios)
                {
                    rutinaStack.Children.Add(CrearItemLista(ejercicio));
                }

                rutinaFrame.Content = rutinaStack;
                stackLayout.Children.Add(rutinaFrame);
                break;

            case "Asistencia":
                var asistenciaFrame = CrearFrameContenido();
                var asistenciaStack = new StackLayout { Spacing = 15 };

                asistenciaStack.Children.Add(CrearTituloSeccion("📊 Resumen de Asistencia"));

                var statsAsistencia = new (string label, string valor, string color)[]
                {
                    ("Asistencia este mes", "12 días", "#10B981"),
                    ("Última visita", "Ayer a las 18:30", "#6B7280"),
                    ("Promedio semanal", "3 días", "#3352FF"),
                    ("Meta mensual", "16 días", "#F59E0B")
                };

                foreach (var stat in statsAsistencia)
                {
                    asistenciaStack.Children.Add(CrearEstadistica(stat.label, stat.valor, stat.color));
                }

                asistenciaFrame.Content = asistenciaStack;
                stackLayout.Children.Add(asistenciaFrame);
                break;

            case "Progreso":
                var progresoFrame = CrearFrameContenido();
                var progresoStack = new StackLayout { Spacing = 15 };

                progresoStack.Children.Add(CrearTituloSeccion("📈 Tu Progreso General"));

                var statsProgreso = new (string label, string valor, string color)[]
                {
                    ("Peso actual", "70 kg", "#10B981"),
                    ("Meta de peso", "75 kg", "#3352FF"),
                    ("Progreso este mes", "+2 kg", "#10B981"),
                    ("Índice de Masa Corporal", "22.5 (Normal)", "#6B7280"),
                    ("Porcentaje de grasa", "15%", "#F59E0B")
                };

                foreach (var stat in statsProgreso)
                {
                    progresoStack.Children.Add(CrearEstadistica(stat.label, stat.valor, stat.color));
                }

                progresoFrame.Content = progresoStack;
                stackLayout.Children.Add(progresoFrame);
                break;

            case "Perfil":
                // Aquí integraremos el perfil más adelante
                stackLayout.Children.Add(new Label
                {
                    Text = "Contenido del perfil se integrará aquí...",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Italic,
                    TextColor = Color.FromArgb("#6B7280"),
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                });
                break;

            case "Reportes":
                var reportesFrame = CrearFrameContenido();
                var reportesStack = new StackLayout { Spacing = 15 };

                reportesStack.Children.Add(CrearTituloSeccion("📊 Tus Reportes"));

                reportesStack.Children.Add(new Label
                {
                    Text = "Los reportes personalizados estarán disponibles próximamente.",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#6B7280"),
                    Margin = new Thickness(0, 10, 0, 0)
                });

                reportesFrame.Content = reportesStack;
                stackLayout.Children.Add(reportesFrame);
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

    private Frame CrearFrameContenido()
    {
        return new Frame
        {
            BackgroundColor = Colors.White,
            CornerRadius = 15,
            HasShadow = true,
            Padding = 25,
            Margin = new Thickness(0, 10, 0, 0)
        };
    }

    private Label CrearTituloSeccion(string texto)
    {
        return new Label
        {
            Text = texto,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1F2937")
        };
    }

    private Label CrearItemLista(string texto)
    {
        return new Label
        {
            Text = $"• {texto}",
            FontSize = 15,
            TextColor = Color.FromArgb("#374151"),
            Margin = new Thickness(15, 3, 0, 3)
        };
    }

    private StackLayout CrearEstadistica(string label, string valor, string color)
    {
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 10,
            Margin = new Thickness(10, 5, 0, 5)
        };

        stack.Children.Add(new Label
        {
            Text = label + ":",
            FontSize = 14,
            TextColor = Color.FromArgb("#374151"),
            VerticalOptions = LayoutOptions.Center
        });

        stack.Children.Add(new Label
        {
            Text = valor,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb(color),
            VerticalOptions = LayoutOptions.Center
        });

        return stack;
    }

    private void ActualizarEstadoMenu(string opcionSeleccionada)
    {
        // Limpiar estados anteriores
        var opciones = new[] { RutinaOption, AsistenciaOption, ProgresoOption, PerfilOption, ReportesOption };

        foreach (var opcion in opciones)
        {
            opcion.BackgroundColor = Colors.Transparent;
        }

        // Marcar la opción seleccionada
        var colorSeleccionado = Color.FromArgb("#4F63FF");

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