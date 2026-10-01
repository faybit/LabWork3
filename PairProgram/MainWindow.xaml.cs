using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;

namespace PairProgram
{
    public partial class MainWindow : Window
    {
        private const string DbPath = "users.csv";
        private readonly UserService _userService = new();

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(DbPath))
                _userService.ImportFromCsvLines(File.ReadAllLines(DbPath));
        }

        private void SaveToCsv(string path)
        {
            var lines = new List<string> { "Username;PasswordHash" };
            lines.AddRange(_userService.Users.Select(u => $"{u.Username};{u.PasswordHash}"));
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        private void RefreshUserList()
        {
            UsersListBox.ItemsSource = null;
            UsersListBox.ItemsSource = _userService.Users.Select(u => u.Username).ToList();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_userService.Login(LoginTextBox.Text.Trim(), PassBox.Password))
            {
                AuthPanel.Visibility = Visibility.Collapsed;
                DataPanel.Visibility = Visibility.Visible;
                RefreshUserList();
            }
            else
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text.Trim();
            var pass = PassBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Заполните все поля", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_userService.Register(login, pass))
            {
                SaveToCsv(DbPath);
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                AuthPanel.Visibility = Visibility.Collapsed;
                DataPanel.Visibility = Visibility.Visible;
                RefreshUserList();
            }
            else
                MessageBox.Show("Пользователь уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv", FileName = "users_export.csv" };
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToCsv(saveFileDialog.FileName);
                MessageBox.Show("Экспорт выполнен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "CSV files (*.csv)|*.csv" };
            if (openFileDialog.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(openFileDialog.FileName);
                int count = _userService.ImportFromCsvLines(lines);
                SaveToCsv(DbPath);
                RefreshUserList();
                MessageBox.Show($"Импортировано новых записей: {count}", "Импорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Clear();
            PassBox.Clear();
            DataPanel.Visibility = Visibility.Collapsed;
            AuthPanel.Visibility = Visibility.Visible;
        }
    }
}