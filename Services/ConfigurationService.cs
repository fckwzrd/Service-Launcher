using System;
using System.IO;
using System.Text.Json;
using System.Reflection;

namespace _123123.Services
{
    public class ConfigurationService
    {
        private static ConfigurationService _instance;
        private readonly JsonDocument _appConfig;
        private readonly string _userDataPath;
        private JsonDocument _userConfig;

        private static readonly object _lock = new object();

        public static ConfigurationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigurationService();
                        }
                    }
                }
                return _instance;
            }
        }

        private ConfigurationService()
        {
            try
            {
                // Определяем путь к файлу конфигурации приложения
                string appConfigPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "app.dev.json"
                );

                // Загружаем конфигурацию приложения
                if (File.Exists(appConfigPath))
                {
                    string jsonString = File.ReadAllText(appConfigPath);
                    _appConfig = JsonDocument.Parse(jsonString);
                }
                else
                {
                    // Создаем конфигурацию по умолчанию для разработки
                    var defaultConfig = new
                    {
                        Discord = new
                        {
                            ClientId = "1371755201819840592",
                            ClientSecret = "3ruDcj_nC3NqdI6kiBOPrNBe9_ryOy4M",
                            CallbackPort = 8080
                        }
                    };
                    string jsonString = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(appConfigPath, jsonString);
                    _appConfig = JsonDocument.Parse(jsonString);
                }

                // Настраиваем путь для пользовательских данных
                string appDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "123123"
                );

                // Создаём директорию для данных пользователя, если её нет
                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                _userDataPath = Path.Combine(appDataFolder, "user_data.json");

                // Загружаем или создаём конфигурацию пользователя
                LoadOrCreateUserConfig();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка инициализации ConfigurationService: {ex.Message}", ex);
            }
        }

        private void LoadOrCreateUserConfig()
        {
            try
            {
                if (File.Exists(_userDataPath))
                {
                    string jsonString = File.ReadAllText(_userDataPath);
                    _userConfig = JsonDocument.Parse(jsonString);
                }
                else
                {
                    CreateDefaultUserConfig();
                }
            }
            catch (Exception)
            {
                CreateDefaultUserConfig();
            }
        }

        private void CreateDefaultUserConfig()
        {
            var defaultConfig = new
            {
                Discord = new
                {
                    AccessToken = "",
                    RefreshToken = "",
                    UserId = ""
                }
            };

            string jsonString = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_userDataPath, jsonString);
            _userConfig = JsonDocument.Parse(jsonString);
        }

        public string GetDiscordClientId()
        {
            try
            {
                return _appConfig?.RootElement
                    .GetProperty("Discord")
                    .GetProperty("ClientId")
                    .GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении Discord Client ID. Проверьте файл конфигурации.", ex);
            }
        }

        public string GetDiscordClientSecret()
        {
            try
            {
                return _appConfig?.RootElement
                    .GetProperty("Discord")
                    .GetProperty("ClientSecret")
                    .GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении Discord Client Secret. Проверьте файл конфигурации.", ex);
            }
        }

        public string GetDiscordAccessToken()
        {
            try
            {
                return _userConfig?.RootElement
                    .GetProperty("Discord")
                    .GetProperty("AccessToken")
                    .GetString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void SaveDiscordTokens(string accessToken, string refreshToken, string userId)
        {
            try
            {
                var config = new
                {
                    Discord = new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        UserId = userId
                    }
                };

                string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_userDataPath, jsonString);
                _userConfig = JsonDocument.Parse(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при сохранении токенов Discord", ex);
            }
        }

        public bool HasDiscordToken()
        {
            try
            {
                var token = GetDiscordAccessToken();
                return !string.IsNullOrEmpty(token);
            }
            catch
            {
                return false;
            }
        }

        public void ClearDiscordTokens()
        {
            try
            {
                SaveDiscordTokens("", "", "");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при очистке токенов Discord", ex);
            }
        }

        public int? GetDiscordCallbackPort()
        {
            try
            {
                return _appConfig?.RootElement
                    .GetProperty("Discord")
                    .GetProperty("CallbackPort")
                    .GetInt32();
            }
            catch
            {
                return null;
            }
        }
    }
} 