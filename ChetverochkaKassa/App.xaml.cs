using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChetverochkaKassa
{
    public partial class App : Application
    {
        public static Users CurrentUser { get; set; }

        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UpdatePasswords();
        }

        private void UpdatePasswords()
        {
            foreach (var user in Db.Content.Users.ToList())
            {
                if (!user.Password.StartsWith("$2a$") &&
                    !user.Password.StartsWith("$2b$") &&
                    !user.Password.StartsWith("$2y$"))
                {
                    user.Password = PasswordHelper.HashPassword(user.Password);
                }
            }
            Db.Content.SaveChanges();
        }
    }
}