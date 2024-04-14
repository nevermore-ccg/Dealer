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
using Dealer.DataBaseFolder;


namespace Dealer.Окна
{
    /// <summary>
    /// Логика взаимодействия для AllOrders.xaml
    /// </summary>
    public partial class AllOrders : Window
    {
        DataBase dataBase = new DataBase();
        SqlDataAdapter adapter;
        DataTable table;
        static User user = new User();
        string statusTable;
        static string queryFillGrid = $"Select Order_ACTM.Id, Order_ACTM.Status As 'Статус' , Production_ACTM.Product_Name As 'Продукция', " +
                    $"Order_ACTM.Quantity As 'Количество', Order_ACTM.Order_Price As 'Цена заказа', Order_ACTM.Delivery_Price As 'Цена доставки', " +
                    $"Order_ACTM.Order_sum As 'Общая цена', PickUpPoint.Address As 'Склад', Order_ACTM.CreateAt As 'Дата создания заказа' From Order_ACTM " +
                    $"Inner Join Production_ACTM On Order_ACTM.ACTM_Id = Production_ACTM.Id " +
                    $"Inner Join Company On Order_ACTM.Company_Id = Company.Id " +
                    $"Inner Join PickUpPoint On PickUpPoint_Id = PickUpPoint.Id";

        public AllOrders(User currentUser)
        {
            InitializeComponent();
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Все заказы" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Новый заказ" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Обрабатывается" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "В доставке" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Выполнен" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Отменен" });
            user = currentUser;
        }

        private void GoToPage_Click(object sender, RoutedEventArgs e)
        {
            MainWindowUser windowUser = new MainWindowUser(user);
            windowUser.Show();
            this.Close();
        }

        private void CmbStatusTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataBase.OpenConnection();
            statusTable = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
            if (statusTable == "Все заказы")
            {
                FillGrid(queryFillGrid + $" Where Order_ACTM.Company_Id = '{user.Company_Id}'");
            }
            else
            {
                FillGrid(queryFillGrid + $" Where Order_ACTM.Company_Id = '{user.Company_Id}' And Order_ACTM.Status = '{statusTable}'");
            }
            dataBase.CloseConnection();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            dataBase.OpenConnection();
            DataRowView rowView = DBGrid.SelectedItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ", "Ошибка удаления/отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить/отменить запись ?",
                "Осторожно", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRow row = rowView.Row;
                int selectedId = Convert.ToInt32(row["Id"]);
                string SelectedStatus = Convert.ToString(row["Статус"]);
                if (SelectedStatus == "Новый заказ")
                {
                    string queryDelete = $"Delete From Order_ACTM Where Id = '{selectedId}'";
                    FillGrid(queryDelete);
                    MessageBox.Show("Заказ успешно удален!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (SelectedStatus == "Обрабатывается")
                {
                    string queryUpdate = $"Update Order_ACTM Set Status = 'Отменен' Where Id = '{selectedId}'";
                    FillGrid(queryUpdate);
                    MessageBox.Show("Заказ успешно отменен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (SelectedStatus == "В доставке")
                {
                    MessageBox.Show("Вы не можете отменить заказ, который уже в доставке", "Ошибка удаления/отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (SelectedStatus == "Выполнен")
                {
                    MessageBox.Show("Вы не можете отменить заказ, который уже выполнен", "Ошибка удаления/отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (SelectedStatus == "Отменен")
                {
                    MessageBox.Show("Этот заказ уже отменен", "Ошибка удаления/отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (statusTable == "Все заказы")
            {
                FillGrid(queryFillGrid + $" Where Order_ACTM.Company_Id = '{user.Company_Id}'");
            }
            else
            {
                FillGrid(queryFillGrid + $" Where Order_ACTM.Company_Id = '{user.Company_Id}' And Order_ACTM.Status = '{statusTable}'");
            }
            dataBase.CloseConnection();
        }

        private void FillGrid(string query)
        {
            SqlCommand fillCommand = new SqlCommand(query, dataBase.GetConnection());
            adapter = new SqlDataAdapter(fillCommand);
            table = new DataTable();
            adapter.Fill(table);
            DBGrid.ItemsSource = table.DefaultView;
        }
    }
}
