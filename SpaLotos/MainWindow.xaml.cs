using System.Windows;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Windows.Media.Animation;
using System;
using System.Windows.Media.Effects;

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
            if (!db.ConnectionStatus())
            {

            }
        }
    }
}
