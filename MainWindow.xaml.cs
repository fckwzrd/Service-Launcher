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
        private ProfileView _profileWindow;

        public MainWindow(User user = null)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser != null)
            {
                // Открываем окно профиля
                _profileWindow = new ProfileView(_currentUser);
                _profileWindow.Show();
                
                // Скрываем главное окно
                this.Hide();
                
                // Подписываемся на закрытие окна профиля
                _profileWindow.Closed += (s, e) => 
                {
                    // Показываем главное окно при закрытии профиля
                    this.Show();
                };
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

            if (_profileWindow == null || !_profileWindow.IsVisible)
            {
                _profileWindow = new ProfileView(_currentUser);
                _profileWindow.Show();
                this.Hide();
                
                _profileWindow.Closed += (s, e) => 
                {
                    this.Show();
                };
            }
        }
    }
}
