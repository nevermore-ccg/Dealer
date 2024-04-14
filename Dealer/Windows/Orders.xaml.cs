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
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : Window
    {
        DataBase dataBase = new DataBase();
        SqlDataAdapter adapter;
        DataTable table;
        User user = new User();
        string statusTable;
        string statusChange = "";
        static string queryFill = $"Select Order_ACTM.Id, Order_ACTM.Status As 'Статус' , Production_ACTM.Product_Name As 'Продукция', " +
                    $"Order_ACTM.Quantity As 'Количество', Order_ACTM.Order_Price As 'Цена заказа', Order_ACTM.Delivery_Price As 'Цена доставки', " +
                    $"Order_ACTM.Order_sum As 'Общая цена', PickUpPoint.Address As 'Склад', Order_ACTM.CreateAt As 'Дата создания заказа' From Order_ACTM " +
                    $"Inner Join Production_ACTM On Order_ACTM.ACTM_Id = Production_ACTM.Id " +
                    $"Inner Join Company On Order_ACTM.Company_Id = Company.Id " +
                    $"Inner Join PickUpPoint On PickUpPoint_Id = PickUpPoint.Id";

        public Orders(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
            CmbChangeStatus.Items.Add(new ComboBoxItem() { Content = "Новый заказ" });
            CmbChangeStatus.Items.Add(new ComboBoxItem() { Content = "Обрабатывается" });
            CmbChangeStatus.Items.Add(new ComboBoxItem() { Content = "В доставке" });
            CmbChangeStatus.Items.Add(new ComboBoxItem() { Content = "Выполнен" });
            CmbChangeStatus.Items.Add(new ComboBoxItem() { Content = "Отменен" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Все заказы" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Новый заказ" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Обрабатывается" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "В доставке" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Выполнен" });
            CmbStatusTable.Items.Add(new ComboBoxItem() { Content = "Отменен" });
        }

        private void GoToPage_Click(object sender, RoutedEventArgs e)
        {
            MainWindowAdmin windowAdmin = new MainWindowAdmin(user);
            windowAdmin.Show();
            this.Close();
        }

        private void CmbStatusTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataBase.OpenConnection();
            statusTable = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
            if (statusTable == "Все заказы")
            {
                FillGrid(queryFill);
            }
            else
            {
                FillGrid(queryFill + $" Where Order_ACTM.Status = '{statusTable}'");
            }
            dataBase.CloseConnection();
        }

        private void FillGrid(string query)
        {
            SqlCommand FillGrid = new SqlCommand(query, dataBase.GetConnection());
            adapter = new SqlDataAdapter(FillGrid);
            table = new DataTable();
            adapter.Fill(table);
            DBGrid.ItemsSource = table.DefaultView;
        }

        private void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            dataBase.OpenConnection();
            DataRowView rowView = DBGrid.SelectedItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ", "Ошибка изменения статуса", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (statusChange == "")
            {
                MessageBox.Show("Пожалуйста, выберите статус", "Ошибка изменения статуса", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            else
            {
                DataRow row = rowView.Row;
                int selectedId = Convert.ToInt32(row["Id"]);
                string queryUpdate = $"Update Order_ACTM Set Status = '{statusChange}' Where Id = '{selectedId}'";
                FillGrid(queryUpdate);
            }
            if (statusTable == "Все заказы")
            {
                FillGrid(queryFill);
            }
            else
            {
                FillGrid(queryFill + $" Where Order_ACTM.Status = '{statusTable}'");
            }
            dataBase.CloseConnection();
        }

        private void CmbChangeStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            statusChange = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
        }

        private void BtnAddDelivery_Click(object sender, RoutedEventArgs e)
        {
            dataBase.OpenConnection();
            DataRowView rowView = DBGrid.SelectedItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ", "Ошибка изменения цены доставки", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DeliveryPriceBox.Text == "")
            {
                MessageBox.Show("Пожалуйста, введите цену доставки", "Ошибка изменения цены доставки", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                int price = Convert.ToInt32(DeliveryPriceBox.Text);
                DataRow row = rowView.Row;
                int sum = price + Convert.ToInt32(row["Цена заказа"]);
                int selectedId = Convert.ToInt32(row["Id"]);
                string queryUpdate = $"Update Order_ACTM Set Delivery_Price = '{price}', Order_sum = '{sum}' Where Id = '{selectedId}'";
                FillGrid(queryUpdate);
            }
            if (statusTable == "Все заказы")
            {
                FillGrid(queryFill);
            }
            else
            {
                FillGrid(queryFill + $" Where Order_ACTM.Status = '{statusTable}'");
            }
            dataBase.CloseConnection();
        }
    }
}
