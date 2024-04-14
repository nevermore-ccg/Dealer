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
using Dealer.Классы;
using System.Data;
using System.Data.SqlClient;
using Dealer.DataBaseFolder;

namespace Dealer.Окна
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        DataBase dataBase = new DataBase();
        User user = new User();

        public Registration(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
            var companies = DealerDBEntities.GetContext().Companies.ToList();
            CmbCompany.ItemsSource = companies;
        }

        #region Ограничение на введение символов в строку
        private void Digit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void Letters_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        #endregion

        private void Button_Registration(object sender, RoutedEventArgs e)
        {
            int role;
            if (CmbCompany.SelectedIndex == 0)
            {
                role = 1;
            }
            else
            {
                role = 2;
            }
            if (Checkuser())
            {
                return;
            }
            User newUser = new User()
            {
                Surname = Last_Name.Text,
                FirstName = First_Name.Text,
                Login = Login.Text,
                Password = Password.Text,
                Phone = Phone.Text,
                Company_Id = CmbCompany.SelectedIndex + 1,
                Role_Id = role,
            };
            DealerDBEntities.GetContext().Users.Add(newUser);
            DealerDBEntities.GetContext().SaveChanges();
            MessageBox.Show("Пользователь зарегистрирован!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            First_Name.Clear();
            Last_Name.Clear();
            Phone.Clear();
            CmbCompany.SelectedIndex = -1;
            Login.Clear();
            Password.Clear();
        }

        private Boolean Checkuser()
        {
            #region переменные
            string firstName = First_Name.Text;
            string lastName = Last_Name.Text;
            string phone = Phone.Text;
            int company = CmbCompany.SelectedIndex + 1;
            string login = Login.Text;
            string password = Password.Text;
            #endregion
            string query = $"select * from Users where  Login = '{login}'";
            SqlCommand cmd = new SqlCommand(query, dataBase.GetConnection());
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();
            adapter.SelectCommand = cmd;
            adapter.Fill(table);
            #region Проверка полей на данные 
            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Такой клиент уже есть в базе данных", "Ошибка регистрации", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return true;
            }
            if (phone == "" || password == "" || firstName == "" || lastName == "" ||
                login == "" || company == 0)
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка регистрации", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return true;
            }
            else
            {
                return false;
            }
            #endregion
        }

        private void GoToPage_Click(object sender, RoutedEventArgs e)
        {
            LoadForm(user.Role.Role1.ToString(), user);
        }

        private void LoadForm(string role, User user)
        {
            switch (role)
            {
                case "Пользователь":
                    MainWindowUser windowUser = new MainWindowUser(user);
                    windowUser.Show();
                    this.Close();
                    break;
                case "Администратор":
                    MainWindowAdmin windowAdmin = new MainWindowAdmin(user);
                    windowAdmin.Show();
                    this.Close();
                    break;
            }
        }
    }
}
