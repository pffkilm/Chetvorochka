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

namespace ChetverochkaKassa
{
    /// <summary>
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        public string PaymentType { get; private set; }   
        public decimal Received { get; private set; }     
        public decimal Change { get; private set; }

        private decimal _finalSum;
        public PaymentWindow(decimal finalSum)
        {
            InitializeComponent();
            _finalSum = finalSum;
            sumPay.Text = $"{finalSum}";
            paymentTb.Text = finalSum.ToString();

            cashierCombo.ItemsSource = Db.Content.Users.ToList();
            cashierCombo.DisplayMemberPath = "FullName";
            cashierCombo.SelectedIndex = 0;

        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {

            var user = cashierCombo.SelectedItem as Users;
            App.CurrentUser = user;
          if(cashRadio.IsChecked == true)
            {
                decimal received;
                if(!decimal.TryParse(paymentTb.Text, out received))
                {
                    MessageBox.Show("Введите сумму");
                    return;
                }
                if(received < _finalSum)
                {
                    MessageBox.Show("Сумма меньше к оплате");
                    return;
                }

                PaymentType = "Наличные";
                Received = received;
               Change = received - _finalSum;
                changeTb.Text = $"{Change}";
            }
            else
            {
                PaymentType = "Карта";
                Received = _finalSum;
                Change = 0;
                changeTb.Text = "0";

            }
            DialogResult = true;
            Close();
        }
    }
}
