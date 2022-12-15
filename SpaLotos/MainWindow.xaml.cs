using MySql.Data.MySqlClient;
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

        #region Вкладка Админ
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

        #endregion

        #region Вкладка Клиенты
        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(ClientsPage, ClientsButton);
                GridFill(ClientsGrid, "SELECT * FROM Clients");

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
                db.SoloRequest($"DELETE FROM Clients WHERE IdClient = {(ClientsGrid.SelectedItem as DataRowView).Row.ItemArray[0]}");
                GridFill(ClientsGrid, "SELECT * FROM Clients");
            }
            else
                MessageBox.Show("Выберите пользователя!");
        }

        private void SearchClientsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchClientsBox.Text))
            {
                GridFill(ClientsGrid,$"SELECT * FROM Clients WHERE Contact = '{SearchClientsBox.Text}'");
                SearchClientsPanel.Visibility= Visibility.Collapsed;
                UndoClientsButton.Visibility= Visibility.Visible;
            }
            else
                MessageBox.Show("Введите контакт клиента!");
        }

        private void UndoClientsButton_Click(object sender, RoutedEventArgs e)
        {
            GridFill(ClientsGrid, "SELECT * FROM Clients");
            UndoClientsButton.Visibility= Visibility.Collapsed;
            SearchClientsPanel.Visibility= Visibility.Visible;
        }
        #endregion

        #region Вкладка Услуги
        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(ServicesPage, ServicesButton);
                GridFill(ServicesGrid, "SELECT * FROM Services");
            }
            else
            {
                StatusUpdate();
            }

        }

        private void AddServicesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PriceServicesBox.Text) && !string.IsNullOrWhiteSpace(NameServicesBox.Text))
            {
                
                db.SoloRequest($"INSERT INTO Services(Name,Price) VALUES ('{NameServicesBox.Text}',{PriceServicesBox.Text})");
                GridFill(ServicesGrid, "SELECT * FROM Services");
            }
            else
                MessageBox.Show("Неверные данные!");
        }

        private void DeleteServicesButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesGrid.SelectedItem != null)
            {
                db.SoloRequest($"DELETE FROM Services WHERE IdService = {(ServicesGrid.SelectedItem as DataRowView).Row.ItemArray[0]}");
                GridFill(ServicesGrid, "SELECT * FROM Services");
            }
            else
                MessageBox.Show("Выберите услугу!");
        }

        private void UndoServicesButton_Click(object sender, RoutedEventArgs e)
        {
            GridFill(ServicesGrid, "SELECT * FROM Services");
            UndoServicesButton.Visibility= Visibility.Collapsed;
            SearchServicesPanel.Visibility= Visibility.Visible;
        }

        private void SearchServicesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchServicesBox.Text))
            {
                GridFill(ServicesGrid, $"SELECT * FROM Services WHERE Name LIKE '%{SearchServicesBox.Text}%'");
                SearchServicesPanel.Visibility= Visibility.Collapsed;
                UndoServicesButton.Visibility= Visibility.Visible;
            }
            else
                MessageBox.Show("Введите название услуги!");
        }
        #endregion

        #region Вкладка Обслуживание
        private void ServenButton_Click(object sender, RoutedEventArgs e)
        {
            if (db.Status)
            {
                ChangePage(ServePage, ServeButton);
                ListBoxFill(ServeListBox,"SELECT * FROM Sessions");

                MakedServicesGrid.ItemsSource = null;
            }
            else
            {
                StatusUpdate();
            }
        }

        private void ServeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServeListBox.Items.Count > 0)
                GridFill(MakedServicesGrid, $"SELECT ms.IdMakedService,s.Name,s.Price,c.FullName,c.Contact,w.FullNameWorker FROM Sessions as ss JOIN MakedService as ms ON ms.IdSession = ss.IdSession JOIN Services as s ON s.IdService = ms.IdService JOIN Clients as c ON c.IdClient = ms.IdClient JOIN Workers as w ON w.IdWorker = ms.IdWorker WHERE ss.IdSession = {(ServeListBox.SelectedItem as ListBoxItem).Tag}; ");
        }

        private void AddSessionButton_Click(object sender, RoutedEventArgs e)
        {
            db.SoloRequest($"INSERT INTO Sessions(starttime) VALUES('{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss")}')");
            ListBoxFill(ServeListBox, "SELECT * FROM Sessions");
        }

        private void DeleteSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServeListBox.SelectedItem != null)
            {
                string currentSession = (ServeListBox.SelectedItem as ListBoxItem).Tag.ToString();
                db.SoloRequest($"DELETE FROM Cheque WHERE IdSession = {currentSession}");
                db.SoloRequest($"DELETE FROM MakedService WHERE IdSession = {currentSession}");
                db.SoloRequest($"DELETE FROM Sessions WHERE IdSession = {currentSession}");
                ListBoxFill(ServeListBox, "SELECT * FROM Sessions");

            }
            else
                MessageBox.Show("Выберите сессию!");
        }

        private void AddMakedServiceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteMakedServiceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChequeButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Общие методы
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

        void ListBoxFill(ListBox lb, string request)
        {
            lb.Items.Clear();

            MySqlDataReader reader = db.MultyRequest(request);

            while (reader.Read())
            {
                ListBoxItem item = new ListBoxItem()
                {
                    Content = $"Сессия №{reader["IdSession"]}\n{reader["StartTime"]}",
                    Tag = reader["IdSession"].ToString()
                };

                lb.Items.Add(item);
            }


            reader.Close();
        }
        #endregion

    }
}
