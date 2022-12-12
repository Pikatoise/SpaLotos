using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;

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
                    connection.Open();
                    connection.Close();
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
                connection.Open();

                object response = command.ExecuteScalarAsync().Result;

                if (response != null)
                    result = response.ToString();

                connection.Close();

            } catch (Exception e)
            {
                MessageBox.Show($"{e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        public MySqlDataReader MultyRequest()
        {
            return null;
        }
    }
}
