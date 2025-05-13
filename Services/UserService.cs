using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _123123.Models;
using System.Security.Cryptography;
using System.Text;

namespace _123123.Services
{
    public class UserService
    {
        private readonly string _dataFolder = "UserData";
        private readonly string _usersFolder;

        public UserService()
        {
            // Создаем папку для данных пользователей
            _usersFolder = Path.Combine(_dataFolder, "Users");
            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }
            if (!Directory.Exists(_usersFolder))
            {
                Directory.CreateDirectory(_usersFolder);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GetUserFilePath(string username)
        {
            return Path.Combine(_usersFolder, $"{username}.txt");
        }

        public bool IsEmailTaken(string email)
        {
            if (!Directory.Exists(_usersFolder))
            {
                return false;
            }

            foreach (string file in Directory.GetFiles(_usersFolder, "*.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    string userEmail = lines.FirstOrDefault(l => l.StartsWith("Email: "))?.Substring("Email: ".Length);
                    
                    if (userEmail?.Equals(email, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return false;
        }

        public bool RegisterUser(User user)
        {
            string userFilePath = GetUserFilePath(user.Username);
            
            if (File.Exists(userFilePath))
            {
                return false; // Username already exists
            }

            if (IsEmailTaken(user.Email))
            {
                return false; // Email already exists
            }

            user.Password = HashPassword(user.Password);
            SaveUserData(user);
            return true;
        }

        public User AuthenticateUser(string username, string password)
        {
            string userFilePath = GetUserFilePath(username);
            
            if (!File.Exists(userFilePath))
            {
                return null;
            }

            string hashedPassword = HashPassword(password);
            string[] lines = File.ReadAllLines(userFilePath);

            try
            {
                string recordUsername = lines.FirstOrDefault(l => l.StartsWith("Имя пользователя: "))?.Substring("Имя пользователя: ".Length);
                string recordPassword = lines.FirstOrDefault(l => l.StartsWith("Пароль (хеш): "))?.Substring("Пароль (хеш): ".Length);
                string recordEmail = lines.FirstOrDefault(l => l.StartsWith("Email: "))?.Substring("Email: ".Length);
                string recordDate = lines.FirstOrDefault(l => l.StartsWith("Дата регистрации: "))?.Substring("Дата регистрации: ".Length);
                string recordSteamId = lines.FirstOrDefault(l => l.StartsWith("Steam ID: "))?.Substring("Steam ID: ".Length);
                string recordSteamConnected = lines.FirstOrDefault(l => l.StartsWith("Steam подключен: "))?.Substring("Steam подключен: ".Length);
                string recordDiscordId = lines.FirstOrDefault(l => l.StartsWith("Discord ID: "))?.Substring("Discord ID: ".Length);
                string recordDiscordToken = lines.FirstOrDefault(l => l.StartsWith("Discord Token: "))?.Substring("Discord Token: ".Length);
                string recordDiscordConnected = lines.FirstOrDefault(l => l.StartsWith("Discord подключен: "))?.Substring("Discord подключен: ".Length);

                if (recordPassword == hashedPassword)
                {
                    return new User
                    {
                        Username = recordUsername,
                        Password = recordPassword,
                        Email = recordEmail,
                        RegistrationDate = DateTime.Parse(recordDate),
                        SteamId = string.IsNullOrEmpty(recordSteamId) ? null : recordSteamId,
                        IsSteamConnected = recordSteamConnected == "Да",
                        DiscordId = string.IsNullOrEmpty(recordDiscordId) ? null : recordDiscordId,
                        DiscordToken = string.IsNullOrEmpty(recordDiscordToken) ? null : recordDiscordToken,
                        IsDiscordConnected = recordDiscordConnected == "Да"
                    };
                }
            }
            catch (Exception)
            {
                // Обработка ошибок при чтении файла
                return null;
            }

            return null;
        }

        public void UpdateUser(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username))
                return;

            SaveUserData(user);
        }

        private void SaveUserData(User user)
        {
            string userFilePath = GetUserFilePath(user.Username);
            string[] lines = {
                $"Имя пользователя: {user.Username}",
                $"Пароль (хеш): {user.Password}",
                $"Email: {user.Email}",
                $"Дата регистрации: {user.RegistrationDate}",
                $"Steam ID: {user.SteamId ?? ""}",
                $"Steam подключен: {(user.IsSteamConnected ? "Да" : "Нет")}",
                $"Discord ID: {user.DiscordId ?? ""}",
                $"Discord Token: {user.DiscordToken ?? ""}",
                $"Discord подключен: {(user.IsDiscordConnected ? "Да" : "Нет")}"
            };
            File.WriteAllLines(userFilePath, lines);
        }

        public List<string> GetAllUsernames()
        {
            if (!Directory.Exists(_usersFolder))
            {
                return new List<string>();
            }

            return Directory.GetFiles(_usersFolder, "*.txt")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }

        public bool DeleteUser(string username)
        {
            string userFilePath = GetUserFilePath(username);
            if (File.Exists(userFilePath))
            {
                File.Delete(userFilePath);
                return true;
            }
            return false;
        }
    }
} 