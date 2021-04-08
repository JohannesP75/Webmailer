using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using OpenPop.Mime.Header;
using System.Diagnostics;
using OpenPop.Mime;
using System.Net;
using System.Globalization;

namespace Webmailer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        SmtpClient smtpClient;
        Pop3Client pop3Client;
        int typePortSMTP=25, typePortPOP=110;
        MailItem[] listMails;

        private void resetIHM_SMTP(bool isSMTPconnected)
        {
            this.Resources["isSMTPConnected"] = isSMTPconnected;
            //comboBox_PortSMTP.IsEnabled = !isSMTPconnected;
            //addr_serv_smtp.IsEnabled = !isSMTPconnected;
            b_connect_SMTP.Content = isSMTPconnected ? (String)FindResource("boutonDeconnection") : (String)FindResource("boutonConnection");

            if (!isSMTPconnected)
            {
                // Purgeons tous les formulaires
                addr_expe_smtp.Text = "";
                addr_dest_smtp.Text = "";
                mail_subject_SMTP.Text = "";
                box_mail_SMTP.Text = "";
            }
        }

        private void resetIHM_POP(bool isPOPconnected)
        {
            this.Resources["isPOPConnected"] = isPOPconnected;
            textBox_nom_serveur_POP.IsEnabled = !isPOPconnected;
            comboBox_type_connection_POP.IsEnabled = !isPOPconnected;
            b_connect_POP.Content = isPOPconnected ? (String)FindResource("boutonDeconnection") : (String)FindResource("boutonConnection");

            if (!isPOPconnected)
            {
                // Purgeons tous les formulaires
                //listView_mails.ItemsSource = null;
                //listView_mails.Items.Clear();
                Array.Clear(listMails, 0, listMails.Length);
            }
        }
        private bool connexion_SMTP()
        {
            bool connect = false;
            ComboBoxPort stmp = (ComboBoxPort)comboBox_PortSMTP.SelectedItem;
            typePortSMTP = stmp.ValuePort;
            Debug.WriteLine("Numéro du port SMTP : {0}", typePortSMTP);
            FormLogin dlg = new FormLogin();

            if (dlg.ShowDialog() == true)
            {
                NetworkCredential basicCredential = new NetworkCredential(dlg.userName, dlg.login);
                smtpClient = new SmtpClient(addr_serv_smtp.Text, typePortSMTP);
                smtpClient.Credentials = basicCredential;
                connect = true;
            }

            return connect;
        }

        private bool connexion_POP()
        {
            pop3Client = new Pop3Client();

            try
            {
                pop3Client.Connect(textBox_nom_serveur_POP.Text, typePortPOP, typePortPOP == 995);
            }
            catch (PopServerNotAvailableException psnae)
            {
                MessageBox.Show("Serveur POP indisponible", "Erreur", MessageBoxButton.OK);
                Debug.WriteLine("Error : {0}", psnae.ToString());
            }
            catch (PopServerNotFoundException psnfe)
            {
                MessageBox.Show("Serveur POP non trouvé", "Erreur", MessageBoxButton.OK);
                Debug.WriteLine("Error : {0}", psnfe.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur inconnue lors de la connection", "Erreur", MessageBoxButton.OK);
                Debug.WriteLine("Error : {0}", ex.ToString());
            }

            if (pop3Client.Connected)
            {
                Debug.WriteLine("Client POP connecté");
                // Récupérer le login et le nom d'utilisateur
                FormLogin dlg = new FormLogin();
                bool error = false;

                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        pop3Client.Authenticate(dlg.userName, dlg.login, AuthenticationMethod.Auto);

                        // Récupérer la liste des emails
                        if (pop3Client.Connected)
                        {
                            //List<string> uidList = pop3Client.GetAll();
                            int messageNumber = pop3Client.GetMessageCount();
                            Debug.WriteLine("messageNumber : {0}", messageNumber);

                            //MessageHeader head;
                            //for (int i = 0; i < messageNumber; i++)
                            //{
                            //    head = pop3Client.GetMessageHeaders(i + 1);
                            //    listMails[i] = new MailItem(i + 1, head.From.ToString(), head.Subject, head.DateSent, 50);
                            //}

                            //listView_mails.ItemsSource = listMails;
                            //listView_mails.da
                        }
                    }
                    catch (InvalidLoginException ile)
                    {
                        MessageBox.Show("Les indentifications envoyées ont été refusées par le serveur", "Erreur", MessageBoxButton.OK);
                        Debug.WriteLine("Error : {0}", ile.ToString());
                        error = true;
                    }
                    catch (PopServerLockedException psle)
                    {
                        MessageBox.Show("La boite mail a été fermée", "Erreur", MessageBoxButton.OK);
                        Debug.WriteLine("Error : {0}", psle.ToString());
                        error = true;
                    }
                    catch (ArgumentNullException ane)
                    {
                        MessageBox.Show("Le mot de passe et/ou le nom d'utilisateur sont des valeurs nulles", "Erreur", MessageBoxButton.OK);
                        Debug.WriteLine("Error : {0}", ane.ToString());
                        error = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur inconnue lors de l'authentification", "Erreur", MessageBoxButton.OK);
                        Debug.WriteLine("Error : {0}", ex.ToString());
                        error = true;
                    }

                    
                }
                else
                {
                    error = true;
                }

                if (error&&pop3Client.Connected) pop3Client.Disconnect();
            }

            return pop3Client.Connected;
        }

        private void deconnexion_POP()
        {
            pop3Client.Disconnect();
            bool connect = pop3Client.Connected;

            if (!connect)
            {
                try
                {
                    int messageNumber = pop3Client.GetMessageCount();
                    Array.Clear(listMails, 0, messageNumber);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la déconnection avec le serveur POP", "Erreur", MessageBoxButton.OK);
                    Debug.WriteLine("Error : {0}", ex.ToString());
                }
                //listMails;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void b_connect_SMTP_Click(object sender, RoutedEventArgs e)
        {
            Boolean connect = (Boolean)FindResource("isSMTPConnected");

            if (connect)
            {
                // On se déconnecte
                smtpClient.Dispose();
                connect = false;
            }
            else
            {
                // On se connecte
                //ComboBoxPort stmp = (ComboBoxPort)comboBox_PortSMTP.SelectedItem;
                //typePortSMTP = stmp.ValuePort;
                //Debug.WriteLine("Numéro du port SMTP : {0}", typePortSMTP);
                //FormLogin dlg = new FormLogin();

                //if (dlg.ShowDialog()==true)
                //{
                //    NetworkCredential basicCredential = new NetworkCredential(dlg.userName, dlg.login);
                //    smtpClient = new SmtpClient(addr_serv_smtp.Text, typePortSMTP);
                //    smtpClient.Credentials = basicCredential;
                //    connect = true;
                //}
                connect = connexion_SMTP();
            }

            resetIHM_SMTP(connect);
        }

        private void button_connect_POP_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Numéro du port : {0} - Type de l'option : {1}");
            ComboBoxPort pop = (ComboBoxPort)comboBox_type_connection_POP.SelectedItem;
            typePortPOP = pop.ValuePort;
            Debug.WriteLine("Numéro du port POP : {0}", typePortPOP);
            //pop3Client = new Pop3Client();
            Boolean connect = (Boolean)this.Resources["isPOPConnected"];

            if (connect)
            {
                deconnexion_POP();
            }
            else
            {
                connect = connexion_POP();
            }

            resetIHM_POP(connect);
        }

        private void b_lireMail_Click(object sender, RoutedEventArgs e)
        {
            MailItem mi = (MailItem)listView_mails.SelectedValue;
            try
            {
                Message msg = pop3Client.GetMessage(mi.UID);
                MailForm mf = new MailForm();
                mf.readMail(msg);
                mf.Show();
            }
            catch (PopServerException pse)
            {
                MessageBox.Show("Erreur avec le serveur POP lors de la réception du mail", "Erreur", MessageBoxButton.OK);
                Debug.WriteLine("Error : {0}", pse.ToString());
            }
        }

        private void b_send_SMTP_Click(object sender, RoutedEventArgs e)
        {
            //smtpClient = new SmtpClient(addr_serv_smtp.Text, int.Parse(serv_port_smtp.Text));
            bool error = false;
            MailMessage message = new MailMessage(addr_expe_smtp.Text, addr_dest_smtp.Text, mail_subject_SMTP.Text, box_mail_SMTP.Text);
            
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Timeout = 100;

            try
            {
                smtpClient.Send(message);
            }
            catch (InvalidOperationException ioe)
            {
                MessageBox.Show("Opération invalide lors de l'envoi d'un message", "Erreur", MessageBoxButton.OK);
                Debug.WriteLine("Error : {0}", ioe.ToString());
                error = true;
            }
            catch (SmtpFailedRecipientsException sfre)
            {
                MessageBox.Show("Tout ou partie des destinataires n'ont pas reçu le mail", "Erreur", MessageBoxButton.OK);
                Debug.WriteLine("Error : {0}", sfre.ToString());
                error = true;
            }
            catch (SmtpException se)
            {
                MessageBox.Show("Problèmes sur connexion SMTP", "Erreur", MessageBoxButton.OK);
                Debug.WriteLine("Error : {0}", se.ToString());
                error = true;
            }

            if (error) {
                smtpClient.Dispose();
                this.Resources["isSMTPConnected"] = false;
            }
        }

    }

    public class ComboBoxPort
    {
        public string ValueString { get; set; }
        public int ValuePort { get; set; }
    }

    public class MailItem
    {
        public MailItem(int _UID, String _auteur, String _sujet, DateTime _date, long _taille)
        {
            UID = _UID;
            auteur = _auteur;
            sujet = _sujet;
            date = _date;
            taille = _taille;
        }
        public int UID { get; set; }
        public string auteur { get; set; }
        public string sujet { get; set; }
        public DateTime date { get; set; }
        public long taille { get; set; }
    }

    public class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return !(Boolean)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return !(Boolean)value;
        }
    }
}
