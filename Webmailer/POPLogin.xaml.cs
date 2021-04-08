using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Webmailer
{
    /// <summary>
    /// Logique d'interaction pour POPLogin.xaml
    /// </summary>
    public partial class FormLogin : Window
    {
        public String login { get; set; }

        public String userName { get; set; }

        public FormLogin()
        {
            InitializeComponent();
        }

        private void button_valider_Click(object sender, RoutedEventArgs e)
        {
            login = textBox_login.Password;
            userName = textBox_user.Text;
            DialogResult = true;
            //Debug.WriteLine("Username : {0} - Password : {1}", userName, login);
            Close();
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
