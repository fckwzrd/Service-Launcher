using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using _123123.Models;

namespace _123123.Services
{
    public class SteamService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey; // You'll need to get this from https://steamcommunity.com/dev/apikey

        // Steam persona states
        private enum PersonaState
        {
            Offline = 0,
            Online = 1,
            Busy = 2,
            Away = 3,
            Snooze = 4,
            LookingToTrade = 5,
            LookingToPlay = 6
        }

        public SteamService(string apiKey)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _apiKey = apiKey;
        }

        public async Task<(bool isOnline, string status, string currentGame)> GetUserStatus(string steamId)
        {
            try
            {
                Debug.WriteLine($"Fetching status for Steam ID: {steamId}");
                var apiUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_apiKey}&steamids={steamId}";
                Debug.WriteLine($"Request URL: {apiUrl}");

                var response = await _httpClient.GetAsync(apiUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Steam API Error: Status {response.StatusCode}, Content: {errorContent}");
                    throw new Exception($"Steam API returned error: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Steam API Response: {content}");

                var data = JsonSerializer.Deserialize<JsonElement>(content);
                var players = data.GetProperty("response").GetProperty("players");

                if (players.GetArrayLength() > 0)
                {
                    var player = players[0];
                    var personaState = (PersonaState)player.GetProperty("personastate").GetInt32();
                    Debug.WriteLine($"Persona State: {personaState}");

                    // Проверяем наличие игры
                    string currentGame = "";

                    // Затем проверяем название игры
                    if (player.TryGetProperty("gameextrainfo", out JsonElement gameInfo))
                    {
                        currentGame = gameInfo.GetString();
                        Debug.WriteLine($"Found game name: {currentGame}");
                    }

                    string status = personaState switch
                    {
                        PersonaState.Online => "В сети",
                        PersonaState.Busy => "Не беспокоить",
                        PersonaState.Away => "Не беспокоить",
                        PersonaState.Snooze => "Не беспокоить",
                        PersonaState.Offline => "Не в сети",
                        _ => "В сети"
                    };

                    bool isOnline = personaState != PersonaState.Offline;
                    Debug.WriteLine($"Final status - Online: {isOnline}, Status: {status}, Game: {currentGame}");

                    return (isOnline, status, currentGame);
                }

                Debug.WriteLine("No player data found in response");
                return (false, "Не в сети", string.Empty);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Request Error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new Exception("Ошибка подключения к Steam API. Проверьте подключение к интернету.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting Steam status: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
} 