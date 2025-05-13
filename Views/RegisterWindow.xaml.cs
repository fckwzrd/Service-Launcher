using System;
using System.Windows;
using System.Windows.Media;
using _123123.Models;
using _123123.Services;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Threading;
using System.Threading.Tasks;

namespace _123123.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly LanguageService _languageService;
        private readonly Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private readonly Regex usernameRegex = new Regex(@"^(?!^\d+$)[a-zA-Z0-9]{4,15}$");
        private readonly Regex passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[-@$!%*#?&])[A-Za-z\d\-@$!%*#?&]{6,}$");
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWindowClosing;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        // Поля для хранения текстов подсказок
        private string _loginTooltipHeaderText;
        private string _loginTooltipMinLengthText;
        private string _loginTooltipCharsText;
        private string _loginTooltipUniqueText;

        private string _emailTooltipHeaderText;
        private string _emailTooltipFormatText;
        private string _emailTooltipValidText;
        private string _emailTooltipUniqueText;

        private string _passwordTooltipHeaderText;
        private string _passwordTooltipMinLengthText;
        private string _passwordTooltipUppercaseText;
        private string _passwordTooltipLowercaseText;
        private string _passwordTooltipNumberText;
        private string _passwordTooltipSpecialText;

        private string _confirmPasswordTooltipHeaderText;
        private string _confirmPasswordTooltipMatchText;

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

        public string LoginTooltipMinLengthText
        {
            get => _loginTooltipMinLengthText;
            set
            {
                _loginTooltipMinLengthText = value;
                OnPropertyChanged();
            }
        }

        public string LoginTooltipCharsText
        {
            get => _loginTooltipCharsText;
            set
            {
                _loginTooltipCharsText = value;
                OnPropertyChanged();
            }
        }

        public string LoginTooltipUniqueText
        {
            get => _loginTooltipUniqueText;
            set
            {
                _loginTooltipUniqueText = value;
                OnPropertyChanged();
            }
        }

        public string EmailTooltipHeaderText
        {
            get => _emailTooltipHeaderText;
            set
            {
                _emailTooltipHeaderText = value;
                OnPropertyChanged();
            }
        }

        public string EmailTooltipFormatText
        {
            get => _emailTooltipFormatText;
            set
            {
                _emailTooltipFormatText = value;
                OnPropertyChanged();
            }
        }

        public string EmailTooltipValidText
        {
            get => _emailTooltipValidText;
            set
            {
                _emailTooltipValidText = value;
                OnPropertyChanged();
            }
        }

        public string EmailTooltipUniqueText
        {
            get => _emailTooltipUniqueText;
            set
            {
                _emailTooltipUniqueText = value;
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

        public string PasswordTooltipMinLengthText
        {
            get => _passwordTooltipMinLengthText;
            set
            {
                _passwordTooltipMinLengthText = value;
                OnPropertyChanged();
            }
        }

        public string PasswordTooltipUppercaseText
        {
            get => _passwordTooltipUppercaseText;
            set
            {
                _passwordTooltipUppercaseText = value;
                OnPropertyChanged();
            }
        }

        public string PasswordTooltipLowercaseText
        {
            get => _passwordTooltipLowercaseText;
            set
            {
                _passwordTooltipLowercaseText = value;
                OnPropertyChanged();
            }
        }

        public string PasswordTooltipNumberText
        {
            get => _passwordTooltipNumberText;
            set
            {
                _passwordTooltipNumberText = value;
                OnPropertyChanged();
            }
        }

        public string PasswordTooltipSpecialText
        {
            get => _passwordTooltipSpecialText;
            set
            {
                _passwordTooltipSpecialText = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPasswordTooltipHeaderText
        {
            get => _confirmPasswordTooltipHeaderText;
            set
            {
                _confirmPasswordTooltipHeaderText = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPasswordTooltipMatchText
        {
            get => _confirmPasswordTooltipMatchText;
            set
            {
                _confirmPasswordTooltipMatchText = value;
                OnPropertyChanged();
            }
        }

        private string _currentMessageKey;
        private bool _isErrorMessage;
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

        private string _registerTitle;
        public string RegisterTitle
        {
            get => _registerTitle;
            set
            {
                _registerTitle = value;
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

        private string _emailField;
        public string EmailField
        {
            get => _emailField;
            set
            {
                _emailField = value;
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

        private string _confirmPasswordField;
        public string ConfirmPasswordField
        {
            get => _confirmPasswordField;
            set
            {
                _confirmPasswordField = value;
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

        private string _haveAccountText;
        public string HaveAccountText
        {
            get => _haveAccountText;
            set
            {
                _haveAccountText = value;
                OnPropertyChanged();
            }
        }

        public class LanguageItem
        {
            public Language Language { get; set; }
            public string DisplayName { get; set; }
        }

        public RegisterWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            _languageService = LanguageService.Instance;
            DataContext = this;
            MessageText = string.Empty;

            // Инициализируем значения подсказок
            UpdateUITexts();

            InitializeLanguageComboBox();

            // Устанавливаем начальное значение времени
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");

            // Инициализируем обновление времени
            _cancellationTokenSource = new CancellationTokenSource();
            StartTimeUpdate(_cancellationTokenSource.Token);
        }

        private void UpdateUITexts()
        {
            RegisterTitle = _languageService.GetMessage("RegisterTitle");
            LoginField = _languageService.GetMessage("LoginField");
            EmailField = _languageService.GetMessage("EmailField");
            PasswordField = _languageService.GetMessage("PasswordField");
            ConfirmPasswordField = _languageService.GetMessage("ConfirmPasswordField");
            RegisterButtonText = _languageService.GetMessage("RegisterButton");
            LoginButtonText = _languageService.GetMessage("LoginButton");
            HaveAccountText = _languageService.GetMessage("HaveAccount");

            // Обновление текстов подсказок
            LoginTooltipHeaderText = _languageService.GetMessage("LoginTooltipHeader");
            LoginTooltipMinLengthText = _languageService.GetMessage("LoginTooltipMinLength");
            LoginTooltipCharsText = _languageService.GetMessage("LoginTooltipChars");
            LoginTooltipUniqueText = _languageService.GetMessage("MustBeUnique");

            EmailTooltipHeaderText = _languageService.GetMessage("EmailTooltipHeader");
            EmailTooltipFormatText = _languageService.GetMessage("EmailTooltipFormat");
            EmailTooltipValidText = _languageService.GetMessage("EmailTooltipValid");
            EmailTooltipUniqueText = _languageService.GetMessage("MustBeUnique");

            PasswordTooltipHeaderText = _languageService.GetMessage("PasswordTooltipHeader");
            PasswordTooltipMinLengthText = _languageService.GetMessage("PasswordTooltipMinLength");
            PasswordTooltipUppercaseText = _languageService.GetMessage("PasswordTooltipUppercase");
            PasswordTooltipLowercaseText = _languageService.GetMessage("PasswordTooltipLowercase");
            PasswordTooltipNumberText = _languageService.GetMessage("PasswordTooltipNumber");
            PasswordTooltipSpecialText = _languageService.GetMessage("PasswordTooltipSpecial");

            ConfirmPasswordTooltipHeaderText = _languageService.GetMessage("ConfirmPasswordTooltipHeader");
            ConfirmPasswordTooltipMatchText = _languageService.GetMessage("ConfirmPasswordTooltipMatch");

            // Обновляем текущее сообщение, если оно есть
            if (!string.IsNullOrEmpty(_currentMessageKey))
            {
                ShowMessage(_currentMessageKey, _isErrorMessage);
            }
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

            StatusBarLanguageComboBox.ItemsSource = languages;
            StatusBarLanguageComboBox.SelectedItem = languages.First(x => x.Language == _languageService.CurrentLanguage);
            StatusBarLanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowMessage("FillAllFields");
                return;
            }

            // Проверка логина
            if (!usernameRegex.IsMatch(username))
            {
                ShowMessage("InvalidUsername");
                return;
            }

            // Проверка email
            if (!emailRegex.IsMatch(email))
            {
                ShowMessage("InvalidEmail");
                return;
            }

            // Проверка пароля
            if (!passwordRegex.IsMatch(password))
            {
                ShowMessage("InvalidPassword");
                PasswordBox.Password = string.Empty;
                ConfirmPasswordBox.Password = string.Empty;
                return;
            }

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                ShowMessage("PasswordsDoNotMatch");
                PasswordBox.Password = string.Empty;
                ConfirmPasswordBox.Password = string.Empty;
                return;
            }

            // Проверка уникальности email
            if (_userService.IsEmailTaken(email))
            {
                ShowMessage("EmailTaken");
                EmailTextBox.Text = string.Empty;
                return;
            }

            User newUser = new User
            {
                Username = username,
                Email = email,
                Password = password
            };

            // Проверка уникальности логина и регистрация
            if (_userService.RegisterUser(newUser))
            {
                NotificationWindow.Show(username);
                var profileWindow = new ProfileView(newUser);
                profileWindow.Show();
                this.Close();
            }
            else
            {
                ShowMessage("UsernameTaken");
                UsernameTextBox.Text = string.Empty;
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

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
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

        private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (e.VerticalChange != 0 && Math.Abs(e.VerticalChange) > 0.1)
                {
                    // Сохраняем позицию прокрутки только при значительном изменении
                    _lastScrollPosition = scrollViewer.VerticalOffset;
                }
                else if (e.ExtentHeightChange != 0)
                {
                    // Предотвращаем автоматическую прокрутку при изменении размера контента
                    scrollViewer.ScrollToVerticalOffset(_lastScrollPosition);
                }
            }
        }

        private double _lastScrollPosition = 0;
    }
} 