using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SpaLotos
{
    public class DBContext
    {
        MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString);

        public bool ConnectionStatus()
        {
            try
            {
                connection.Open();
                connection.Close();
            } catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
