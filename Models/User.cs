using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _123123.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _steamId;
        private bool _isSteamConnected;

        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string PhotoPath { get; set; }
        public string SteamId
        {
            get => _steamId;
            set
            {
                _steamId = value;
                OnPropertyChanged();
            }
        }
        public bool IsSteamConnected
        {
            get => _isSteamConnected;
            set
            {
                _isSteamConnected = value;
                OnPropertyChanged();
            }
        }
        public string DiscordId { get; set; }
        public string DiscordToken { get; set; }
        public bool IsDiscordConnected { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public User()
        {
            RegistrationDate = DateTime.Now;
        }
    }
} 