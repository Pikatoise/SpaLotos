using System;
using System.Collections.Generic;
using System.Data;
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
            StatusUpdate();

            if (role.Equals("admin"))
                UserBlock.Text = "Пользователь: Администратор";
            else
            {
                UserBlock.Text = "Пользователь: Оператор-кассир";
                AdminButton.Visibility = Visibility.Collapsed;
            }

            GridFill(ClientsGrid, "SELECT * FROM Clients");
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(AdminPage,AdminButton);
            }
            else
            {
                StatusUpdate();
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
                StatusUpdate();
            }

            GridFill(ClientsGrid, "SELECT * FROM Clients");
        }

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(ServicesPage, ServicesButton);
            }
            else
            {
                StatusUpdate();
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
                StatusUpdate();
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

        void StatusUpdate()
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

        void GridFill(DataGrid grid, string request)
        {
            grid.ItemsSource = null;
            grid.ItemsSource = db.TableRequest(request).DefaultView;
        }

    }
}
