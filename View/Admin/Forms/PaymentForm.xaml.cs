using AForge.Video;
using AForge.Video.DirectShow;
using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using GYM_CLIENT.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
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
using ZXing;
using static QRCoder.PayloadGenerator;
using static QRCoder.PayloadGenerator.SwissQrCode;

namespace GYM_CLIENT.View.Admin.Forms
{
    /// <summary>
    /// Interaction logic for PaymentForm.xaml
    /// </summary>
    public partial class PaymentForm : Window
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public event EventHandler paymentCreated;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentBitmap;
        private readonly ClientService _service;

        private string lastScannedCode = string.Empty;

        public PaymentForm()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            _service = new ClientService();

        }
        private void StartCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No camera found");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
            videoSource.Start();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartCamera();
        }


        string ClientId = "";
        decimal? priceOfPlan = 0;
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Clone frame and update currentBitmap
            currentBitmap?.Dispose();
            currentBitmap = (Bitmap)eventArgs.Frame.Clone();

            var result = DecodeQRCode(currentBitmap);

            if (result != null && result.Text != lastScannedCode)
            {
                lastScannedCode = result.Text;


                var userInfo = GetUserByBarcodeAsync(result.Text);

                _ = (Dispatcher?.BeginInvoke(() =>
                {
                    if (userInfo != null)
                    {
                        ClientId = userInfo.ClientId;

                        Membership.Text = userInfo.PlanName;
                        ClientName.Text = userInfo.FullName;
                        priceOfPlan = userInfo.Price;
                        Amount.Text = userInfo.Price.ToString();
                        DaysleftTxt.Text = userInfo.DaysLeft.ToString();
                        return;


                    }
                    else
                    {
                        MessageBox.Show($"QR Code: {result.Text}\nNo active client found with this QR code.");

                    }
                }));

            }

            var image = BitmapToImageSource(currentBitmap);
            image.Freeze();

            Dispatcher.BeginInvoke(() =>
            {
                CameraImage.Source = image;
            });
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseCamera();
            this.Close();
        }
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void CloseCamera()
        {

            if (videoSource != null)
            {
                videoSource.SignalToStop();

                // wait ~ 3 seconds
                for (int i = 0; i < 30; i++)
                {
                    if (!videoSource.IsRunning)
                        break;
                    System.Threading.Thread.Sleep(100);
                }

                if (videoSource.IsRunning)
                {
                    videoSource.Stop();
                }

            }
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            AddPayment();
        }

        private Result DecodeQRCode(Bitmap bitmap)
        {
            try
            {

                var reader = new BarcodeReader<Bitmap>(b =>
                {
                    var data = b.LockBits(new System.Drawing.Rectangle(0, 0, b.Width, b.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    try
                    {
                        var bytes = new byte[data.Stride * data.Height];
                        System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                        return new RGBLuminanceSource(bytes, b.Width, b.Height);
                    }
                    finally
                    {
                        b.UnlockBits(data);
                    }
                });
                return reader.Decode(bitmap);


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private ClientModel GetUserByBarcodeAsync(string barcode)
        {
            try
            {

                sqlConnection.Open();

                string query = @"
                       SELECT c.ClientId,
                             c.FullName,
                             c.Contact,
                             c.TrainerId,
                             c.PlanId,
                             p.PlanName,  
                             p.Price,
                             c.hasAccessCard,
                             DATEDIFF(DAY, GETDATE(), c.PlanEnded) AS DaysLeft,
                             ac.CardID,
                             ac.CardNumber,
                             ac.IssuedDate,
                             ac.ExpiryDate
                        FROM [GymDb].[dbo].[Client] c
                        INNER JOIN [GymDb].[dbo].[Plan] p 
                            ON c.PlanId = p.Id
                        INNER JOIN [GymDb].[dbo].[AccessCards] ac 
                            ON c.ClientId = ac.ClientId
                        WHERE c.ClientId = @ClientId";

                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@ClientId", barcode);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            return new ClientModel
                            {
                                ClientId = reader["ClientId"].ToString(),
                                FullName = reader.GetString("FullName"),                             
                                PlanId = reader["PlanId"].ToString(),
                                PlanName = reader["PlanName"].ToString(),
                                Price = Convert.ToDecimal( reader["Price"]),
                                CardNumber = reader["CardNumber"].ToString(),
                                DaysLeft = reader["DaysLeft"] != DBNull.Value
                                ? Convert.ToInt32(reader["DaysLeft"])
                                : 0,
                                

                            };

                        }

                    }
                    sqlConnection.Close();

                }

            }
            catch (SqlException ex)
            {
                Dispatcher?.BeginInvoke(() =>
                {
                    MessageBox.Show($"Database error: {ex.Message}");
                });
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                Dispatcher?.BeginInvoke(() =>
                {
                    MessageBox.Show($"Error retrieving user: {ex.Message}");
                });
                sqlConnection.Close();

            }

            return null;
        }



        private void AddPayment()
        {
            string query = @"INSERT INTO Payments (PaymentId,ClientId,NonMemberClient,PaymentType,PlanName,AmountPaid,Changed)
                     VALUES(@PaymentId,@ClientId,@NonMemberClient,@PaymentType,@PlanName,@AmountPaid,@Changed)";

            if (!decimal.TryParse(Amount.Text, out decimal amountValue))
            {
                MessageBox.Show("Please enter a valid numeric amount.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (amountValue < priceOfPlan)
            {
                MessageBox.Show("Entered amount is less than the plan price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (sqlConnection.State != System.Data.ConnectionState.Open) 
                    sqlConnection.Open();

                string? paymentType = PaymentType?.Text;

                if (string.IsNullOrWhiteSpace(paymentType))
                {
                    MessageBox.Show("Please select a payment type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@PaymentId", $"PAY-{Guid.NewGuid().ToString().ToUpper().Substring(0, 6)}");

                    if (!string.IsNullOrWhiteSpace(ClientId))
                    {
                        cmd.Parameters.AddWithValue("@ClientId", ClientId);
                        cmd.Parameters.AddWithValue("@NonMemberClient", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ClientId", DBNull.Value);
                        cmd.Parameters.AddWithValue("@NonMemberClient", $"NON-MEMBER-{Guid.NewGuid().ToString().ToUpper().Substring(0, 4)}");
                    }

                    cmd.Parameters.AddWithValue("@PaymentType", paymentType);
                    cmd.Parameters.AddWithValue("@PlanName", Membership.Text);
                    cmd.Parameters.AddWithValue("@AmountPaid", amountValue);
                    cmd.Parameters.AddWithValue("@Changed",
                        decimal.TryParse(Change.Text, out decimal changeValue) ? changeValue : 0m);

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        MessageBox.Show("Payment created successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        paymentCreated?.Invoke(this, new EventArgs());

                        //Renew Contract if client Id is not null and the Daysleft is 0
                        if (!string.IsNullOrWhiteSpace(ClientId))
                        {
                            UpdateContract(ClientId);
                        }
                        Clear();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open) 
                    sqlConnection.Close();
            }
        }


        private void UpdateContract(string? ClientId)
        {
            string query = "UPDATE Client SET PlanStarted = @PlanStarted, PlanEnded = @PlanEnded WHERE ClientId = @ClientId";

            string? selectedPlanId = Membership.Text.ToLower();
            string id = selectedPlanId == "yearly" ? "PLAN-101" : "PLAN-102";

            var duration = _service.FetchDaysOfPlan(id);

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection)) // reuse existing open connection
            {
                cmd.Parameters.AddWithValue("@PlanStarted", DateTime.Now);
                cmd.Parameters.AddWithValue("@PlanEnded", DateTime.Now.AddDays(duration));
                cmd.Parameters.AddWithValue("@ClientId", ClientId);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    this.Close();
                }
            }
        }


        private void Clear()
        {

            Amount.Text = "";
            DaysleftTxt.Text = "";
            ClientName.Text = "";
            ClientId = "";
            Membership.Text = "";
            priceOfPlan = 0;
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Amount_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(Amount.Text, out decimal value))
            {
                decimal? change = 0;

                if (priceOfPlan > 0)
                {
                    change = value - priceOfPlan;
                }

                Change.Text = change.ToString();
            }
            else
            {
                Change.Text = "0"; // reset if invalid input
            }

        }

        private void DaysleftTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

            if(int.TryParse(DaysleftTxt.Text, out int day))
            {

                if(day <= 0)
                {
                    Submit.IsEnabled = true;

                }
                else
                {
                    MessageBox.Show("Client has remaining days in his/her membership!", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear();
                    sqlConnection.Close();

                }
            }

        }

        private void Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.ValidationHelper.NoSpaceOnly(sender, e);

        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.AllowOnlyNumbers(sender, e);

        }
    }
}
