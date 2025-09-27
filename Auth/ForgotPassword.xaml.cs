using GYM_CLIENT.DatabaseConnection;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using static QRCoder.PayloadGenerator;

namespace GYM_CLIENT.Auth
{
    /// <summary>
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window
    {

        private string generatedOTP;
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public ForgotPassword()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }


        private void SendOTP_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(OTP.Text))
            {
                MessageBox.Show("Please enter the OTP.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OTP.Text == generatedOTP)
            {
                MessageBox.Show("OTP verified successfully! You may now reset your password.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                var changePassword = new ChangePassword(email);
                changePassword.Show();

                this.Close();

            }
            else
            {
                MessageBox.Show("Invalid OTP. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                OTP.Text = "";
            }

            

        }
        private void Disable()
        {
            OTP.IsEnabled = false;
            SubmitOtp.IsEnabled = false;


        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        string email = "";
        private void VerifyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email.Text))
            {
                MessageBox.Show("Please enter your registered email.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                bool emailExists = false;


                sqlConnection.Open();
                string query = "SELECT COUNT(*) FROM Staff WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Email", Email.Text.Trim());
                    int count = (int)cmd.ExecuteScalar();
                    emailExists = count > 0;
                }


                if (!emailExists)
                {
                    MessageBox.Show("This email is not registered in our system.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Email.Text = "";
                    sqlConnection.Close();

                    return;
                }

                // Generate a 6-digit OTP
                Random random = new Random();
                generatedOTP = random.Next(100000, 999999).ToString();

                // Configure SMTP client
                // Configure SMTP client
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Gmail uses 587 for TLS
                    Credentials = new NetworkCredential("alicantejannet5@gmail.com", "rsvx lqtn tgcq txps"), // App password
                    EnableSsl = true,
                };


                // Create the email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("alicantejannet5@gmail.com"),
                    Subject = "Gym - Password Reset OTP",
                    Body = $"Your OTP for resetting the password is: {generatedOTP}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(Email.Text);

                // Send email
                smtpClient.Send(mailMessage);

                MessageBox.Show("OTP sent successfully. Please check your email.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                sqlConnection.Close();

                email = Email.Text;
                Email.Text = "";

                OTP.IsEnabled = true;
                SubmitOtp.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send OTP. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);


            }
        }

        private void Email_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.ValidationHelper.NoSpaceOnly(sender, e);

        }

        private void Email_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.EmailTextComposition(sender, e);

        }

        private void OTP_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.ValidationHelper.NoSpaceOnly(sender, e);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Disable();
        }

        private void OTP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.AllowOnlyNumbers(sender, e);

        }


        private void MaximizeBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;

            }
            else
            {

                WindowState = WindowState.Normal;

            }
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {

                WindowState = WindowState.Minimized;
            }
            else
            {

                WindowState = WindowState.Normal;

            }

        }
    }
}
