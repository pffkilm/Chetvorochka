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
    /// Логика взаимодействия для SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        private List<ProductForSale> productList;
        private List<CartItem> cart = new List<CartItem>();

        private Clients _currentClient;
        private decimal _skidka = 0;

        private decimal _total = 0;
        private decimal _skidkaSum = 0;
        private decimal _finalSum = 0;
        public SalesPage()
        {
            InitializeComponent();
            LoadProducts();
            SortList();
        }


        private void LoadProducts()
        {
            var products = Db.Content.Products
                .Where(p => p.IsDeleted == false && p.Stock > 0)
                .Select(p => new ProductForSale
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Barcode = p.Barcode,
                }).ToList();
            productList = products;
            gridProducts.ItemsSource = productList;
        }


        public void SortList()
        {
            List<ProductForSale> newList = new List<ProductForSale>(productList);

            if (!string.IsNullOrEmpty(searchText.Text))
            {
                string search = searchText.Text.ToLower();
                newList = newList.Where(x => x.Name.ToLower().Contains(search) || x.Barcode.ToLower() == searchText.Text).ToList();
            }
            gridProducts.ItemsSource = newList;

        }

        private void goBack_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void CheckShow(object sender, RoutedEventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            var payWindow = new PaymentWindow(_finalSum);

            if (payWindow.ShowDialog() == true) {
                SaveSale(payWindow.PaymentType, payWindow.Received, payWindow.Change);
            }



        }

        private void searchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortList();
        }

        private void gridProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var product = gridProducts.SelectedItem as ProductForSale;
            if (product == null) return;

            var item = cart.FirstOrDefault(c => c.ProductId == product.Id);

            if (item != null)
            {
                item.CountProduct++;
            }
            else
            {
                cart.Add(
                   new CartItem
                   {
                       ProductId = product.Id,
                       ProductName = product.Name,
                       Price = product.Price,
                       CountProduct = 1
                   });

            }


            UpdateCart();
        }
        private void UpdateCart()
        {
            korzinProduct.ItemsSource = null;
            korzinProduct.ItemsSource = cart;

            _total = 0;
            foreach (var item in cart)
            {
                _total += item.Price * item.CountProduct;
            }
            _skidkaSum = _total * _skidka / 100;
            _finalSum = _total - _skidkaSum;
            totalSum.Text = $"Итого: {_total} , Скидка: {_skidka}%,  Всего к оплате: {_finalSum}";
        }
        private void AllDellit(object sender, RoutedEventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }
            var result = MessageBox.Show("Очистить корзину?", "Подтверждение",
         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                cart.Clear();
                UpdateCart();
            }


        }

        private void Delit(object sender, RoutedEventArgs e)
        {
            var productDel = korzinProduct.SelectedItem as CartItem;

            if (productDel == null)
            {
                MessageBox.Show("Выберите товар для удаления.");
                return;
            }
            else
            {
                cart.Remove(productDel);
            }
            UpdateCart();
        }

        private void SearchClient_Click(object sender, RoutedEventArgs e)
        {

            string phone = PhoneTb.Text;
            var client = Db.Content.Clients.FirstOrDefault(c => c.PhoneNumber == phone);

            if (client != null)
            {
                _currentClient = client;
                _skidka = client.Discount ?? 5;

                NameClient.Text = $"{client.LastName} {client.FirstName}";
                Skidka.Text = $"{_skidka}";
                MessageBox.Show($"Клиент найден! Скидка: {_skidka}%");
            }
            else
            {
                _currentClient = null;
                _skidka = 0;
                NameClient.Text = "Клиент не зайден";
                Skidka.Text = "";
                MessageBox.Show("Клиент не найден. Нажмите 'Зарегистрироваться'");
            }
            UpdateCart();
        }

        private void regist_Click(object sender, RoutedEventArgs e)
        {


            string phone = PhoneTb.Text;
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Сначала введите телефон");
                return;
            }
            if (phone.Length != 11)
            {
                MessageBox.Show("Введите 11 цифр телефона");
                return;
            }
            if (phone[0] != '7' && phone[0] != '8')
            {
                MessageBox.Show("Телефон должен начинаться с 7 или 8");
                return;
            }

            foreach (char c in phone)
            {
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("Телефон должен содержать только цифры");
                    return;
                }
            }


            var regWindow = new RegisterClientWindow(phone);
            regWindow.ShowDialog();

            var client = Db.Content.Clients.FirstOrDefault(x => x.PhoneNumber == phone);


            if (client != null)
            {
                _currentClient = client;
                _skidka = 5;
                NameClient.Text = $"{client.LastName} {client.FirstName}";
                Skidka.Text = $"{_skidka}";
                UpdateCart();
            }
            else
            {
                MessageBox.Show("Регистрация не завершена");
            }
        }

        private void plus_Click(object sender, RoutedEventArgs e)
        {
            var item = korzinProduct.SelectedItem as CartItem;
            if (item == null) { MessageBox.Show("Выберите товар в корзине"); return; }

            item.CountProduct++;
            UpdateCart();

        }

        private void minus_Click(object sender, RoutedEventArgs e)
        {
            var item = korzinProduct.SelectedItem as CartItem;
            if (item == null) { MessageBox.Show("Выберите товар в корзине"); return; }

            item.CountProduct--;
            if (item.CountProduct == 0)
            {
                cart.Remove(item);
            }

            UpdateCart();
        }



        private void SaveSale(string paymentType, decimal received, decimal change)
        {

            var sale = new Sales
            {
                SaleDate = DateTime.Now,
                UserId = App.CurrentUser.Id,
                ClientId = _currentClient?.Id,
                PaymentType = paymentType,
                TotalDiscount = _skidkaSum,
                TotalAmount = _finalSum
            };
            Db.Content.Sales.Add(sale);
            Db.Content.SaveChanges();


            foreach (var item in cart)
            {
                var saleItem = new SaleItems
                {
                    SaleId = sale.Id,
                    ProductId = item.ProductId,
                    Quantity = item.CountProduct,
                    PriceAtSale = item.Price
                };
                Db.Content.SaleItems.Add(saleItem);

                var product = Db.Content.Products.First(p => p.Id == item.ProductId);
                product.Stock -= item.CountProduct;
            }
            Db.Content.SaveChanges();


            ShowCheks(paymentType, received, change);


            cart.Clear();
            _currentClient = null;
            _skidka = 0;
            NameClient.Text = "";
            Skidka.Text = "";
            PhoneTb.Text = "";
            UpdateCart();
            LoadProducts();
            SortList();
        }

        private void ShowCheks(string paymentType, decimal received, decimal change)
        {
            string check = "";  

            check += $"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}\n";

            if (App.CurrentUser != null)
            {
                check += $"Кассир: {App.CurrentUser.FullName}\n";
            }

            if (_currentClient != null)
            {
                check += $"Клиент: {_currentClient.LastName} {_currentClient.FirstName}\n";
                check += $"Скидка: {_skidka}%\n";
            }


            foreach (var item in cart)
            {
                check += $"{item.ProductName}";
                check += $"  {item.CountProduct} x {item.Price:F2} = {item.Price * item.CountProduct:F2}\n";
            }

            check += $"Сумма без скидки: {_total}\n";
            check += $"Скидка: {_skidkaSum}\n";
            check += $"ИТОГО: {_finalSum}\n";
            check += $"Способ оплаты: {paymentType}\n";

            if (paymentType == "Наличные")
            {
                check += $"Получено: {received}\n";
                check += $"Сдача: {change}\n";
            }

            check += "Спасибо за покупку!";
            checkTb.Text = check;

        }
    }
}
