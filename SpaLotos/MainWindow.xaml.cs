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

        private void AddClientsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FioClientsBox.Text) && !string.IsNullOrWhiteSpace(ContactClientsBox.Text) && !string.IsNullOrWhiteSpace(AgeClientsBox.Text) && !string.IsNullOrWhiteSpace(GenderClientsBox.Text) && GenderClientsBox.Text.Length == 1)
            {
                bool isExist = false;
                foreach (object o in ClientsGrid.Items)
                    if ((o as DataRowView).Row.ItemArray[2].ToString().Equals(ContactClientsBox.Text))
                    {
                        MessageBox.Show("Клиент с таким контактом уже существует!");
                        isExist = true;
                        break;
                    }
                if (!isExist)
                {
                    db.SoloRequest($"INSERT INTO Clients(FullName,Contact,Age,Gender) VALUES ('{FioClientsBox.Text}','{ContactClientsBox.Text}',{AgeClientsBox.Text},'{GenderClientsBox.Text}')");
                    GridFill(ClientsGrid, "SELECT * FROM Clients");
                }
            }
            else
                MessageBox.Show("Неверные данные!");
        }

        private void DeleteClientsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem != null)
            {
                db.SoloRequest($"DELETE FROM Clients WHERE IdClient = {(ClientsGrid.SelectedItem as DataRowView).Row.ItemArray[0].ToString()}");
                GridFill(ClientsGrid, "SELECT * FROM Clients");
            }
            else
                MessageBox.Show("Выберите пользователя!");
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
