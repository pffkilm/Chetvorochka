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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private int _countLogin = 0;
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(_countLogin >= 3)
            {
                MessageBox.Show("Превышено число попыток. Обратитесь к админимстратору");
                return;
            }

            string hashedPassword = PasswordHelper.HashPassword(pbPassword.Password);
            
            var user = Db.Content.Users.FirstOrDefault(
                u => u.Login == txtLogin.Text
            );
           
            
            if (user == null || !PasswordHelper.VerifyPassword(pbPassword.Password, user.Password))
            {
                _countLogin++;
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            if (user.IsBlocked == true) {
                MessageBox.Show("Вы заблокированы,обратитесь к администратору. ");
                return;
            }

            NavigationService.Navigate(new MainMenuPage(user));
        }
    }
}
