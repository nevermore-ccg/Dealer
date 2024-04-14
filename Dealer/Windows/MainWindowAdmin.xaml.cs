using Dealer.DataBaseFolder;
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

namespace Dealer.Окна
{
    /// <summary>
    /// Логика взаимодействия для MainWindowAdmin.xaml
    /// </summary>
    public partial class MainWindowAdmin : Window
    {
        User user = new User();

        public MainWindowAdmin(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
            NameLabel.Content = user.Surname + " " + user.FirstName;
        }

        private void BtnRegistration_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration(user);
            registration.Show();
            this.Close();
        }

        private void BtnDB_Click(object sender, RoutedEventArgs e)
        {
            DB db = new DB(user);
            db.Show();
            this.Close();
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            AddProducts products = new AddProducts(user);
            products.Show();
            this.Close();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            Orders orders = new Orders(user);
            orders.Show();
            this.Close();
        }
    }
}
