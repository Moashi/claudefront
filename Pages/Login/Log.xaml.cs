// Pages/Login/Log.xaml.cs - ACTUALIZADO PARA LoginResponse
using FrontFitLife.Models.LoginModels; // ✅ Usando tus modelos
using FrontFitLife.Services.ApiService;

namespace FrontFitLife.Pages.Login;

public partial class Log : ContentPage
{
    private bool _isPasswordVisible = false;
    private readonly ApiService _apiService;

    public Log()
    {
        InitializeComponent();
        _apiService = new ApiService();

        // Debug info al inicializar
        _apiService.LogDeviceInfo();
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        ContrasenaEntry.IsPassword = !_isPasswordVisible;
        TogglePasswordButton.Text = _isPasswordVisible ? "\ue8f5" : "\ue8f4";
    }

    private async void OnIniciarSesionClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"🔥 === BOTÓN LOGIN PRESIONADO ===");

        // Limpiar errores previos
        LimpiarErrores();

        // Validar campos
        if (!ValidarCampos())
        {
            System.Diagnostics.Debug.WriteLine($"❌ Validación de campos falló");
            return;
        }

        // Mostrar estado de carga
        SetLoadingState(true);

        try
        {
            // Probar conexión primero
            System.Diagnostics.Debug.WriteLine($"🔍 Probando conexión al servidor...");
            bool conexionOk = await _apiService.TestConnectionAsync();
            System.Diagnostics.Debug.WriteLine($"🔍 Conexión: {(conexionOk ? "✅ OK" : "❌ FALLO")}");

            // Llamar a la API
            System.Diagnostics.Debug.WriteLine($"🚀 Llamando LoginAsync...");
            var resultado = await RealizarLoginConAPI();

            System.Diagnostics.Debug.WriteLine($"📨 Resultado recibido:");
            System.Diagnostics.Debug.WriteLine($"   - Result: {resultado.Result}");
            System.Diagnostics.Debug.WriteLine($"   - IsSuccess: {resultado.IsSuccess}");
            System.Diagnostics.Debug.WriteLine($"   - User: {(resultado.User != null ? "✅ PRESENTE" : "❌ NULL")}");
            System.Diagnostics.Debug.WriteLine($"   - Token: {(!string.IsNullOrEmpty(resultado.Token) ? "✅ PRESENTE" : "❌ NULL/EMPTY")}");
            System.Diagnostics.Debug.WriteLine($"   - Errors: {resultado.Error?.Count ?? 0}");

            // ✅ ARREGLADO: Usar IsSuccess en lugar de Result && User != null
            if (resultado.IsSuccess)
            {
                System.Diagnostics.Debug.WriteLine($"✅ LOGIN EXITOSO para {resultado.User.Email}");

                // Login exitoso - guardar datos del usuario
                await GuardarDatosUsuario(resultado);

                // Mostrar mensaje de bienvenida
                await DisplayAlert("Bienvenido",
                    $"¡Hola {resultado.User.FirstName}! Has iniciado sesión correctamente.",
                    "Continuar");

                // Navegar según el rol del usuario
                await NavegarSegunRol(resultado.User.Role);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ LOGIN FALLÓ");

                // Login fallido - mostrar errores específicos
                await MostrarErroresLogin(resultado);
            }
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine($"🌐 HttpRequestException: {httpEx.Message}");
            await DisplayAlert("Sin Conexión",
                "No se pudo conectar al servidor. Verifica:\n\n" +
                "1. Tu conexión a internet\n" +
                "2. Que el servidor esté ejecutándose\n" +
                "3. La URL del servidor",
                "OK");
        }
        catch (TaskCanceledException timeoutEx)
        {
            System.Diagnostics.Debug.WriteLine($"⏰ TaskCanceledException: {timeoutEx.Message}");
            await DisplayAlert("Tiempo Agotado",
                "La solicitud tardó demasiado. El servidor puede estar lento o no disponible.",
                "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"💥 Exception general: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"💥 Stack trace: {ex.StackTrace}");

            await DisplayAlert("Error Inesperado",
                $"Ocurrió un error inesperado:\n{ex.Message}\n\nRevisa los logs para más detalles.",
                "OK");
        }
        finally
        {
            SetLoadingState(false);
            System.Diagnostics.Debug.WriteLine($"🏁 === PROCESO LOGIN TERMINADO ===");
        }
    }

    // ✅ ARREGLADO: Ahora devuelve LoginResponse
    private async Task<LoginResponse> RealizarLoginConAPI()
    {
        string email = UsuarioEntry.Text?.Trim() ?? string.Empty;
        string password = ContrasenaEntry.Text ?? string.Empty;

        System.Diagnostics.Debug.WriteLine($"📧 Email a enviar: '{email}'");
        System.Diagnostics.Debug.WriteLine($"🔐 Password length: {password.Length}");

        // Llamar al servicio API
        var response = await _apiService.LoginAsync(email, password);

        return response;
    }

    // ✅ ARREGLADO: Ahora recibe LoginResponse
    private async Task GuardarDatosUsuario(LoginResponse loginResult)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"💾 Guardando datos de usuario...");

            // Validar que los datos estén completos
            if (string.IsNullOrEmpty(loginResult.Token) || loginResult.User == null)
            {
                throw new InvalidOperationException("Datos de login incompletos");
            }

            // Guardar datos básicos en Preferences (no sensibles)
            Preferences.Set("user_email", loginResult.User.Email ?? "");
            Preferences.Set("user_firstname", loginResult.User.FirstName ?? "");
            Preferences.Set("user_lastname", loginResult.User.LastName ?? "");
            Preferences.Set("user_role", loginResult.User.Role ?? "User");
            Preferences.Set("user_cedula", loginResult.User.Cedula ?? "");
            Preferences.Set("is_logged_in", true);

            if (loginResult.ExpiresAt.HasValue)
            {
                Preferences.Set("token_expires", loginResult.ExpiresAt.Value.ToString("O"));
            }

            // Para datos sensibles, usar SecureStorage
            await SecureStorage.SetAsync("auth_token", loginResult.Token);

            // Guardar datos completos del usuario en JSON
            var userJson = System.Text.Json.JsonSerializer.Serialize(loginResult.User);
            await SecureStorage.SetAsync("user_data", userJson);

            System.Diagnostics.Debug.WriteLine($"✅ Datos guardados correctamente para {loginResult.User.Email}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error guardando datos: {ex.Message}");
            await DisplayAlert("Advertencia",
                "Se pudo iniciar sesión pero hubo un problema guardando algunos datos localmente.",
                "OK");
        }
    }

    private async Task NavegarSegunRol(string role)
    {
        try
        {
            string rolUsuario = role?.ToLower() ?? "user";
            System.Diagnostics.Debug.WriteLine($"🧭 Navegando según rol: {rolUsuario}");

            switch (rolUsuario)
            {
                case "admin":
                case "administrador":
                    await Shell.Current.GoToAsync("//AdminDashboard");
                    break;

                case "staff":
                case "empleado":
                    await Shell.Current.GoToAsync("//StaffDashboard");
                    break;

                case "user":
                case "usuario":
                default:
                    await Shell.Current.GoToAsync("//UserDashboard");
                    break;
            }

            System.Diagnostics.Debug.WriteLine($"✅ Navegación exitosa");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error en navegación: {ex.Message}");
            // Intentar ir a página por defecto
            await Shell.Current.GoToAsync("//UserDashboard");
        }
    }

    // ✅ ARREGLADO: Ahora recibe LoginResponse
    private async Task MostrarErroresLogin(LoginResponse resultado)
    {
        string mensajeError = "Error al iniciar sesión";
        string tituloError = "Login Fallido";

        if (resultado.Error != null && resultado.Error.Count > 0)
        {
            var primerError = resultado.Error.First();

            System.Diagnostics.Debug.WriteLine($"❌ Error principal: Code={primerError.ErrorCode}, Message='{primerError.Message}'");

            // Mapear códigos de error a mensajes amigables
            switch (primerError.ErrorCode)
            {
                case 32: // credencialesInvalidas
                    tituloError = "Credenciales Incorrectas";
                    mensajeError = "El email o contraseña no son correctos. Verifica tus datos.";
                    break;

                case 4: // correoFaltante
                    mensajeError = "Debes ingresar tu correo electrónico.";
                    break;

                case 5: // passwordFaltante
                    mensajeError = "Debes ingresar tu contraseña.";
                    break;

                case 6: // correoIncorrecto
                    mensajeError = "El formato del correo electrónico no es válido.";
                    break;

                case -1: // Error de conexión
                    tituloError = "Error de Conexión";
                    mensajeError = "No se pudo conectar al servidor. Verifica tu conexión.";
                    break;

                default:
                    mensajeError = primerError.Message ?? "Error desconocido al iniciar sesión.";
                    break;
            }
        }

        await DisplayAlert(tituloError, mensajeError, "OK");
    }

    private async void OnOlvidasteContrasenaClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Recuperar Contraseña",
            "Funcionalidad de recuperación de contraseña en desarrollo.",
            "OK");
    }

    private bool ValidarCampos()
    {
        bool esValido = true;

        if (string.IsNullOrWhiteSpace(UsuarioEntry.Text))
        {
            MostrarErrorCampo(UsuarioErrorLabel, "El email es requerido");
            esValido = false;
        }
        else if (!EsEmailValido(UsuarioEntry.Text))
        {
            MostrarErrorCampo(UsuarioErrorLabel, "Ingresa un email válido");
            esValido = false;
        }

        if (string.IsNullOrWhiteSpace(ContrasenaEntry.Text))
        {
            MostrarErrorCampo(ContrasenaErrorLabel, "La contraseña es requerida");
            esValido = false;
        }

        return esValido;
    }

    private void MostrarErrorCampo(Label errorLabel, string mensaje)
    {
        errorLabel.Text = mensaje;
        errorLabel.IsVisible = true;
    }

    private bool EsEmailValido(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void LimpiarErrores()
    {
        UsuarioErrorLabel.IsVisible = false;
        ContrasenaErrorLabel.IsVisible = false;
    }

    private void SetLoadingState(bool isLoading)
    {
        IniciarSesionButton.IsEnabled = !isLoading;
        UsuarioEntry.IsEnabled = !isLoading;
        ContrasenaEntry.IsEnabled = !isLoading;
        TogglePasswordButton.IsEnabled = !isLoading;

        if (isLoading)
        {
            IniciarSesionButton.Text = "Iniciando...";
        }
        else
        {
            IniciarSesionButton.Text = "Iniciar Sesión";
        }
    }

    public void LimpiarCampos()
    {
        UsuarioEntry.Text = string.Empty;
        ContrasenaEntry.Text = string.Empty;
        _isPasswordVisible = false;
        ContrasenaEntry.IsPassword = true;
        TogglePasswordButton.Text = "\ue8f4";
        LimpiarErrores();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LimpiarCampos();
        UsuarioEntry.Focus();
    }
}