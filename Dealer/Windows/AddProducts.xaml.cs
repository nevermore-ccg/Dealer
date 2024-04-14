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
using Microsoft.Win32;
using Dealer.Классы;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Dealer.Окна
{
    /// <summary>
    /// Логика взаимодействия для AddProducts.xaml
    /// </summary>
    public partial class AddProducts : Window
    {
        User user = new User();
        static string ImagePath = System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,
            @"Resources\Pictures\back_pic.png");

        public AddProducts(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Установка изображения при загрузке окна
            DisplaySelectedImage(ImagePath);
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

        private void BtnEnterImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                DisplaySelectedImage(selectedImagePath);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Picture.Source == null)
                {
                    MessageBox.Show("Пожалуйста, выберите изображение");
                    return;
                }
                #region объявление переменных из TextBox 
                string name = Product_Name.Text;
                string ofr1 = OFRBox.Text;
                string itr1 = ITRBox.Text;
                string npc1 = NPCBox.Text;
                string outputPower = Output_powerBox.Text;
                string power = PowerBox.Text;
                string operatingConditions = Operating_conditions.Text;
                string overallDimensions = Overall_dimensions.Text;
                string status = "Активен";
                int price = Convert.ToInt32(PriceBox.Text);
                #endregion
                if (name == "" || ofr1 == "" || itr1 == "" || npc1 == "" || outputPower == "" || power == ""
                    || operatingConditions == "" || overallDimensions == "" || PriceBox.Text == "" || price == 0)
                {
                    MessageBox.Show("Пожалуйста, заполните все характеристики продукта");
                    return;
                }
                // Преобразуем изображение в массив байтов
                byte[] imageBytes = null;
                BitmapSource bitmapSource = (BitmapSource)Picture.Source;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                Production_ACTM production_ACTM = new Production_ACTM()
                {
                    Status = status,
                    Product_Image = imageBytes,
                    Product_Name = name,
                    Operating_frequency_range = ofr1,
                    Information_transfer_rate_KBIT_S = itr1,
                    Number_of_programmable_channels = npc1,
                    Output_power = outputPower,
                    Power = power,
                    Operating_conditions = operatingConditions,
                    Overall_dimensions_of_the_components = overallDimensions,
                    Price = price,
                };
                DealerDBEntities.GetContext().Production_ACTM.Add(production_ACTM);
                DealerDBEntities.GetContext().SaveChanges();
                MessageBox.Show("Товар добавлен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                #region очищение TextBox после успешного добавления товара
                Overall_dimensions.Clear();
                Operating_conditions.Clear();
                PowerBox.Clear();
                Output_powerBox.Clear();
                NPCBox.Clear();
                ITRBox.Clear();
                OFRBox.Clear();
                Product_Name.Clear();
                PriceBox.Clear();
                #endregion
                DisplaySelectedImage(ImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении записи: " + ex.Message);
            }
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
