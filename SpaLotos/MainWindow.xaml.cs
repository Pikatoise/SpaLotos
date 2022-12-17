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
                GridFill(WorkersGrid,"SELECT w.IdWorker,w.FullNameWorker,p.Name,p.Salary,w.Contact,w.Birthday FROM Workers as w JOIN Positions as p ON p.IdPosition = w.IdPosition");
                GridFill(PositionsGrid, "SELECT * FROM Positions");
                GridFill(UsersGrid,"SELECT * FROM Users");
                ComboBoxFill(PositionComboBox,"IdPosition,Name,Salary","Positions");
            }
            else
            {
                StatusUpdate();
            }
        }

        private void AddWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            if (BirthdayWorkerPicker.SelectedDate != null && !string.IsNullOrWhiteSpace(ContactWorkersBox.Text) && !string.IsNullOrWhiteSpace(FioWorkersBox.Text) && PositionComboBox.SelectedItem != null)
            {
                bool isExist = false;
                foreach (object o in WorkersGrid.Items)
                    if ((o as DataRowView).Row.ItemArray[4].ToString().Equals(ContactWorkersBox.Text))
                    {
                        MessageBox.Show("Работник с таким контактом уже существует!");
                        isExist = true;
                        break;
                    }
                if (!isExist)
                {
                    db.SoloRequest($"INSERT INTO Workers(IdPosition,Contact,Birthday,FullNameWorker) VALUES ({(PositionComboBox.SelectedItem as ComboBoxItem).Tag},'{ContactWorkersBox.Text}','{Convert.ToDateTime(BirthdayWorkerPicker.SelectedDate).ToString("yyyy-MM-dd")}','{FioWorkersBox.Text}')");
                    GridFill(WorkersGrid, "SELECT w.IdWorker,w.FullNameWorker,p.Name,p.Salary,w.Contact,w.Birthday FROM Workers as w JOIN Positions as p ON p.IdPosition = w.IdPosition");
                }
            }
            else
                MessageBox.Show("Неверные данные!");
        }

        private void DeleteWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersGrid.SelectedItem != null)
            {
                db.SoloRequest($"DELETE FROM Workers WHERE IdWorker = {(WorkersGrid.SelectedItem as DataRowView).Row.ItemArray[0]}");
                GridFill(WorkersGrid, "SELECT w.IdWorker,w.FullNameWorker,p.Name,p.Salary,w.Contact,w.Birthday FROM Workers as w JOIN Positions as p ON p.IdPosition = w.IdPosition");

            }
            else
                MessageBox.Show("Выберите работника!");
        }

        private void AddPositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PositionNameBox.Text) && !string.IsNullOrWhiteSpace(SalaryBox.Text))
            {
                bool isExist = false;
                foreach (object o in PositionsGrid.Items)
                    if ((o as DataRowView).Row.ItemArray[1].ToString().Equals(PositionNameBox.Text))
                    {
                        MessageBox.Show("Должность с таким названием уже существует!");
                        isExist = true;
                        break;
                    }
                if (!isExist)
                {
                    db.SoloRequest($"INSERT INTO Positions(Name,Salary) VALUES ('{PositionNameBox.Text}',{SalaryBox.Text})");
                    GridFill(PositionsGrid, "SELECT * FROM Positions");
                }
            }
            else
                MessageBox.Show("Неверные данные!");
        }

        private void DeletePositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (PositionsGrid.SelectedItem != null)
            {
                db.SoloRequest($"DELETE FROM Positions WHERE IdPosition = {(PositionsGrid.SelectedItem as DataRowView).Row.ItemArray[0]}");
                GridFill(PositionsGrid, "SELECT * FROM Positions");
            }
            else
                MessageBox.Show("Выберите должность!");
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(LoginBox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Text) && !string.IsNullOrWhiteSpace(RoleComboBox.Text))
            {
                bool isExist = false;
                foreach (object o in UsersGrid.Items)
                    if ((o as DataRowView).Row.ItemArray[1].ToString().Equals(LoginBox.Text))
                    {
                        MessageBox.Show("Аккаунт с таким логином уже существует!");
                        isExist = true;
                        break;
                    }
                if (!isExist)
                {
                    db.SoloRequest($"INSERT INTO Users(Login,Password,Role) VALUES ('{LoginBox.Text}','{PasswordBox.Text}','{RoleComboBox.Text}')");
                    GridFill(UsersGrid, "SELECT * FROM Users");
                }
            }
            else
                MessageBox.Show("Неверные данные!");
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem != null)
            {
                db.SoloRequest($"DELETE FROM Users WHERE IdUser = {(UsersGrid.SelectedItem as DataRowView).Row.ItemArray[0]}");
                GridFill(UsersGrid, "SELECT * FROM Users");
            }
            else
                MessageBox.Show("Выберите пользователя!");
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
                ComboBoxFill(ClientsComboBox,"IdClient,FullName,Contact","Clients");
                ComboBoxFill(WorkerComboBox, "IdWorker,FullNameWorker,Contact", "Workers");
                ComboBoxFill(ServicesComboBox, "IdService,Name,Price", "Services");

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

            if (ServeListBox.SelectedItem == null)
            {
                ClientsComboBox.Visibility = Visibility.Hidden;
                ServicesComboBox.Visibility= Visibility.Hidden;
                WorkerComboBox.Visibility= Visibility.Hidden;
                AddMakedServiceButton.Visibility= Visibility.Hidden;
                DeleteMakedServiceButton.Visibility= Visibility.Hidden;
            }
            else
            {
                string idCheque = db.SoloRequest($"SELECT IdCheque FROM Cheque WHERE IdSession = {(ServeListBox.SelectedItem as ListBoxItem).Tag}");
                if (idCheque != null)
                {
                    ClientsComboBox.Visibility = Visibility.Hidden;
                    ServicesComboBox.Visibility= Visibility.Hidden;
                    WorkerComboBox.Visibility= Visibility.Hidden;
                    AddMakedServiceButton.Visibility= Visibility.Hidden;
                    DeleteMakedServiceButton.Visibility= Visibility.Hidden;
                }
                else
                {
                    ClientsComboBox.Visibility = Visibility.Visible;
                    ServicesComboBox.Visibility= Visibility.Visible;
                    WorkerComboBox.Visibility= Visibility.Visible;
                    AddMakedServiceButton.Visibility= Visibility.Visible;
                    DeleteMakedServiceButton.Visibility= Visibility.Visible;
                }
            }
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
            if (ClientsComboBox.SelectedItem != null && ServicesComboBox.SelectedItem != null && ServeListBox.SelectedItem != null && WorkerComboBox.SelectedItem != null)
            {
                db.SoloRequest($"INSERT INTO MakedService(IdClient,IdService,IdWorker,IdSession) VALUES ({(ClientsComboBox.SelectedItem as ComboBoxItem).Tag},{(ServicesComboBox.SelectedItem as ComboBoxItem).Tag},{(WorkerComboBox.SelectedItem as ComboBoxItem).Tag},{(ServeListBox.SelectedItem as ListBoxItem).Tag})");
                GridFill(MakedServicesGrid, $"SELECT ms.IdMakedService,s.Name,s.Price,c.FullName,c.Contact,w.FullNameWorker FROM Sessions as ss JOIN MakedService as ms ON ms.IdSession = ss.IdSession JOIN Services as s ON s.IdService = ms.IdService JOIN Clients as c ON c.IdClient = ms.IdClient JOIN Workers as w ON w.IdWorker = ms.IdWorker WHERE ss.IdSession = {(ServeListBox.SelectedItem as ListBoxItem).Tag}; ");
            }
            else
                MessageBox.Show("Все поля обязательны к выбору!");
        }

        private void DeleteMakedServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (MakedServicesGrid.SelectedItem != null)
            {
                db.SoloRequest($"DELETE FROM MakedService WHERE IdMakedService = {(MakedServicesGrid.SelectedItem as DataRowView).Row.ItemArray[0]}"); GridFill(MakedServicesGrid, $"SELECT ms.IdMakedService,s.Name,s.Price,c.FullName,c.Contact,w.FullNameWorker FROM Sessions as ss JOIN MakedService as ms ON ms.IdSession = ss.IdSession JOIN Services as s ON s.IdService = ms.IdService JOIN Clients as c ON c.IdClient = ms.IdClient JOIN Workers as w ON w.IdWorker = ms.IdWorker WHERE ss.IdSession = {(ServeListBox.SelectedItem as ListBoxItem).Tag}; ");
                GridFill(MakedServicesGrid, $"SELECT ms.IdMakedService,s.Name,s.Price,c.FullName,c.Contact,w.FullNameWorker FROM Sessions as ss JOIN MakedService as ms ON ms.IdSession = ss.IdSession JOIN Services as s ON s.IdService = ms.IdService JOIN Clients as c ON c.IdClient = ms.IdClient JOIN Workers as w ON w.IdWorker = ms.IdWorker WHERE ss.IdSession = {(ServeListBox.SelectedItem as ListBoxItem).Tag}; ");
            }
            else
                MessageBox.Show("Выберите оказанную услугу!");
        }

        private void ChequeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServeListBox.SelectedItem != null)
            {
                string idCheque = db.SoloRequest($"SELECT IdCheque FROM Cheque WHERE IdSession = {(ServeListBox.SelectedItem as ListBoxItem).Tag}");
                if (idCheque != null)
                {
                    OpenCheck((ServeListBox.SelectedItem as ListBoxItem).Tag.ToString());
                }
                else
                {
                    db.SoloRequest($"INSERT INTO Cheque (IdSession, FinalSum, DateCheque) VALUES ({(ServeListBox.SelectedItem as ListBoxItem).Tag},(SELECT SUM(Price) FROM Sessions AS ss JOIN MakedService AS m ON m.IdSession = ss.IdSession JOIN Services AS s ON s.IdService = m.IdService WHERE ss.IdSession = {(ServeListBox.SelectedItem as ListBoxItem).Tag}),'{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss")}')");
                    ClientsComboBox.Visibility = Visibility.Hidden;
                    ServicesComboBox.Visibility= Visibility.Hidden;
                    WorkerComboBox.Visibility= Visibility.Hidden;
                    AddMakedServiceButton.Visibility= Visibility.Hidden;
                    DeleteMakedServiceButton.Visibility= Visibility.Hidden;

                    OpenCheck((ServeListBox.SelectedItem as ListBoxItem).Tag.ToString());
                }

            }
            else
                MessageBox.Show("Выберите сессию для формирования чека!");
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

        void ComboBoxFill(ComboBox box,string fields, string table)
        {
            box.Items.Clear();

            MySqlDataReader reader = db.MultyRequest($"SELECT {fields} FROM {table}");
            string[] field = fields.Split(new string[] { "," }, StringSplitOptions.None);
            while (reader.Read())
            {
                ComboBoxItem item = new ComboBoxItem()
                {
                    Content = $"{reader[field[1]]}\n{reader[field[2]]}",
                    Tag = reader[field[0]].ToString()
                };

                box.Items.Add(item);
            }

            reader.Close();
        }

        void OpenCheck(string IdSession)
        {
            string client = db.SoloRequest($"SELECT c.FullName FROM Clients as c JOIN MakedService as ms ON ms.IdClient = c.IdClient JOIN Sessions as s ON s.IdSession = ms.IdSession WHERE S.IdSession = {IdSession}");
            string worker = db.SoloRequest($"SELECT w.FullNameWorker FROM Workers as w JOIN MakedService as ms ON ms.IdWorker = w.IdWorker JOIN Sessions as s ON s.IdSession = ms.IdSession WHERE S.IdSession = {IdSession}");
            string price = db.SoloRequest($"SELECT FinalSum FROM Cheque WHERE IdSession = {IdSession}");
            string date = db.SoloRequest($"SELECT DateCheque FROM Cheque WHERE IdSession = {IdSession}");

            Cheque chequeForm = new Cheque(client,worker,price,date);
            chequeForm.ShowDialog();
        }
        #endregion

    }
}
