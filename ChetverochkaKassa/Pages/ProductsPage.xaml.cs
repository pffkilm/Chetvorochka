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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChetverochkaKassa.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private List<ProductForSale> ProductsList { get; set; }
        public ProductsPage()
        {
            InitializeComponent();
            LoadProducts();

        }

        private void goBack_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadProducts()
        {
            var product = Db.Content.Products.Select(
                p => new ProductForSale
                {

                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Barcode = p.Barcode,
                    CategoryName = p.Categories.Name,
                }

                ).ToList();
            ProductsList = product;
            productDg.ItemsSource = product;

            //фильтр
            var categories = Db.Content.Categories.ToList();
            categoryCombo.DisplayMemberPath = "Name";
            categoryCombo.ItemsSource = categories;

            //для добавления
            addCategory.DisplayMemberPath = "Name";
            addCategory.ItemsSource = categories;
            if (categories.Count > 0)
            {
                addCategory.SelectedIndex = 0;
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTb.Text) ||
                string.IsNullOrWhiteSpace(priceTb.Text) ||
                string.IsNullOrWhiteSpace(barcodeTb.Text) ||
                string.IsNullOrWhiteSpace(stockTb.Text))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            var category = addCategory.SelectedItem as Categories;
            if (category == null)
            {
                MessageBox.Show("Выберите категорию");
                return;
            }

            if (Db.Content.Products.Any(p => p.Barcode == barcodeTb.Text))
            {
                MessageBox.Show("Штрихкод уже существует");
                return;
            }


            decimal price;
            if (!decimal.TryParse(priceTb.Text, out price))
            {
                MessageBox.Show("Цена - не число");
                return;
            }

            int stock;
            if (!int.TryParse(stockTb.Text, out stock))
            {
                MessageBox.Show("Остаток должег быть целым числом");
            }

            var product = new Products
            {
                Name = nameTb.Text,
                CategoryId = category.Id,
                Price = price,
                Barcode = barcodeTb.Text,
                Stock = stock,
                IsDeleted = false

            };

            try
            {
                Db.Content.Products.Add(product);
                Db.Content.SaveChanges();

                MessageBox.Show("Товар добавлен");

                LoadProducts();

                nameTb.Text = "";
                priceTb.Text = "";
                barcodeTb.Text = "";
                stockTb.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}");
            }

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selected = productDg.SelectedItem as ProductForSale;

            if(selected == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }
            var product = Db.Content.Products.FirstOrDefault(p => p.Id == selected.Id);

            if(product == null)
            {
                MessageBox.Show("Товар не найден");
                return;
            }

            if(product.IsDeleted == true)
            {
                MessageBox.Show("Товар уже снять с продаж");
                return;
            }

            var result = MessageBox.Show($"Снят с продажи:{product.Name}? ", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            product.IsDeleted = true;
            Db.Content.SaveChanges();
            LoadProducts();

        
        }

        private void searchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortList();
        }

        private void SortList()
        {
            List<ProductForSale> newList = new List<ProductForSale>(ProductsList);
            if (!string.IsNullOrEmpty(searchProduct.Text))
            {
                string search = searchProduct.Text.ToLower();
                newList = newList.Where(x => x.Name.ToLower().Contains(search)).ToList();
            }
            productDg.ItemsSource = newList;


        }

        private void categoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var category = categoryCombo.SelectedItem as Categories;

            if (category == null)
            {
                productDg.ItemsSource = Db.Content.Products.ToList();
                return;
            }
            productDg.ItemsSource = ProductsList
       .Where(x => x.CategoryName == category.Name)
       .ToList();


        }





        private void saveProduct_Click(object sender, RoutedEventArgs e)
        {
            productDg.CommitEdit(DataGridEditingUnit.Cell, true);
            productDg.CommitEdit(DataGridEditingUnit.Row, true);

            var item = productDg.SelectedItem as ProductForSale;

            if (item == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            if (item.Price < 0 || item.Stock < 0)
            {
                MessageBox.Show("Цена и остаток не могут быть отрицательными");
                return;
            }

            var product = Db.Content.Products
                .FirstOrDefault(x => x.Id == item.Id);

            if (product == null)
            {
                MessageBox.Show("Товар не найден");
                return;
            }

            product.Name = item.Name;
            product.Price = item.Price;
            product.Stock = item.Stock;

            Db.Content.SaveChanges();

            MessageBox.Show("Сохранено");
            LoadProducts();
        }
    }
}
