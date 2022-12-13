using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.Data;

namespace SpaLotos
{
    public class DBContext
    {
        MySqlConnection connection;
        public bool Status 
        { 
            get 
            {
                try
                {
                    connection.OpenAsync();
                    connection.CloseAsync();
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            } 
        }

        public DBContext()
        {
            try
            {
                connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString);
            } catch (Exception e) {}
        }

        public string SoloRequest(string request)
        {
            MySqlCommand command = new MySqlCommand(request, connection);
            string result = null;

            try
            {
                connection.OpenAsync();

                object response = command.ExecuteScalarAsync().Result;

                if (response != null)
                    result = response.ToString();

                connection.CloseAsync();

            } catch (Exception e)
            {
                MessageBox.Show($"{e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        public DataTable TableRequest(string request)
        {
            MySqlCommand command = new MySqlCommand(request, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();

            try
            {
                connection.OpenAsync();
                adapter.Fill(table);
                connection.CloseAsync();

            } catch(Exception e)
            {
                MessageBox.Show($"{e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return table;
        }

        public MySqlDataReader MultyRequest(string request)
        {
            return null;
        }
    }
}
