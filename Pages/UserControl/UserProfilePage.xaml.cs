using FrontFitLife.Models.LoginModels;
using FrontFitLife.Services.ApiService;
using System.Text.Json;

namespace FrontFitLife.Pages.UserControl;

public partial class UserProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private User _currentUser;
    private bool _isLoading = false;
    private bool _hasUnsavedChanges = false;

    public UserProfilePage()
    {
        try
        {
            InitializeComponent();
            _apiService = new ApiService();
            System.Diagnostics.Debug.WriteLine("✅ UserProfilePage inicializada correctamente");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en constructor UserProfilePage: {ex.Message}");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            System.Diagnostics.Debug.WriteLine("🔄 OnAppearing iniciado");
            await CargarDatosUsuario();
            System.Diagnostics.Debug.WriteLine("✅ OnAppearing completado");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en OnAppearing: {ex.Message}");
            await DisplayAlert("Error", $"Error cargando página: {ex.Message}", "OK");
        }
    }

    private async Task CargarDatosUsuario()
    {
        try
        {
            SetLoadingState(true);

            System.Diagnostics.Debug.WriteLine("🔄 Cargando datos del usuario...");

            // Intentar obtener datos del usuario desde almacenamiento local
            _currentUser = await _apiService.GetUserAsync();

            if (_currentUser == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ No se encontraron datos de usuario locales");
                await MostrarErrorYVolver("No se pudieron cargar los datos del usuario. Inicia sesión nuevamente.");
                return;
            }

            // Intentar obtener datos actualizados del servidor
            var token = await _apiService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var serverUserData = await _apiService.GetUserDataAsync();
                    if (serverUserData != null && serverUserData.IsSuccess && serverUserData.User != null)
                    {
                        _currentUser = serverUserData.User;
                        System.Diagnostics.Debug.WriteLine("✅ Datos actualizados desde el servidor");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("⚠️ Usando datos locales (servidor no disponible o error)");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Error obteniendo datos del servidor: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine("⚠️ Usando datos locales");
                }
            }

            // Mostrar datos en la interfaz
            MostrarDatosEnInterfaz();

            System.Diagnostics.Debug.WriteLine($"✅ Datos cargados para: {_currentUser.FirstName} {_currentUser.LastName}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error cargando datos: {ex.Message}");
            await DisplayAlert("Error", "No se pudieron cargar los datos del perfil.", "OK");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void MostrarDatosEnInterfaz()
    {
        if (_currentUser == null) return;

        try
        {
            // Información del header
            NombreCompletoLabel.Text = _currentUser.FullName;
            RolLabel.Text = _currentUser.Role ?? "Usuario";

            // Estado con colores
            EstadoLabel.Text = _currentUser.Status ?? "Activo";
            EstadoFrame.BackgroundColor = (_currentUser.Status?.ToLower()) switch
            {
                "active" or "activo" => Color.FromArgb("#10B981"), // Verde
                "suspended" or "suspendido" => Color.FromArgb("#F59E0B"), // Amarillo
                "delinquent" or "moroso" => Color.FromArgb("#EF4444"), // Rojo
                _ => Color.FromArgb("#6B7280") // Gris por defecto
            };

            // Información personal (solo lectura)
            CedulaLabel.Text = _currentUser.Cedula ?? "No especificada";
            EmailLabel.Text = _currentUser.Email ?? "No especificado";

            if (_currentUser.BirthDate.HasValue)
            {
                FechaNacimientoLabel.Text = _currentUser.BirthDate.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                FechaNacimientoLabel.Text = "No especificada";
            }

            // Campos editables
            TelefonoEntry.Text = _currentUser.PhoneNumber ?? "";
            DireccionEditor.Text = _currentUser.Address ?? "";

            // Configurar eventos para detectar cambios
            TelefonoEntry.TextChanged -= OnCampoEditableChanged; // Remover evento anterior
            DireccionEditor.TextChanged -= OnCampoEditableChanged;

            TelefonoEntry.TextChanged += OnCampoEditableChanged;
            DireccionEditor.TextChanged += OnCampoEditableChanged;

            _hasUnsavedChanges = false;
            GuardarButton.BackgroundColor = Color.FromArgb("#10B981"); // Verde normal
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error mostrando datos: {ex.Message}");
        }
    }

    private void OnCampoEditableChanged(object sender, EventArgs e)
    {
        _hasUnsavedChanges = true;
        GuardarButton.BackgroundColor = Color.FromArgb("#059669"); // Verde más intenso
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (_isLoading) return;

        try
        {
            System.Diagnostics.Debug.WriteLine("💾 Iniciando guardado de perfil...");

            // Validar campos
            if (!ValidarCampos())
            {
                System.Diagnostics.Debug.WriteLine("❌ Validación de campos falló");
                return;
            }

            SetLoadingState(true);

            // Obtener token
            var token = await _apiService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Error", "Sesión expirada. Inicia sesión nuevamente.", "OK");
                await Shell.Current.GoToAsync("//Login");
                return;
            }

            // Preparar datos para actualizar - CORREGIDO: Usar la clase correcta
            var updateRequest = new UpdateUserProfileRequest
            {
                Token = token,
                PhoneNumber = TelefonoEntry.Text?.Trim(),
                Address = DireccionEditor.Text?.Trim()
            };

            // Validar que los campos no estén vacíos (según tu backend)
            if (string.IsNullOrEmpty(updateRequest.PhoneNumber))
            {
                await DisplayAlert("Error", "El teléfono es requerido", "OK");
                return;
            }

            if (string.IsNullOrEmpty(updateRequest.Address))
            {
                await DisplayAlert("Error", "La dirección es requerida", "OK");
                return;
            }

            // Actualizar datos locales temporalmente
            var originalPhone = _currentUser.PhoneNumber;
            var originalAddress = _currentUser.Address;

            _currentUser.PhoneNumber = updateRequest.PhoneNumber;
            _currentUser.Address = updateRequest.Address;

            // Intentar actualizar en el servidor - CORREGIDO: Usar el request correcto
            bool updateSuccess = false;
            try
            {
                updateSuccess = await _apiService.UpdateUserProfileAsync(updateRequest);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error actualizando en servidor: {ex.Message}");
                // Restaurar datos originales si falla
                _currentUser.PhoneNumber = originalPhone;
                _currentUser.Address = originalAddress;
            }

            if (updateSuccess)
            {
                // Guardar cambios localmente
                await _apiService.SaveUserDataAsync(token, _currentUser);

                _hasUnsavedChanges = false;
                GuardarButton.BackgroundColor = Color.FromArgb("#10B981"); // Verde normal

                await DisplayAlert("Éxito", "Perfil actualizado correctamente.", "OK");
                System.Diagnostics.Debug.WriteLine("✅ Perfil actualizado exitosamente");
            }
            else
            {
                // Restaurar datos originales si falla
                _currentUser.PhoneNumber = originalPhone;
                _currentUser.Address = originalAddress;

                await DisplayAlert("Error",
                    "No se pudo actualizar el perfil. Verifica tu conexión e intenta nuevamente.",
                    "OK");
                System.Diagnostics.Debug.WriteLine("❌ Error actualizando perfil en servidor");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Excepción al guardar: {ex.Message}");
            await DisplayAlert("Error", "Ocurrió un error inesperado al guardar los cambios.", "OK");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private async void OnCambiarPasswordClicked(object sender, EventArgs e)
    {
        // Por ahora mostrar mensaje, después se puede implementar
        await DisplayAlert("Cambiar Contraseña",
            "Funcionalidad de cambio de contraseña en desarrollo.",
            "OK");
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await VerificarCambiosSinGuardarYNavegar();
    }

    private async Task VerificarCambiosSinGuardarYNavegar()
    {
        // Verificar si hay cambios sin guardar
        if (_hasUnsavedChanges)
        {
            bool continuar = await DisplayAlert("Cambios sin guardar",
                "Tienes cambios sin guardar. ¿Deseas salir sin guardar?",
                "Sí, salir", "Cancelar");

            if (!continuar)
                return;
        }

        await Shell.Current.GoToAsync("..");
    }

    private bool ValidarCampos()
    {
        bool esValido = true;

        // Limpiar errores previos
        LimpiarErrores();

        // Validar teléfono
        string telefono = TelefonoEntry.Text?.Trim();
        if (!string.IsNullOrEmpty(telefono))
        {
            if (!EsNumeroTelefonoValido(telefono))
            {
                MostrarErrorCampo(TelefonoErrorLabel, "Formato de teléfono inválido (8 dígitos)");
                esValido = false;
            }
        }

        // Validar dirección
        string direccion = DireccionEditor.Text?.Trim();
        if (!string.IsNullOrEmpty(direccion))
        {
            if (direccion.Length < 10)
            {
                MostrarErrorCampo(DireccionErrorLabel, "La dirección debe tener al menos 10 caracteres");
                esValido = false;
            }
        }

        return esValido;
    }

    private bool EsNumeroTelefonoValido(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
            return true; // Opcional

        // Remover espacios, guiones y paréntesis
        string telefonoLimpio = telefono.Replace(" ", "")
                                      .Replace("-", "")
                                      .Replace("(", "")
                                      .Replace(")", "")
                                      .Replace("+", "");

        // Validar que sea solo números y tenga 8 dígitos (Costa Rica)
        return telefonoLimpio.All(char.IsDigit) && telefonoLimpio.Length == 8;
    }

    private void MostrarErrorCampo(Label errorLabel, string mensaje)
    {
        errorLabel.Text = mensaje;
        errorLabel.IsVisible = true;
    }

    private void LimpiarErrores()
    {
        TelefonoErrorLabel.IsVisible = false;
        DireccionErrorLabel.IsVisible = false;
    }

    private void SetLoadingState(bool isLoading)
    {
        _isLoading = isLoading;
        LoadingOverlay.IsVisible = isLoading;

        // Deshabilitar/habilitar controles
        GuardarButton.IsEnabled = !isLoading;
        CambiarPasswordButton.IsEnabled = !isLoading;
        VolverButton.IsEnabled = !isLoading;
        TelefonoEntry.IsEnabled = !isLoading;
        DireccionEditor.IsEnabled = !isLoading;

        if (isLoading)
        {
            GuardarButton.Text = "💾 Guardando...";
        }
        else
        {
            GuardarButton.Text = "💾 Guardar Cambios";
        }
    }

    private async Task MostrarErrorYVolver(string mensaje)
    {
        await DisplayAlert("Error", mensaje, "OK");
        await Shell.Current.GoToAsync("//Login");
    }

    protected override bool OnBackButtonPressed()
    {
        // Manejar el botón atrás del dispositivo
        if (_hasUnsavedChanges)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await VerificarCambiosSinGuardarYNavegar();
            });

            return true; // Prevenir navegación automática
        }

        return base.OnBackButtonPressed();
    }
}