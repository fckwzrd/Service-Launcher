using System;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using _123123.Services;
using System.Windows.Input;

namespace _123123.Views
{
    public class BaseWindow : Window, INotifyPropertyChanged
    {
        protected readonly LanguageService _languageService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWindowClosing;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public BaseWindow()
        {
            _languageService = LanguageService.Instance;
            DataContext = this;

            // Инициализируем обновление времени
            _cancellationTokenSource = new CancellationTokenSource();
            StartTimeUpdate(_cancellationTokenSource.Token);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        private async void StartTimeUpdate(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Синхронизируем с началом следующей секунды
                    var now = DateTime.Now;
                    var delay = 1000 - now.Millisecond;
                    try
                    {
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }

                    // Обновляем время сразу после начала новой секунды
                    CurrentTime = DateTime.Now.ToString("HH:mm:ss");
                }
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _isWindowClosing = true;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            base.OnClosing(e);
        }

        protected void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
} 