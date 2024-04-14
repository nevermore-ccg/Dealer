using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Pass.Password;
            User user = new User();
            ///Проверка наличия логина и пароля в строке таблицы Users, с помощью контекста базы данных
            user = DealerDBEntities.GetContext().Users.Where(p => p.Login == login && p.Password == password).FirstOrDefault();
            int userCount = DealerDBEntities.GetContext().Users.Where(p => p.Login == login && p.Password == password).Count();
            ///Если найдены совпадения, то пользователь заходит под своим аккаунтом с соответсвующей ему ролью в систему
            if (userCount > 0)
            {
                MessageBox.Show("Вы вошли под: " + user.Role.Role1.ToString());
                LoadForm(user.Role.Role1.ToString(), user);
            }
            else
            {
                Login.Clear();
                Pass.Clear();
                MessageBox.Show("Неверно введены данные", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
