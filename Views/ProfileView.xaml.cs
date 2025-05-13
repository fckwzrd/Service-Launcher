using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using _123123.Services;
using _123123.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;

namespace _123123.Views
{
    public partial class ProfileView : Window
    {
        private readonly User _currentUser;
        private readonly UserProfile _userProfile;
        private readonly SteamService _steamService;
        private readonly SteamAuthService _steamAuthService;
        private readonly DiscordService _discordService;
        private readonly LocalAuthServer _authServer;
        private readonly DispatcherTimer _steamUpdateTimer;
        private readonly DispatcherTimer _discordUpdateTimer;
        private readonly string _steamApiKey;
        private Storyboard _pulseAnimation;
        private readonly string _profilePhotosFolder = Path.Combine("UserData", "ProfilePhotos");
        private readonly string _defaultPhotoPath = Path.Combine("Assets", "default_profile.png");

        public ProfileView(User currentUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser), "Пользователь не может быть null");
            }

            InitializeComponent();
            
            _currentUser = currentUser;
            _userProfile = new UserProfile();
            _steamApiKey = "17C89B6C8325E165560F5452AB6FD1BE";
            _steamService = new SteamService(_steamApiKey);
            _steamAuthService = new SteamAuthService();
            _discordService = new DiscordService();
            _authServer = new LocalAuthServer();

            // Создаем папку для фотографий, если её нет
            if (!Directory.Exists(_profilePhotosFolder))
            {
                Directory.CreateDirectory(_profilePhotosFolder);
            }

            // Настраиваем таймер для обновления статуса Steam каждые 5 секунд
            _steamUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _steamUpdateTimer.Tick += SteamUpdateTimer_Tick;

            // Настраиваем таймер для обновления статуса Discord каждые 5 секунд
            _discordUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _discordUpdateTimer.Tick += DiscordUpdateTimer_Tick;

            try
            {
                // Инициализация UI
                UsernameText.Text = _currentUser.Username;
                LoadProfilePhoto();

                // Получаем анимацию из ресурсов
                _pulseAnimation = (Storyboard)FindResource("PulseAnimation");

                // Проверяем, есть ли сохраненная интеграция со Steam
                if (_currentUser.IsSteamConnected && !string.IsNullOrEmpty(_currentUser.SteamId))
                {
                    _userProfile.SteamId = _currentUser.SteamId;
                    UpdateSteamConnectionUI(true);
                    _ = UpdateSteamStatus(); // Запускаем первое обновление статуса
                }

                // Проверяем подключение Discord
                if (_currentUser.IsDiscordConnected && !string.IsNullOrEmpty(_currentUser.DiscordToken))
                {
                    UpdateDiscordConnectionUI(true);
                    _ = UpdateDiscordStatus(); // Запускаем первое обновление статуса
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing ProfileView: {ex.Message}");
                MessageBox.Show("Ошибка при инициализации профиля. Некоторые функции могут быть недоступны.",
                              "Ошибка инициализации",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void LoadProfilePhoto()
        {
            try
            {
                string photoPath = GetUserPhotoPath();
                if (File.Exists(photoPath))
                {
                    LoadPhotoFromPath(photoPath);
                }
                else
                {
                    LoadDefaultPhoto();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading profile photo: {ex.Message}");
                LoadDefaultPhoto();
            }
        }

        private void LoadDefaultPhoto()
        {
            try
            {
                if (File.Exists(_defaultPhotoPath))
                {
                    LoadPhotoFromPath(_defaultPhotoPath);
                }
                else
                {
                    // Если нет даже дефолтной фотографии, оставляем пустым
                    ProfileImage.ImageSource = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading default photo: {ex.Message}");
            }
        }

        private void LoadPhotoFromPath(string path)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            image.EndInit();
            image.Freeze(); // Оптимизация производительности
            ProfileImage.ImageSource = image;
        }

        private string GetUserPhotoPath()
        {
            return Path.Combine(_profilePhotosFolder, $"{_currentUser.Username}.jpg");
        }

        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string destPath = GetUserPhotoPath();
                    
                    // Если файл существует, сначала удаляем его
                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);
                    }

                    // Копируем новый файл
                    File.Copy(openFileDialog.FileName, destPath);
                    
                    // Загружаем новое изображение
                    LoadPhotoFromPath(destPath);

                    // Сохраняем изменения
                    _currentUser.PhotoPath = destPath;
                    SaveUserChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                }
            }
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string photoPath = GetUserPhotoPath();
                if (File.Exists(photoPath))
                {
                    File.Delete(photoPath);
                    _currentUser.PhotoPath = null;
                    SaveUserChanges();
                }
                LoadDefaultPhoto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении фотографии: {ex.Message}");
            }
        }

        private void UpdateSteamConnectionUI(bool isConnected)
        {
            SteamConnectButton.Visibility = isConnected ? Visibility.Collapsed : Visibility.Visible;
            SteamDisconnectButton.Visibility = isConnected ? Visibility.Visible : Visibility.Collapsed;
            SteamStatusPanel.Visibility = isConnected ? Visibility.Visible : Visibility.Collapsed;

            if (isConnected)
            {
                _steamUpdateTimer.Start();
            }
            else
            {
                _steamUpdateTimer.Stop();
                _pulseAnimation?.Stop(this);
            }
        }

        private async void ConnectSteam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var authTask = _authServer.StartAsync();

                var authUrl = _steamAuthService.GetAuthUrl();
                Process.Start(new ProcessStartInfo
                {
                    FileName = authUrl,
                    UseShellExecute = true
                });

                var responseUrl = await authTask;
                var steamId = await _steamAuthService.ValidateAndGetSteamIdAsync(responseUrl);
                
                // Сохраняем Steam ID в профиле пользователя
                _userProfile.SteamId = steamId;
                _currentUser.SteamId = steamId;
                _currentUser.IsSteamConnected = true;

                // Обновляем UI и запускаем обновление статуса
                UpdateSteamConnectionUI(true);
                await UpdateSteamStatus();

                // TODO: Сохранить изменения в базе данных
                await SaveUserChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error connecting Steam: {ex.Message}");
                MessageBox.Show($"Ошибка при подключении Steam: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _authServer.Stop();
            }
        }

        private async void DisconnectSteam_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите отключить интеграцию со Steam?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _currentUser.SteamId = null;
                _currentUser.IsSteamConnected = false;
                _userProfile.SteamId = null;

                UpdateSteamConnectionUI(false);

                // TODO: Сохранить изменения в базе данных
                await SaveUserChanges();
            }
        }

        private async Task SaveUserChanges()
        {
            try
            {
                var userService = new UserService();
                userService.UpdateUser(_currentUser);

                // Сохраняем токен Discord в конфигурации
                if (_currentUser.IsDiscordConnected)
                {
                    ConfigurationService.Instance.SaveDiscordTokens(
                        _currentUser.DiscordToken,
                        _currentUser.DiscordRefreshToken,
                        _currentUser.DiscordUserId
                    );
                }

                Debug.WriteLine("User changes saved successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving user changes: {ex.Message}");
                MessageBox.Show("Ошибка при сохранении изменений. Изменения могут быть потеряны при следующем входе.",
                              "Ошибка сохранения",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private async void SteamUpdateTimer_Tick(object sender, EventArgs e)
        {
            await UpdateSteamStatus();
        }

        private async Task UpdateSteamStatus()
        {
            try
            {
                Debug.WriteLine("Updating Steam status...");
                
                if (string.IsNullOrEmpty(_userProfile.SteamId))
                {
                    Debug.WriteLine("Steam ID is empty");
                    return;
                }

                Debug.WriteLine($"Using Steam ID: {_userProfile.SteamId}");
                var (isOnline, status, currentGame) = await _steamService.GetUserStatus(_userProfile.SteamId);
                
                Debug.WriteLine($"Status received - Online: {isOnline}, Status: {status}, Game: {currentGame}");
                
                _userProfile.IsOnline = isOnline;
                _userProfile.CurrentGame = currentGame;

                // Обновляем UI в потоке диспетчера
                Dispatcher.Invoke(() =>
                {
                    OnlineStatusText.Text = status;
                    CurrentGameText.Text = string.IsNullOrEmpty(currentGame) ? "Не играет" : currentGame;
                    LastUpdateText.Text = $"Последнее обновление: {DateTime.Now:HH:mm:ss}";

                    try
                    {
                        // Управляем анимацией в зависимости от статуса
                        if (isOnline && status != "Не беспокоить")
                        {
                            if (_pulseAnimation != null && _pulseAnimation.GetCurrentState() != ClockState.Active)
                            {
                                _pulseAnimation.Begin(this, true);
                            }
                        }
                        else
                        {
                            _pulseAnimation?.Stop(this);
                        }
                    }
                    catch (Exception animationEx)
                    {
                        Debug.WriteLine($"Animation error: {animationEx.Message}");
                        // Не показываем ошибку пользователю, так как это не критично
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateSteamStatus: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                Dispatcher.Invoke(() =>
                {
                    var errorMessage = "Ошибка при обновлении статуса Steam:\n\n";
                    
                    if (ex.Message.Contains("401"))
                    {
                        errorMessage += "Неверный API ключ Steam. Пожалуйста, проверьте правильность ключа.";
                    }
                    else if (ex.Message.Contains("403"))
                    {
                        errorMessage += "Доступ запрещен. Убедитесь, что ваш профиль Steam публичный.";
                    }
                    else if (ex.Message.Contains("429"))
                    {
                        errorMessage += "Слишком много запросов к API Steam. Пожалуйста, подождите немного.";
                    }
                    else
                    {
                        errorMessage += $"{ex.Message}\n\nПроверьте:\n- Подключение к интернету\n- Правильность API ключа\n- Публичность профиля Steam";
                    }

                    MessageBox.Show(errorMessage,
                                  "Ошибка обновления",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);

                    // Останавливаем таймер при критических ошибках
                    if (ex.Message.Contains("401") || ex.Message.Contains("403"))
                    {
                        _steamUpdateTimer.Stop();
                    }
                });
            }
        }

        private void UpdateDiscordConnectionUI(bool isConnected)
        {
            Dispatcher.Invoke(() =>
            {
                Debug.WriteLine($"Updating Discord UI, isConnected: {isConnected}");
                
                DiscordConnectButton.Visibility = isConnected ? Visibility.Collapsed : Visibility.Visible;
                DiscordDisconnectButton.Visibility = isConnected ? Visibility.Visible : Visibility.Collapsed;
                DiscordStatusPanel.Visibility = isConnected ? Visibility.Visible : Visibility.Collapsed;

                Debug.WriteLine($"Discord panel visibility: {DiscordStatusPanel.Visibility}");
                Debug.WriteLine($"Connect button visibility: {DiscordConnectButton.Visibility}");
                Debug.WriteLine($"Disconnect button visibility: {DiscordDisconnectButton.Visibility}");

                if (isConnected)
                {
                    _discordUpdateTimer?.Start();
                    Debug.WriteLine("Discord update timer started");
                }
                else
                {
                    _discordUpdateTimer?.Stop();
                    DiscordOnlineIndicator.BeginAnimation(OpacityProperty, null);
                    DiscordOnlineIndicator.Opacity = 1;
                    Debug.WriteLine("Discord update timer stopped");
                }
            });
        }

        private async void ConnectDiscord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Starting Discord connection process...");
                var authTask = _authServer.StartAsync();

                var authUrl = _discordService.GetAuthUrl();
                Process.Start(new ProcessStartInfo
                {
                    FileName = authUrl,
                    UseShellExecute = true
                });

                var code = await authTask;
                Debug.WriteLine("Received authorization code from Discord");

                var success = await _discordService.AuthorizeWithCode(code);
                if (success)
                {
                    UpdateDiscordConnectionUI(true);
                    await UpdateDiscordStatus();
                    MessageBox.Show("Discord успешно подключен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось подключить Discord. Попробуйте еще раз.", 
                                  "Ошибка", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error connecting Discord: {ex.Message}");
                MessageBox.Show($"Ошибка при подключении Discord:\n\n{ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
            finally
            {
                _authServer.Stop();
            }
        }

        private async void DisconnectDiscord_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите отключить интеграцию с Discord?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _currentUser.DiscordId = null;
                _currentUser.DiscordToken = null;
                _currentUser.IsDiscordConnected = false;

                UpdateDiscordConnectionUI(false);
                await SaveUserChanges();
            }
        }

        private async void DiscordUpdateTimer_Tick(object sender, EventArgs e)
        {
            await UpdateDiscordStatus();
        }

        private async Task UpdateDiscordStatus()
        {
            try
            {
                if (!_currentUser.IsDiscordConnected) return;

                var status = await _discordService.GetUserStatus(_currentUser.DiscordToken);
                
                // Обновляем UI в основном потоке
                Dispatcher.Invoke(() =>
                {
                    if (status != null)
                    {
                        DiscordStatusText.Text = status.Status;
                        DiscordGameText.Text = status.Game ?? "Не в игре";
                        
                        // Добавляем дополнительную информацию о статусе
                        if (!string.IsNullOrEmpty(status.CustomStatus))
                        {
                            DiscordCustomStatusText.Text = status.CustomStatus;
                            DiscordCustomStatusPanel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            DiscordCustomStatusPanel.Visibility = Visibility.Collapsed;
                        }

                        // Показываем время в игре, если доступно
                        if (!string.IsNullOrEmpty(status.GameStartTime))
                        {
                            DateTime startTime = DateTime.Parse(status.GameStartTime);
                            TimeSpan playTime = DateTime.Now - startTime;
                            DiscordPlayTimeText.Text = $"Время в игре: {playTime.Hours}ч {playTime.Minutes}м";
                            DiscordPlayTimePanel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            DiscordPlayTimePanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        DiscordStatusText.Text = "Не в сети";
                        DiscordGameText.Text = "";
                        DiscordCustomStatusPanel.Visibility = Visibility.Collapsed;
                        DiscordPlayTimePanel.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating Discord status: {ex.Message}");
                Dispatcher.Invoke(() =>
                {
                    DiscordStatusText.Text = "Ошибка обновления";
                    // Не показываем ошибку пользователю, просто логируем
                });
            }
        }

        // Добавляем метод для ручного обновления статуса
        private void RefreshDiscordStatus_Click(object sender, RoutedEventArgs e)
        {
            _ = UpdateDiscordStatus();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Останавливаем только таймер Steam
            _steamUpdateTimer?.Stop();

            // Сохраняем Discord токен перед закрытием
            if (_currentUser.IsDiscordConnected)
            {
                SaveUserChanges().Wait();
            }

            // НЕ отключаем Discord при закрытии окна
            // Оставляем _discordUpdateTimer работающим
        }
    }
} 