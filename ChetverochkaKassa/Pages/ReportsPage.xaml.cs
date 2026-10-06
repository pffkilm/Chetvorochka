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
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            startDate.SelectedDate = DateTime.Now;
            endDate.SelectedDate = DateTime.Now;
        }

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void otchet_Click(object sender, RoutedEventArgs e)
        {
            var start = startDate.SelectedDate ?? DateTime.Now;
            var end = endDate.SelectedDate ?? DateTime.Now;

            var sale = Db.Content.Sales
                .Where(s => (s.SaleDate >= start) && (s.SaleDate < end)).ToList();


            int count = sale.Count;
            decimal mouney = sale.Sum(a => a.TotalAmount);
            decimal skidka = sale.Sum(a => a.TotalDiscount ?? 0);

            infoTb.Text = $"Продаж: {count},  Выручка: {mouney}, Скидка: {skidka}";


            cashiersDg.ItemsSource = sale
                .GroupBy(s=> s.Users.FullName).Select( g => 
                 new
                 {
                     Cashier = g.Key,
                     Count = g.Count(),
                     Mouney = g.Sum( a=> a.TotalAmount)

                 }
                ).ToList();


        }

        private void searchClient_Click(object sender, RoutedEventArgs e)
        {
            string phone = clientPhoneTb.Text;

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Ведите телефон");
                return;
            }

            var client = Db.Content.Clients.FirstOrDefault(c => c.PhoneNumber == phone);

            if(client == null)
            {
                MessageBox.Show("Клиент не найден");
                clientDg.ItemsSource = null;
                return;
            }

            clientDg.ItemsSource = Db.Content.Sales
                .Where(s => s.ClientId == client.Id)
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new
                {
                    Date = s.SaleDate,
                    Sum = s.TotalAmount,
                    Pay = s.PaymentType
                })
                .ToList();

           

        }
    }
}
