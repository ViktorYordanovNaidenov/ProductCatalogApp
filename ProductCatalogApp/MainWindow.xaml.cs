using System;
using System.Windows;
using System.Windows.Controls;

namespace ProductCatalogApp
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _dbService;

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService(); // Базата се създава при стартиране[cite: 4]
            LoadProducts();
        }

        private void LoadProducts(string searchTerm = "")
        {
            try
            {
                gridProducts.ItemsSource = _dbService.GetProducts(searchTerm);
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Грешка при зареждане: " + ex.Message;
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts(txtSearch.Text); // Извиква LoadProducts с критерий за филтриране[cite: 3]
            txtStatus.Text = "Резултатите са филтрирани.";
        }

        private void GridProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Показва данните в полетата за редактиране при избор на ред[cite: 3]
            if (gridProducts.SelectedItem is Product selectedProduct)
            {
                txtName.Text = selectedProduct.Name;
                txtPrice.Text = selectedProduct.Price.ToString();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Валидация на празно име и невалидна цена[cite: 4]
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtStatus.Text = "Грешка: Въведете име на продукт.";
                return;
            }
            if (!double.TryParse(txtPrice.Text, out double price))
            {
                txtStatus.Text = "Грешка: Невалидна цена.";
                return;
            }

            try
            {
                _dbService.AddProduct(txtName.Text, price);
                txtStatus.Text = "Продуктът е добавен успешно.";
                ClearInputs();
                LoadProducts();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Грешка: " + ex.Message;
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (gridProducts.SelectedItem is not Product selectedProduct)
            {
                txtStatus.Text = "Грешка: Изберете продукт за редакция.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text) || !double.TryParse(txtPrice.Text, out double price))
            {
                txtStatus.Text = "Грешка: Невалидни данни за редакция.";
                return;
            }

            try
            {
                _dbService.UpdateProduct(selectedProduct.Id, txtName.Text, price);
                txtStatus.Text = "Продуктът е обновен успешно.";
                LoadProducts(); // Презареждане на списъка след редакция[cite: 3]
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Грешка: " + ex.Message;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Изтриване без избран ред показва съобщение[cite: 4]
            if (gridProducts.SelectedItem is not Product selectedProduct)
            {
                txtStatus.Text = "Грешка: Изберете продукт за изтриване.";
                return;
            }

            try
            {
                _dbService.DeleteProduct(selectedProduct.Id);
                txtStatus.Text = "Продуктът е изтрит.";
                ClearInputs();
                LoadProducts();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Грешка: " + ex.Message;
            }
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtPrice.Clear();
            gridProducts.SelectedItem = null;
        }
    }
}