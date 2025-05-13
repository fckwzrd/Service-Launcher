using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using _123123.Services;

namespace _123123.Views
{
    public partial class NotificationWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly Storyboard _fadeIn;
        private readonly Storyboard _fadeOut;
        private readonly LanguageService _languageService;

        public NotificationWindow(string username)
        {
            InitializeComponent();
            _languageService = LanguageService.Instance;

            // Устанавливаем тексты в соответствии с текущим языком
            WelcomeText.Text = _languageService.GetFormattedMessage("WelcomeMessage", username);
            StatusText.Text = _languageService.GetMessage("RegistrationSuccessful");

            _fadeIn = (Storyboard)FindResource("FadeIn");
            _fadeOut = (Storyboard)FindResource("FadeOut");

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _timer.Tick += Timer_Tick;

            // Position the window in the bottom right corner
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Bottom - Height - 20;

            // Start fade in animation
            _fadeIn.Begin(this);
            _timer.Start();

            // Добавляем возможность перетаскивания окна
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            };
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            CloseWithAnimation();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            CloseWithAnimation();
        }

        private void CloseWithAnimation()
        {
            _fadeOut.Completed += (s, _) => Close();
            _fadeOut.Begin(this);
        }

        public static void Show(string username)
        {
            var window = new NotificationWindow(username);
            window.Show();
        }
    }
} 