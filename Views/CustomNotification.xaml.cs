using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace YourNamespace.Views
{
    public partial class CustomNotification : Window
    {
        private readonly Storyboard showAnimation;
        private readonly Storyboard hideAnimation;
        private double initialTop;
        private double targetTop;

        public double InitialTop
        {
            get => initialTop;
            set
            {
                initialTop = value;
                DataContext = this;
            }
        }

        public double TargetTop
        {
            get => targetTop;
            set
            {
                targetTop = value;
                DataContext = this;
            }
        }

        public CustomNotification(string title, string message)
        {
            InitializeComponent();
            
            TitleText.Text = title;
            MessageText.Text = message;

            showAnimation = (Storyboard)FindResource("ShowAnimation");
            hideAnimation = (Storyboard)FindResource("HideAnimation");

            // Позиционируем уведомление в правом нижнем углу
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            
            InitialTop = workArea.Bottom;
            TargetTop = workArea.Bottom - Height - 20;

            Opacity = 0;
            
            hideAnimation.Completed += (s, e) => Close();
        }

        public async Task ShowAsync(int durationMs = 3000)
        {
            Show();
            showAnimation.Begin(this);

            await Task.Delay(durationMs);

            hideAnimation.Begin(this);
        }
    }
} 