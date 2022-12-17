using System.Windows;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Windows.Media.Animation;
using System;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Threading.Tasks;

namespace SpaLotos
{
    public partial class AuthWindow : Window
    {
        DBContext db = new DBContext();

        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (db.Status)
                StatusBlock.Text = "Статус сервера: включен";
            else
            {
                StatusBlock.Text = "Статус сервера: отключен";
                LoginBox.IsEnabled= false;
                PasswordBox.IsEnabled= false;
                AuthButton.IsEnabled= false;
            }

            ShadowAnimation();
        }

        private void LoginBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoginBox.Foreground = Brushes.Black;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox.Foreground = Brushes.Black;
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text.Length <= 0 || PasswordBox.Password.Length <= 0)
            {
                LoginBox.Foreground = Brushes.Red;
                PasswordBox.Foreground = Brushes.Red;
            }
            else
            {
                string role = db.SoloRequest($"SELECT Role FROM Users Where Login = '{LoginBox.Text}' AND Password = '{PasswordBox.Password}'");

                if (role != null)
                {
                    MainWindow main = new MainWindow(role,db);
                    main.Show();
                    Close();
                }
                else
                {
                    LoginBox.Text = "";
                    PasswordBox.Password = "";
                    LoginBox.Foreground = Brushes.Red;
                    PasswordBox.Foreground = Brushes.Red;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        async void ShadowAnimation()
        {
            while (true)
            {
                if (ShadowEffect.Direction >= 360)
                    ShadowEffect.Direction = 0;

                ShadowEffect.Direction += 10;

                await Task.Delay(50);
            }
        }
    }
}
