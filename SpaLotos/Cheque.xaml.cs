﻿using System;
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
    public partial class Cheque : Window
    {
        public Cheque(string client, string worker, string price, string date)
        {
            InitializeComponent();

            ClientBlock.Text = client;
            WorkerBlock.Text = worker;
            SmallPriceBlock.Text = price;
            BigPriceBlock.Text = price;
            DateBlock.Text = date;
            Title = $"Чек от {date}";
        }
    }
}
