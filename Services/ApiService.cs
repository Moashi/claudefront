// Services/ApiService/ApiService.cs - USANDO TUS MODELOS CORRECTOS
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FrontFitLife.Models.LoginModels; // ✅ Solo este using necesitas

namespace FrontFitLife.Services.ApiService
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService()
        {
            _httpClient = new HttpClient();

#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                _baseUrl = "http://10.0.2.2:44328/api/";
            }
            else
            {
                _baseUrl = "https://localhost:44328/api/";
            }
#else
            _baseUrl = "https://tu-servidor.com/api/";
#endif

            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

        }

        #region Authentication Methods

        // ✅ ARREGLADO: Ahora usa LoginResponse (tu modelo correcto)
        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            try
            {


                var request = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                // Debug: Mostrar request
                var requestJson = JsonSerializer.Serialize(request, _jsonOptions);


                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");


                var httpResponse = await _httpClient.PostAsync($"{_baseUrl}user/login", content);


                var responseContent = await httpResponse.Content.ReadAsStringAsync();


                if (httpResponse.IsSuccessStatusCode)
                {
                    try
                    {
                        // ✅ ARREGLADO: Deserializar a LoginResponse (tu modelo)
                        var response = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);

                 

                        if (response.Error != null && response.Error.Any()){
                         
                      
                        }

                        // Configurar header si login exitoso
                        if (response.IsSuccess)
                        {
                            await SetAuthorizationHeaderAsync(response.Token);
                          
                        }

                        return response;
                    }
                    catch (JsonException jsonEx)
                    {
                       

                        return new LoginResponse
                        {
                            Result = false,
                            Error = new List<ErrorModel>
                            {
                                new ErrorModel
                                {
                                    ErrorCode = -1,
                                    Message = $"Error de formato en respuesta del servidor: {jsonEx.Message}"
                                }
                            }
                        };
                    }
                }
                else
                {
                   

                    return new LoginResponse
                    {
                        Result = false,
                        Error = new List<ErrorModel>
                        {
                            new ErrorModel
                            {
                                ErrorCode = (int)httpResponse.StatusCode,
                                Message = $"Error HTTP {httpResponse.StatusCode}: {responseContent}"
                            }
                        }
                    };
                }
            }
            catch (HttpRequestException httpEx)
            {
               
                return new LoginResponse
                {
                    Result = false,
                    Error = new List<ErrorModel>
                    {
                        new ErrorModel
                        {
                            ErrorCode = -1,
                            Message = $"Error de conexión: No se pudo conectar al servidor. Verifica que esté ejecutándose en {_baseUrl}"
                        }
                    }
                };
            }
            catch (TaskCanceledException timeoutEx)
            {
              
                return new LoginResponse
                {
                    Result = false,
                    Error = new List<ErrorModel>
                    {
                        new ErrorModel
                        {
                            ErrorCode = -1,
                            Message = "Tiempo de espera agotado. El servidor tardó demasiado en responder."
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Result = false,
                    Error = new List<ErrorModel>
                    {
                        new ErrorModel
                        {
                            ErrorCode = -1,
                            Message = $"Error inesperado: {ex.Message}"
                        }
                    }
                };
            }
        }

        #endregion

        #region User Data Methods

        public async Task<GetUserProfileResponse> GetUserDataAsync()
        {
            try
            {
              
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }
                await SetAuthorizationHeaderAsync(token);
                var response = await _httpClient.GetAsync($"{_baseUrl}user/profile");
                if (response.IsSuccessStatusCode)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<GetUserProfileResponse>(userJson, _jsonOptions);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileRequest updateRequest)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔄 Actualizando perfil de usuario en servidor...");

                // Verificar token
                if (string.IsNullOrEmpty(updateRequest.Token))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ No hay token en el request");
                    return false;
                }

                // Configurar headers de autorización
                await SetAuthorizationHeaderAsync(updateRequest.Token);

                // Serializar el request - CORREGIDO: Enviar solo los campos necesarios
                var requestJson = JsonSerializer.Serialize(updateRequest, _jsonOptions);
                System.Diagnostics.Debug.WriteLine($"🔄 Enviando datos: {requestJson}");

                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // CORREGIDO: Usar PATCH en lugar de PUT y endpoint correcto
                var response = await _httpClient.PatchAsync($"{_baseUrl}user/update_profile", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"🔄 Update Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"🔄 Update Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // Intentar deserializar la respuesta
                        var updateResponse = JsonSerializer.Deserialize<UpdateUserProfileResponse>(responseContent, _jsonOptions);

                        if (updateResponse != null && updateResponse.Result)
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ Perfil actualizado exitosamente en servidor");
                            return true;
                        }
                        else
                        {
                            var errorMsg = updateResponse?.Error?.FirstOrDefault()?.Message ?? "Error desconocido";
                            System.Diagnostics.Debug.WriteLine($"❌ Error en respuesta del servidor: {errorMsg}");
                            return false;
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Error deserializando respuesta: {jsonEx.Message}");
                        // Si no se puede deserializar pero el status es exitoso, asumir que funcionó
                        System.Diagnostics.Debug.WriteLine($"✅ Perfil actualizado (respuesta no JSON pero status exitoso)");
                        return true;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error HTTP actualizando perfil: {response.StatusCode}");

                    // Intentar obtener más detalles del error
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<UpdateUserProfileResponse>(responseContent, _jsonOptions);
                        if (errorResponse?.Error?.Any() == true)
                        {
                            var errorMsg = errorResponse.Error.First().Message;
                            System.Diagnostics.Debug.WriteLine($"❌ Error específico: {errorMsg}");
                        }
                    }
                    catch
                    {
                        // Ignorar si no se puede deserializar el error
                    }

                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error de conexión actualizando perfil: {httpEx.Message}");
                return false;
            }
            catch (TaskCanceledException timeoutEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Timeout actualizando perfil: {timeoutEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error inesperado actualizando perfil: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Helper Methods

        private async Task SetAuthorizationHeaderAsync(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                System.Diagnostics.Debug.WriteLine($"🔑 Token configurado: {token.Substring(0, Math.Min(10, token.Length))}...");
            }
        }

        #endregion

        #region Data Storage Methods



        public async Task SaveUserDataAsync(string token, User user)
        {
            try
            {
                await SecureStorage.SetAsync("auth_token", token);
                await SecureStorage.SetAsync("user_data", JsonSerializer.Serialize(user, _jsonOptions));
                await SetAuthorizationHeaderAsync(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error saving user data: {ex.Message}");
            }
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync("auth_token");
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> GetUserAsync()
        {
            try
            {
                var userJson = await SecureStorage.GetAsync("user_data");
                if (!string.IsNullOrEmpty(userJson))
                {
                    return JsonSerializer.Deserialize<User>(userJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting user: {ex.Message}");
            }
            return null;
        }

        public void ClearUserData()
        {
            try
            {
                SecureStorage.RemoveAll();
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error clearing user data: {ex.Message}");
            }
        }

        #endregion

        #region Test Methods

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl.TrimEnd('/'));

                return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔍 Test falló: {ex.Message}");
                return false;
            }
        }

        public void LogDeviceInfo()
        {
            System.Diagnostics.Debug.WriteLine($"📱 === INFORMACIÓN DEL DISPOSITIVO ===");
            System.Diagnostics.Debug.WriteLine($"📱 Platform: {DeviceInfo.Platform}");
            System.Diagnostics.Debug.WriteLine($"📱 Version: {DeviceInfo.VersionString}");
            System.Diagnostics.Debug.WriteLine($"🌐 Base URL: {_baseUrl}");

            var connectivity = Connectivity.NetworkAccess;
            System.Diagnostics.Debug.WriteLine($"🌐 Network Access: {connectivity}");
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        #endregion
    }
}