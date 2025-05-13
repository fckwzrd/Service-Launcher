using System;
using System.Windows;
using System.Windows.Media;
using _123123.Models;
using _123123.Services;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace _123123.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly LanguageService _languageService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWindowClosing;
        private string _currentMessageKey;
        private bool _isErrorMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Поля для хранения текстов подсказок
        private string _loginTooltipHeaderText;
        private string _loginTooltipText;
        private string _passwordTooltipHeaderText;
        private string _passwordTooltipText;

        // Поле для хранения текущего времени
        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged();
                }
            }
        }

        // Свойства для привязки текстов
        private string _loginTitle;
        public string LoginTitle
        {
            get => _loginTitle;
            set
            {
                _loginTitle = value;
                OnPropertyChanged();
            }
        }

        private string _loginField;
        public string LoginField
        {
            get => _loginField;
            set
            {
                _loginField = value;
                OnPropertyChanged();
            }
        }

        private string _passwordField;
        public string PasswordField
        {
            get => _passwordField;
            set
            {
                _passwordField = value;
                OnPropertyChanged();
            }
        }

        private string _loginButtonText;
        public string LoginButtonText
        {
            get => _loginButtonText;
            set
            {
                _loginButtonText = value;
                OnPropertyChanged();
            }
        }

        private string _registerButtonText;
        public string RegisterButtonText
        {
            get => _registerButtonText;
            set
            {
                _registerButtonText = value;
                OnPropertyChanged();
            }
        }

        private string _noAccountText;
        public string NoAccount
        {
            get => _noAccountText;
            set
            {
                _noAccountText = value;
                OnPropertyChanged();
            }
        }

        // Свойства с поддержкой уведомлений
        public string LoginTooltipHeaderText
        {
            get => _loginTooltipHeaderText;
            set
            {
                _loginTooltipHeaderText = value;
                OnPropertyChanged();
            }
        }

        public string LoginTooltipText
        {
            get => _loginTooltipText;
            set
            {
                _loginTooltipText = value;
                OnPropertyChanged();
            }
        }

        public string PasswordTooltipHeaderText
        {
            get => _passwordTooltipHeaderText;
            set
            {
                _passwordTooltipHeaderText = value;
                OnPropertyChanged();
            }
        }

        public string PasswordTooltipText
        {
            get => _passwordTooltipText;
            set
            {
                _passwordTooltipText = value;
                OnPropertyChanged();
            }
        }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged();
            }
        }

        public class LanguageItem
        {
            public Services.Language Language { get; set; }
            public string DisplayName { get; set; }
        }

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            _languageService = LanguageService.Instance;
            DataContext = this;
            MessageText = string.Empty;
            InitializeLanguageComboBox();

            // Устанавливаем начальное значение времени
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");

            // Инициализируем обновление времени
            _cancellationTokenSource = new CancellationTokenSource();
            StartTimeUpdate(_cancellationTokenSource.Token);

            // Инициализируем значения подсказок
            UpdateUITexts();
        }

        private void InitializeLanguageComboBox()
        {
            var languages = new List<LanguageItem>
            {
                new LanguageItem { Language = Services.Language.English, DisplayName = "English" },
                new LanguageItem { Language = Services.Language.Russian, DisplayName = "Русский" },
                new LanguageItem { Language = Services.Language.Chinese, DisplayName = "中文" },
                new LanguageItem { Language = Services.Language.Portuguese, DisplayName = "Português" },
                new LanguageItem { Language = Services.Language.Spanish, DisplayName = "Español" },
                new LanguageItem { Language = Services.Language.French, DisplayName = "Français" },
                new LanguageItem { Language = Services.Language.German, DisplayName = "Deutsch" }
            };

            LanguageComboBox.ItemsSource = languages;
            LanguageComboBox.SelectedItem = languages.First(x => x.Language == _languageService.CurrentLanguage);
            LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
        }

        private void ShowMessage(string messageKey, bool isError = true)
        {
            _currentMessageKey = messageKey;
            _isErrorMessage = isError;
            MessageText = _languageService.GetMessage(messageKey);
            if (MessageTextBlock != null)
            {
                MessageTextBlock.Foreground = isError ? Brushes.Red : Brushes.Green;
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is LanguageItem selectedItem)
            {
                _languageService.SetLanguage(selectedItem.Language);
                UpdateUITexts();
            }
        }

        private void UpdateUITexts()
        {
            LoginTitle = _languageService.GetMessage("LoginTitle");
            LoginField = _languageService.GetMessage("LoginField");
            PasswordField = _languageService.GetMessage("PasswordField");
            LoginButtonText = _languageService.GetMessage("LoginButton");
            RegisterButtonText = _languageService.GetMessage("RegisterButton");
            NoAccount = _languageService.GetMessage("NoAccount");

            // Обновление текстов подсказок
            LoginTooltipHeaderText = _languageService.GetMessage("LoginTooltipHeader");
            LoginTooltipText = _languageService.GetMessage("LoginTooltipText");

            PasswordTooltipHeaderText = _languageService.GetMessage("PasswordTooltipHeader");
            PasswordTooltipText = _languageService.GetMessage("PasswordTooltipText");

            // Обновляем текущее сообщение, если оно есть
            if (!string.IsNullOrEmpty(_currentMessageKey))
            {
                ShowMessage(_currentMessageKey, _isErrorMessage);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("FillAllFields");
                return;
            }

            var user = _userService.AuthenticateUser(username, password);
            if (user != null)
            {
                var profileWindow = new ProfileView(user);
                profileWindow.Show();
                this.Close();
            }
            else
            {
                ShowMessage("InvalidCredentials");
                PasswordBox.Password = string.Empty;
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void StartTimeUpdate(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Синхронизируем с началом следующей секунды
                    var now = DateTime.Now;
                    var delay = 1000 - now.Millisecond;
                    await Task.Delay(delay, cancellationToken);

                    // Проверяем, не закрыто ли окно
                    if (_isWindowClosing) break;

                    // Обновляем время сразу после начала новой секунды
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        CurrentTime = DateTime.Now.ToString("HH:mm:ss");
                    });
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _isWindowClosing = true;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            base.OnClosing(e);
        }
    }
} 