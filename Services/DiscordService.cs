using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _123123.Services
{
    public class DiscordService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;
        private readonly string _baseRedirectUri = "http://127.0.0.1";
        private readonly int _defaultPort = 8080;
        private readonly string _redirectPath = "/callback/";
        private readonly ConfigurationService _configService;
        private readonly string _redirectUri;

        public DiscordService()
        {
            _configService = ConfigurationService.Instance;
            _clientId = _configService.GetDiscordClientId();
            _clientSecret = _configService.GetDiscordClientSecret();
            
            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                throw new Exception("Discord client credentials not found in app.dev.json");
            }

            // Формируем redirect URI с учетом возможных настроек из конфигурации
            var port = _configService.GetDiscordCallbackPort() ?? _defaultPort;
            _redirectUri = $"{_baseRedirectUri}:{port}{_redirectPath}";

            _httpClient = new HttpClient();
            
            // Добавляем более информативный User-Agent
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                $"DiscordBot ({_baseRedirectUri}, v{version})");

            Debug.WriteLine($"Initialized DiscordService with redirect URI: {_redirectUri}");
        }

        // Добавляем метод для проверки доступности порта
        public bool IsCallbackPortAvailable()
        {
            try
            {
                var port = _configService.GetDiscordCallbackPort() ?? _defaultPort;
                var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Port availability check failed: {ex.Message}");
                return false;
            }
        }

        public string GetAuthUrl()
        {
            try
            {
                Debug.WriteLine("Generating Discord auth URL...");
                
                // Используем только официальные scopes из документации Discord
                var scopes = new[] { 
                    "identify",     // Базовая информация о пользователе (username, avatar)
                    "email",        // Email пользователя
                    "guilds"        // Список серверов пользователя
                };
                
                // Используем официальный URL для OAuth2
                var baseUrl = "https://discord.com/oauth2/authorize";
                
                // Собираем URL в точном соответствии с документацией Discord
                var queryParams = new Dictionary<string, string>
                {
                    { "client_id", _clientId },
                    { "redirect_uri", _redirectUri },
                    { "response_type", "code" },
                    { "scope", string.Join(" ", scopes) } // Используем пробел как разделитель
                };

                var queryString = string.Join("&", queryParams.Select(p => 
                    $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
                    
                var authUrl = $"{baseUrl}?{queryString}";
                
                Debug.WriteLine($"Generated auth URL: {authUrl}");
                return authUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating auth URL: {ex.Message}");
                throw new Exception("Ошибка при создании URL авторизации", ex);
            }
        }

        // Добавляем метод для получения Install Link
        public string GetInstallUrl()
        {
            return $"https://discord.com/oauth2/authorize?client_id={_clientId}";
        }

        public async Task<bool> AuthorizeWithCode(string code)
        {
            try
            {
                Debug.WriteLine("Getting tokens from code...");
                
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/oauth2/token");
                var tokenContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", _redirectUri }
                });
                tokenRequest.Content = tokenContent;

                var tokenResponse = await _httpClient.SendAsync(tokenRequest);
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                Debug.WriteLine($"Token response: {tokenJson}");
                
                if (!tokenResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка получения токена: {tokenJson}");
                }

                var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);
                var accessToken = tokenData.GetProperty("access_token").GetString();
                var refreshToken = tokenData.GetProperty("refresh_token").GetString();

                // Получаем информацию о пользователе
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                
                var userResponse = await _httpClient.GetAsync("https://discord.com/api/users/@me");
                var userJson = await userResponse.Content.ReadAsStringAsync();
                
                if (!userResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка получения информации о пользователе: {userJson}");
                }

                var userData = JsonSerializer.Deserialize<JsonElement>(userJson);
                var userId = userData.GetProperty("id").GetString();

                // Сохраняем токены
                _configService.SaveDiscordTokens(accessToken, refreshToken, userId);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in authorization: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool isOnline, string status, string activity, string details)> GetUserStatus()
        {
            try
            {
                var accessToken = _configService.GetDiscordAccessToken();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return (false, "Не авторизован", null, null);
                }

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                // Получаем информацию о пользователе
                var userResponse = await _httpClient.GetAsync("https://discord.com/api/v10/users/@me");
                var userJson = await userResponse.Content.ReadAsStringAsync();
                Debug.WriteLine($"User response: {userJson}");

                if (!userResponse.IsSuccessStatusCode)
                {
                    if (userResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _configService.ClearDiscordTokens();
                        return (false, "Требуется повторная авторизация", null, null);
                    }
                    return (false, "Ошибка получения данных", null, null);
                }

                var userData = JsonSerializer.Deserialize<JsonElement>(userJson);
                var userId = userData.GetProperty("id").GetString();

                // Получаем информацию о гильдиях пользователя с включенным presence intent
                var guildsResponse = await _httpClient.GetAsync("https://discord.com/api/v10/users/@me/guilds");
                var guildsContent = await guildsResponse.Content.ReadAsStringAsync();
                Debug.WriteLine($"Guilds response: {guildsContent}");

                if (!guildsResponse.IsSuccessStatusCode)
                {
                    return (false, "Ошибка получения данных", null, null);
                }

                var guildsList = JsonSerializer.Deserialize<JsonElement>(guildsContent);

                foreach (var guild in guildsList.EnumerateArray())
                {
                    var guildId = guild.GetProperty("id").GetString();
                    
                    // Запрашиваем данные пользователя с presence
                    var memberResponse = await _httpClient.GetAsync(
                        $"https://discord.com/api/v10/guilds/{guildId}/members/{userId}?with_presence=true");
                    
                    var memberContent = await memberResponse.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Member response for guild {guildId}: {memberContent}");
                    
                    if (memberResponse.IsSuccessStatusCode)
                    {
                        var memberData = JsonSerializer.Deserialize<JsonElement>(memberContent);

                        // Пробуем получить presence напрямую
                        var presenceResponse = await _httpClient.GetAsync(
                            $"https://discord.com/api/v10/guilds/{guildId}/presences/{userId}");
                        var presenceContent = await presenceResponse.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Presence response: {presenceContent}");

                        if (memberData.TryGetProperty("presence", out var presence) || 
                            presenceResponse.IsSuccessStatusCode)
                        {
                            var presenceData = presence;
                            if (presenceResponse.IsSuccessStatusCode)
                            {
                                presenceData = JsonSerializer.Deserialize<JsonElement>(presenceContent);
                            }

                            var status = presenceData.GetProperty("status").GetString();
                            string activityName = null;
                            string activityDetails = null;

                            if (presenceData.TryGetProperty("activities", out var activities) && 
                                activities.GetArrayLength() > 0)
                            {
                                var activity = activities[0];
                                activityName = activity.GetProperty("name").GetString();
                                
                                if (activity.TryGetProperty("details", out var details))
                                {
                                    activityDetails = details.GetString();
                                }
                            }

                            var statusText = status switch
                            {
                                "online" => "В сети",
                                "idle" => "Не активен",
                                "dnd" => "Не беспокоить",
                                "offline" => "Не в сети",
                                _ => "Неизвестно"
                            };

                            return (status != "offline", statusText, activityName, activityDetails);
                        }
                    }
                }

                return (false, "Не в сети", null, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting Discord status: {ex.Message}");
                return (false, "Ошибка", null, null);
            }
        }
    }
} 