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
using System.Data;
using System.Data.SqlClient;
using Dealer.Классы;

namespace Dealer.Окна
{
    /// <summary>
    /// Логика взаимодействия для DB.xaml
    /// </summary>
    public partial class DB : Window
    {
        User user = new User();
        DataBase dataBase = new DataBase();
        SqlDataAdapter adapter;
        DataTable dataTable;
        string table;

        public DB(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
            CmbTable.Items.Add(new ComboBoxItem() { Content = "Company" });
            CmbTable.Items.Add(new ComboBoxItem() { Content = "Order_ACTM" });
            CmbTable.Items.Add(new ComboBoxItem() { Content = "Production_ACTM" });
            CmbTable.Items.Add(new ComboBoxItem() { Content = "Role" });
            CmbTable.Items.Add(new ComboBoxItem() { Content = "Users" });
            CmbTable.Items.Add(new ComboBoxItem() { Content = "PickUpPoint" });
        }

        private void CmbTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataBase.OpenConnection();
            table = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
            string queryFillGrid = $"Select * From {table}";
            SqlCommand FillGrid = new SqlCommand(queryFillGrid, dataBase.GetConnection());
            adapter = new SqlDataAdapter(FillGrid);
            dataTable = new DataTable();
            adapter.Fill(dataTable);
            DBGrid.ItemsSource = dataTable.DefaultView;
            dataBase.CloseConnection();
        }

        private void UpdateDB()
        {
            try
            {
                SqlCommandBuilder commandbuilder = new SqlCommandBuilder(adapter);
                if (adapter == null)
                {
                    MessageBox.Show("Выберите таблицу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    adapter.Update(dataTable);
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Ошибка при обновлении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при обновлении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void Button_GoToPage(object sender, RoutedEventArgs e)
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DBGrid.ItemsSource == null || DBGrid.Items.Count == 0)
            {
                MessageBox.Show("Выберите таблицу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DBGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись в таблице", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить запись ?",
                "Осторожно", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (DBGrid.SelectedItems != null && result == MessageBoxResult.Yes)
            {
                try
                {
                    for (int i = 0; i < DBGrid.SelectedItems.Count; i++)
                    {
                        DataRowView datarowView = DBGrid.SelectedItems[i] as DataRowView;
                        if (datarowView != null)
                        {
                            DataRow dataRow = datarowView.Row;
                            dataRow.Delete();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                UpdateDB();
                MessageBox.Show("Запись успешно удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}