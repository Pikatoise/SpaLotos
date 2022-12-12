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
using System.Windows.Shapes;

namespace SpaLotos
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Grid currentPage;
        Button currentMenuItem;
        TextBlock statusBlock;

        string role;
        DBContext db;

        public MainWindow(string Role, DBContext db)
        {
            InitializeComponent();
            this.Height = SystemParameters.PrimaryScreenHeight*0.95;
            this.Width = SystemParameters.PrimaryScreenWidth;

            currentPage = ClientsPage;
            currentMenuItem = ClientsButton;
            statusBlock = StatusBlock;

            role = Role;
            this.db = db;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Status();

            if (role.Equals("admin"))
                UserBlock.Text = "Пользователь: Администратор";
            else
            {
                UserBlock.Text = "Пользователь: Оператор-кассир";
                AdminButton.Visibility = Visibility.Collapsed;
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(AdminPage,AdminButton);
            }
            else
            {
                Status();
            }
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(ClientsPage, ClientsButton);
            }
            else
            {
                Status();
            }
        }

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(ServicesPage, ServicesButton);
            }
            else
            {
                Status();
            }
        }

        private void ServenButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(ServePage, ServeButton);
            }
            else
            {
                Status();
            }
        }

        void ChangePage(Grid nextPage, Button nextMenuButton)
        {
            currentMenuItem.IsEnabled= true;
            currentMenuItem = nextMenuButton;
            currentMenuItem.IsEnabled= false;

            currentPage.Visibility= Visibility.Hidden;
            currentPage = nextPage;
            currentPage.Visibility= Visibility.Visible;
        }

        void Status()
        {
            if (db.Status)
            {
                statusBlock.Text = "Подключен";
                statusBlock.Foreground = Brushes.LimeGreen;
            }
            else
            {
                statusBlock.Text = "Отключен";
                statusBlock.Foreground = Brushes.Red;
            }
        }

    }
}
