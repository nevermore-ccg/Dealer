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
using System.IO;
using Dealer.Классы;
using Dealer.DataBaseFolder;

namespace Dealer.Окна
{
    /// <summary>
    /// Логика взаимодействия для MainWindowUser.xaml
    /// </summary>
    public partial class MainWindowUser : Window
    {
        DataBase dataBase = new DataBase();
        SqlDataAdapter adapter;
        DataTable table;
        User user = new User();
        static string imagePath = System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,
            @"Resources\Pictures\back_pic.png");

        public MainWindowUser(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
            dataBase.OpenConnection();
            string queryFillGrid = $"Select Id, Status As 'Статус', Product_Name As 'Название', Price As 'Цена', Operating_frequency_range As 'Диапазон рабочих частот', " +
                $"Information_transfer_rate_KBIT_S As 'Скорость передачи информации (Кбит/С)', Number_of_programmable_channels As 'Количество программируемых каналов'," +
                $" Output_power As 'Выходная мощь(пиковая/средняя)', Power As 'Питание', Operating_conditions As 'Условия эксплуатации'," +
                $" Overall_dimensions_of_the_components As 'Габаритные размеры (ШxГxВ) и масса составных частей' From Production_ACTM";
            SqlCommand FillGrid = new SqlCommand(queryFillGrid, dataBase.GetConnection());
            adapter = new SqlDataAdapter(FillGrid);
            table = new DataTable();
            adapter.Fill(table);
            DBGrid.ItemsSource = table.DefaultView;
            #region Заполнение CmbPickUpPoint данными из таблицы
            string query = $"Select Address From PickUpPoint Where Company_Id = {currentUser.Company_Id}";
            SqlCommand command = new SqlCommand(query, dataBase.GetConnection());
            command.ExecuteNonQuery();
            SqlDataAdapter adapterPickupPoints = new SqlDataAdapter(command);
            DataTable tablePickupPoints = new DataTable();
            adapterPickupPoints.Fill(tablePickupPoints);
            foreach (DataRow dataRow in tablePickupPoints.Rows)
            {
                CmbPickUpPoint.Items.Add(dataRow["Address"].ToString());
            }
            #endregion
            dataBase.CloseConnection();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Установка изображения при загрузке окна
            DisplaySelectedImage(imagePath);
        }

        private void DisplaySelectedImage(string imagePath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();

                Picture.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message);
            }
        }

        private void DisplaySelectedImageBytes(byte[] imageBytes)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.EndInit();
                Picture.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message);
            }
        }

        private void DBGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView rowView = DBGrid.SelectedItem as DataRowView;
            if (rowView == null)
            {
                return;
            }
            DataRow row = rowView.Row;
            if (row["Id"] == DBNull.Value)
            {
                return;
            }
            else
            {
                if (rowView != null)
                {
                    int selectedId = Convert.ToInt32(row["Id"]);
                    string query = $"Select Product_Image From Production_ACTM Where Id = {selectedId}";
                    SqlCommand command = new SqlCommand(query, dataBase.GetConnection());
                    dataBase.OpenConnection();
                    try
                    {
                        byte[] imageBytes = (byte[])command.ExecuteScalar();
                        DisplaySelectedImageBytes(imageBytes);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message);
                    }
                    finally
                    {
                        dataBase.CloseConnection();
                    }
                }
            }
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            int pickUpPoint = CmbPickUpPoint.SelectedIndex + 1;
            DataRowView rowView = DBGrid.SelectedItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("Пожалуйста, выберите продукцию", "Ошибка формирования заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DataRow row = rowView.Row;
            if (row["Id"] == DBNull.Value)
            {
                MessageBox.Show("Пожалуйста, выберите продукцию", "Ошибка формирования заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (pickUpPoint == 0)
            {
                MessageBox.Show("Пожалуйста, выберите склад", "Ошибка формирования заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Quantity.Text == "")
            {
                MessageBox.Show("Пожалуйста, выберите количество заказываемой продукции", "Ошибка формирования заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Convert.ToInt32(Quantity.Text) == 0)
            {
                MessageBox.Show("Количество заказываемой продуции не может быть равно 0", "Ошибка формирования заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (rowView != null && pickUpPoint != 0 && Quantity.Text != "" && Convert.ToInt32(Quantity.Text) != 0)
            {
                int selectedId = Convert.ToInt32(row["Id"]);
                int orderPrice = Convert.ToInt32(row["Цена"]) * Convert.ToInt32(Quantity.Text);
                Order_ACTM order = new Order_ACTM()
                {
                    ACTM_Id = selectedId,
                    Company_Id = user.Company_Id,
                    PickUpPoint_Id = pickUpPoint,
                    Status = "Новый заказ",
                    CreateAt = DateTime.Now,
                    Quantity = Convert.ToInt32(Quantity.Text),
                    Order_Price = orderPrice,
                };
                DealerDBEntities.GetContext().Order_ACTM.Add(order);
                DealerDBEntities.GetContext().SaveChanges();
                MessageBox.Show("Заказ добавлен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                Quantity.Clear();
                CmbPickUpPoint.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка заказа", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Digit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                table.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                string searchText = SearchTextBox.Text.ToLower();
                // фильтрация по всем столбцам
                string filter = $"[Статус] LIKE '%{searchText}%' OR " +
                                $"[Название] LIKE '%{searchText}%' OR " +
                                $"[Диапазон рабочих частот] LIKE '%{searchText}%' OR " +
                                $"[Скорость передачи информации (Кбит/С)] LIKE '%{searchText}%' OR " +
                                $"[Количество программируемых каналов] LIKE '%{searchText}%' OR " +
                                $"[Выходная мощь(пиковая/средняя)] LIKE '%{searchText}%' OR " +
                                $"[Питание] LIKE '%{searchText}%' OR " +
                                $"[Условия эксплуатации] LIKE '%{searchText}%' OR " +
                                $"[Габаритные размеры (ШxГxВ) и масса составных частей] LIKE '%{searchText}%'";
                DataView dataView = table.DefaultView;
                dataView.RowFilter = filter;
                DBGrid.ItemsSource = dataView;
            }
        }

        private void BtnAllOrders_Click(object sender, RoutedEventArgs e)
        {
            AllOrders allOrders = new AllOrders(user);
            allOrders.Show();
            this.Close();
        }
    }
}
