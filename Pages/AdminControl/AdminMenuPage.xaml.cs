namespace FrontFitLife.Pages.AdminControl;

public partial class AdminMenuPage : ContentPage
{
    public AdminMenuPage()
    {
        InitializeComponent();
    }

    private void OnAdminUsuariosClicked(object sender, EventArgs e)
    {
        // Cambiar el contenido principal
        ActualizarContenido("Administrar Usuarios", "Gestiona todos los usuarios registrados en el sistema.");

        // Actualizar estado visual del menú
        ActualizarEstadoMenu("AdminUsuarios");
    }

    private void OnReportesClicked(object sender, EventArgs e)
    {
        ActualizarContenido("Reportes", "Consulta reportes generales del sistema y estadísticas.");
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

    private void ActualizarContenido(string titulo, string descripcion)
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

        // Agregar contenido específico según la sección
        var contenidoEspecifico = CrearContenidoEspecifico(titulo);

        ContenidoPrincipal.Children.Add(tituloLabel);
        ContenidoPrincipal.Children.Add(descripcionLabel);
        ContenidoPrincipal.Children.Add(contenidoEspecifico);
    }

    private View CrearContenidoEspecifico(string seccion)
    {
        var stackLayout = new StackLayout { Margin = new Thickness(0, 20, 0, 0) };

        switch (seccion)
        {
            case "Administrar Usuarios":
                // Botones de acción
                var accionesFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 15,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var accionesStack = new StackLayout();
                accionesStack.Children.Add(new Label
                {
                    Text = "⚡ Acciones Rápidas",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                var buttonsGrid = new Grid();
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                buttonsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var btnAgregarUsuario = new Button
                {
                    Text = "➕ Agregar Usuario",
                    BackgroundColor = Color.FromArgb("#10B981"),
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                Grid.SetColumn(btnAgregarUsuario, 0);

                var btnExportarDatos = new Button
                {
                    Text = "📥 Exportar Datos",
                    BackgroundColor = Color.FromArgb("#3B82F6"),
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                Grid.SetColumn(btnExportarDatos, 1);

                buttonsGrid.Children.Add(btnAgregarUsuario);
                buttonsGrid.Children.Add(btnExportarDatos);
                accionesStack.Children.Add(buttonsGrid);

                accionesFrame.Content = accionesStack;
                stackLayout.Children.Add(accionesFrame);

                // Lista de usuarios
                var usuariosFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 15,
                    Margin = new Thickness(0, 15, 0, 0)
                };

                var usuariosStack = new StackLayout();
                usuariosStack.Children.Add(new Label
                {
                    Text = "👥 Usuarios Registrados (24)",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                var usuarios = new (string nombre, string estado, string fecha)[]
                {
                    ("👤 Juan Pérez", "✅ Activo", "Ult. acceso: Hoy"),
                    ("👤 María García", "✅ Activo", "Ult. acceso: Ayer"),
                    ("👤 Carlos López", "❌ Inactivo", "Ult. acceso: 5 días"),
                    ("👤 Ana Martínez", "✅ Activo", "Ult. acceso: Hoy"),
                    ("👤 Luis Rodríguez", "⚠️ Pendiente", "Registro: Hoy")
                };

                foreach (var usuario in usuarios)
                {
                    var usuarioGrid = new Grid();
                    usuarioGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                    usuarioGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    usuarioGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    usuarioGrid.Margin = new Thickness(0, 5, 0, 5);

                    var nombreLabel = new Label
                    {
                        Text = usuario.nombre,
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetColumn(nombreLabel, 0);

                    var estadoLabel = new Label
                    {
                        Text = usuario.estado,
                        FontSize = 12,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetColumn(estadoLabel, 1);

                    var fechaLabel = new Label
                    {
                        Text = usuario.fecha,
                        FontSize = 11,
                        TextColor = Color.FromArgb("#6B7280"),
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetColumn(fechaLabel, 2);

                    usuarioGrid.Children.Add(nombreLabel);
                    usuarioGrid.Children.Add(estadoLabel);
                    usuarioGrid.Children.Add(fechaLabel);

                    usuariosStack.Children.Add(usuarioGrid);
                }

                usuariosFrame.Content = usuariosStack;
                stackLayout.Children.Add(usuariosFrame);
                break;

            case "Reportes":
                // Estadísticas principales
                var estadisticasFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 20,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var estadisticasStack = new StackLayout();
                estadisticasStack.Children.Add(new Label
                {
                    Text = "📊 Estadísticas Generales",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                var estadisticasGrid = new Grid();
                estadisticasGrid.ColumnDefinitions.Add(new ColumnDefinition());
                estadisticasGrid.ColumnDefinitions.Add(new ColumnDefinition());
                estadisticasGrid.RowDefinitions.Add(new RowDefinition());
                estadisticasGrid.RowDefinitions.Add(new RowDefinition());

                // Estadística 1
                var stat1Frame = new Frame { BackgroundColor = Color.FromArgb("#EFF6FF"), CornerRadius = 8, Padding = 15, Margin = new Thickness(0, 0, 5, 5) };
                var stat1Stack = new StackLayout();
                stat1Stack.Children.Add(new Label { Text = "245", FontSize = 24, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#1E40AF") });
                stat1Stack.Children.Add(new Label { Text = "Total Usuarios", FontSize = 12, TextColor = Color.FromArgb("#6B7280") });
                stat1Frame.Content = stat1Stack;
                Grid.SetRow(stat1Frame, 0);
                Grid.SetColumn(stat1Frame, 0);

                // Estadística 2
                var stat2Frame = new Frame { BackgroundColor = Color.FromArgb("#F0FDF4"), CornerRadius = 8, Padding = 15, Margin = new Thickness(5, 0, 0, 5) };
                var stat2Stack = new StackLayout();
                stat2Stack.Children.Add(new Label { Text = "89", FontSize = 24, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#16A34A") });
                stat2Stack.Children.Add(new Label { Text = "Activos Hoy", FontSize = 12, TextColor = Color.FromArgb("#6B7280") });
                stat2Frame.Content = stat2Stack;
                Grid.SetRow(stat2Frame, 0);
                Grid.SetColumn(stat2Frame, 1);

                // Estadística 3
                var stat3Frame = new Frame { BackgroundColor = Color.FromArgb("#FEF3C7"), CornerRadius = 8, Padding = 15, Margin = new Thickness(0, 5, 5, 0) };
                var stat3Stack = new StackLayout();
                stat3Stack.Children.Add(new Label { Text = "72%", FontSize = 24, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#D97706") });
                stat3Stack.Children.Add(new Label { Text = "Prom. Asistencia", FontSize = 12, TextColor = Color.FromArgb("#6B7280") });
                stat3Frame.Content = stat3Stack;
                Grid.SetRow(stat3Frame, 1);
                Grid.SetColumn(stat3Frame, 0);

                // Estadística 4
                var stat4Frame = new Frame { BackgroundColor = Color.FromArgb("#FDF2F8"), CornerRadius = 8, Padding = 15, Margin = new Thickness(5, 5, 0, 0) };
                var stat4Stack = new StackLayout();
                stat4Stack.Children.Add(new Label { Text = "1,247", FontSize = 24, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#BE185D") });
                stat4Stack.Children.Add(new Label { Text = "Rutinas Completadas", FontSize = 12, TextColor = Color.FromArgb("#6B7280") });
                stat4Frame.Content = stat4Stack;
                Grid.SetRow(stat4Frame, 1);
                Grid.SetColumn(stat4Frame, 1);

                estadisticasGrid.Children.Add(stat1Frame);
                estadisticasGrid.Children.Add(stat2Frame);
                estadisticasGrid.Children.Add(stat3Frame);
                estadisticasGrid.Children.Add(stat4Frame);

                estadisticasStack.Children.Add(estadisticasGrid);
                estadisticasFrame.Content = estadisticasStack;
                stackLayout.Children.Add(estadisticasFrame);

                // Actividad reciente
                var actividadFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 15,
                    Margin = new Thickness(0, 15, 0, 0)
                };

                var actividadStack = new StackLayout();
                actividadStack.Children.Add(new Label
                {
                    Text = "⚡ Actividad Reciente",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                var actividades = new string[]
                {
                    "🔵 Juan Pérez completó su rutina hace 5 min",
                    "🟢 Nuevo usuario registrado: Ana López",
                    "🟡 María García actualizó su perfil",
                    "🔴 Sistema: Backup completado exitosamente",
                    "🟣 Carlos Ruiz alcanzó su meta mensual"
                };

                foreach (var actividad in actividades)
                {
                    actividadStack.Children.Add(new Label
                    {
                        Text = actividad,
                        FontSize = 13,
                        Margin = new Thickness(5, 3, 0, 3)
                    });
                }

                actividadFrame.Content = actividadStack;
                stackLayout.Children.Add(actividadFrame);
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
        AdminUsuariosOption.BackgroundColor = Colors.Transparent;
        ReportesOption.BackgroundColor = Colors.Transparent;

        // Marcar la opción seleccionada
        var colorSeleccionado = Color.FromArgb("#5B21B6");

        switch (opcionSeleccionada)
        {
            case "AdminUsuarios":
                AdminUsuariosOption.BackgroundColor = colorSeleccionado;
                break;
            case "Reportes":
                ReportesOption.BackgroundColor = colorSeleccionado;
                break;
        }
    }
}