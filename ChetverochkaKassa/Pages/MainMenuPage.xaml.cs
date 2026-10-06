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
    /// Логика взаимодействия для MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        private Users _currentUser;
        public MainMenuPage(Users user)
        {
            InitializeComponent();

            _currentUser = user;
            string roleName = user.Roles.Name;

            if(roleName == "Кассир")
            {
                btnProducts.Visibility = Visibility.Collapsed;
                btnUser.Visibility = Visibility.Collapsed;
                btnReport.Visibility = Visibility.Collapsed;

            }
            else if(roleName == "Менеджер")
            {
                btnUser.Visibility = Visibility.Collapsed;
            }


        }

        private void btSale_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SalesPage());

        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UsersPage());
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportsPage());
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsPage());
        }

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }
    }
}
