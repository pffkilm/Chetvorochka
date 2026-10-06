using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            LoadUsers();
            LoadRoles();
        }

        private void goBack_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadUsers()
        {
            var users = Db.Content.Users.ToList();
            usersDg.ItemsSource = users;


        }

        private void LoadRoles()
        {
            roleCombo.ItemsSource = Db.Content.Roles.ToList();
            roleCombo.DisplayMemberPath = "Name";
            roleCombo.SelectedIndex = 0;
        }
        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fullNameTb.Text) ||
                 string.IsNullOrWhiteSpace(loginTb.Text) ||
                 string.IsNullOrWhiteSpace(passwordPb.Password))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (Db.Content.Users.Any(x => x.Login == loginTb.Text))
            {
                MessageBox.Show("Логин уже существует");
                return;
            }

            var role = roleCombo.SelectedItem as Roles;
            if (role == null)
            {
                MessageBox.Show("Выберите роль");
                return;
            }

            var user = new Users
            {
                FullName = fullNameTb.Text.Trim(),
                Login = loginTb.Text.Trim(),
                Password = PasswordHelper.HashPassword(passwordPb.Password),
                RoleId = role.Id,
                IsBlocked = false
            };

            Db.Content.Users.Add(user);
            Db.Content.SaveChanges();

            MessageBox.Show("Пользователь добавлен");
            LoadUsers();

            fullNameTb.Text = "";
            loginTb.Text = "";
            passwordPb.Password = "";
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            var selected = usersDg.SelectedItem as Users;
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            var role = roleCombo.SelectedItem as Roles;
            if (role == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            var user = Db.Content.Users.FirstOrDefault(u=> u.Id == selected.Id);

            if (user == null) return;

            user.RoleId = role.Id;
            Db.Content.SaveChanges();

            MessageBox.Show("Роль изменена");
            LoadUsers();

        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            var selected = usersDg.SelectedItem as Users;

            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }
            if (string.IsNullOrWhiteSpace(resetPasswordPb.Password))
            {
                MessageBox.Show("Введите новый пароль");
                return;
            }

            var user = Db.Content.Users.FirstOrDefault( a => a.Id == selected.Id);

            if(user == null) return;

            user.Password = PasswordHelper.HashPassword(resetPasswordPb.Password);

            Db.Content.SaveChanges();
            MessageBox.Show("Пароль сброшен");
            resetPasswordPb.Password = "";
        }

        private void block_Click(object sender, RoutedEventArgs e)
        {

            var selected = usersDg.SelectedItem as Users;

            if(selected == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            

            var user = Db.Content.Users.FirstOrDefault( s => s.Id ==  selected.Id);
            if (user == null) return;

            if (user.IsBlocked == true)
                user.IsBlocked = false;
            else
                user.IsBlocked = true;

            Db.Content.SaveChanges();

            MessageBox.Show(user.IsBlocked == true ? "Заблокирован" : "Разблокирован");
            LoadUsers();

        }
    }
}
