using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _123123.Models;
using _123123.Views;

namespace _123123
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User user = null)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser != null)
            {
                WelcomeText.Text = $"Добро пожаловать, {_currentUser.Username}!";
                
                // Автоматически открываем окно профиля
                var profileWindow = new ProfileView(_currentUser);
                profileWindow.Show();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void TestNotification_Click(object sender, RoutedEventArgs e)
        {
            NotificationWindow.Show("This is a test notification!");
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Необходимо войти в систему для просмотра профиля.",
                              "Ошибка доступа",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            var profileWindow = new ProfileView(_currentUser);
            profileWindow.Show();
        }
    }
}
