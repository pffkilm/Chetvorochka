using System.Linq;
using System.Windows;

namespace ChetverochkaKassa
{
    /// <summary>
    /// Логика взаимодействия для RegisterClientWindow.xaml
    /// </summary>
    public partial class RegisterClientWindow : Window
    {
        private string _phone;

        public Clients CreateClient { get; private set; }

        public RegisterClientWindow(string phone)
        {
            InitializeComponent();
            _phone = phone;
            PhoneTb.Text = phone;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string phone = PhoneTb.Text;
           
            if (string.IsNullOrWhiteSpace(LastNameTb.Text) || string.IsNullOrWhiteSpace(FirstNameTb.Text) || string.IsNullOrWhiteSpace(PhoneTb.Text))
            {
                MessageBox.Show("Фамилия, имя, телефон обязательны");
                return;
            }
            else
            {
               
                var newClient = new Clients
                {
                    PhoneNumber = _phone,
                    LastName = LastNameTb.Text.Trim(),
                    FirstName = FirstNameTb.Text.Trim(),
                    BirthDate = Birthday.SelectedDate,
                    Discount = 5
                };

                Db.Content.Clients.Add(newClient);
                MessageBox.Show("Пользователь сохранен");
                Db.Content.SaveChanges();
                Close();

                

            }


        }
    }
}
