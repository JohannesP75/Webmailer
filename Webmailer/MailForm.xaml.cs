using OpenPop.Mime;
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

namespace Webmailer
{
    /// <summary>
    /// Logique d'interaction pour MailForm.xaml
    /// </summary>
    public partial class MailForm : Window
    {
        public MailForm()
        {
            InitializeComponent();
        }

        public void readMail(Message msg)
        {
            this.Title = "From : " + msg.Headers.From.ToString();
            this.textBox_cc.Text = msg.Headers.Cc.ToString();
            this.textBox_date.Text = msg.Headers.Date;
            this.textBox_sujet.Text = msg.Headers.Subject;
            this.textBox_mail.Text = msg.MessagePart.Body.ToString();
        }

        public void readMail(String _from, String _cc, DateTime _date, String _subject, String _body)
        {
            this.Title = "From : " + _from;
            this.textBox_cc.Text = _cc;
            this.textBox_date.Text = _date.ToString();
            this.textBox_sujet.Text = _subject;
            this.textBox_mail.Text = _body;
        }
    }
}
