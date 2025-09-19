

using AForge.Video;
using AForge.Video.DirectShow;
using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;


namespace GYM_CLIENT.View.Admin.Forms
{
    /// <summary>
    /// Interaction logic for CameraWindow.xaml
    /// </summary>
    public partial class CameraWindow : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        private Bitmap currentBitmap;
        private string lastScannedCode = string.Empty;
        public CameraWindow()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseCamera();
            this.Close();

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
        private  void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Clone frame and update currentBitmap
            currentBitmap?.Dispose();
            currentBitmap = (Bitmap)eventArgs.Frame.Clone();

            var result = DecodeQRCode(currentBitmap);

            if (result != null && result.Text != lastScannedCode)
            {
                  lastScannedCode = result.Text;

             
                    var userInfo = GetUserByBarcodeAsync(result.Text); 

                    Dispatcher?.BeginInvoke(() =>
                    {
                        if (userInfo != null)
                        {
                            string IdClient = userInfo.ClientId;


                            Membership.Text = userInfo.PlanName;
                            ClientName.Text = userInfo.FullName;
                            CardNumber.Text = userInfo.CardNumber;

                            //Validate first if the Client has valid plan
                            UpdateAttendance(IdClient);

                        }
                        else
                        {
                            MessageBox.Show($"QR Code: {result.Text}\nNo active client found with this QR code.");
                        }
                    });
              
            }

            var image = BitmapToImageSource(currentBitmap);
            image.Freeze();

            Dispatcher.BeginInvoke(() =>
            {
                CameraImage.Source = image;
            });
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

        private Result DecodeQRCode(Bitmap bitmap)
        {
            try
            {

                var reader = new BarcodeReader<Bitmap>(b =>
                {
                    var data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
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


        private  ClientModel GetUserByBarcodeAsync(string barcode)
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
                             c.hasAccessCard,
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
                                    //Contact = reader.IsDBNull("Contact") ? "N/A" : reader.GetString("Contact"),
                                    //TraineeId = reader.GetString("TrainerId") ,
                                    PlanId = reader["PlanId"].ToString(),
                                    PlanName = reader["PlanName"].ToString(),
                                    //hasAccessCard = reader.GetBoolean("hasAccessCard"),
                                    CardNumber = reader["CardNumber"].ToString(),
                                    //IssuedDate = reader.GetDateTime("IssuedDate"),
                                    //ExpiryDate = reader.GetDateTime("ExpiryDate"),
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
            }
            catch (Exception ex)
            {
                Dispatcher?.BeginInvoke(() =>
                {
                    MessageBox.Show($"Error retrieving user: {ex.Message}");
                });
            }

            return null ;
        }



        private void UpdateAttendance(string clientId)
        {
            try {


                string QUERY = "Insert Into Attendance (ClientId,CheckInTime) VALUES(@ClientId,@CheckInTime)";
                SqlCommand cmd = new SqlCommand(QUERY, sqlConnection);
                cmd.Parameters.AddWithValue("@ClientId", clientId);
                cmd.Parameters.AddWithValue("@CheckInTime", DateTime.Now);

                int row = cmd.ExecuteNonQuery();

                if(row > 0 )
                {

                    MessageBox.Show("Client attendance recorded","Success",MessageBoxButton.OK,MessageBoxImage.Information);
                    sqlConnection.Close();

                    return;
                }

            }
            catch (Exception ex) {


                MessageBox.Show("Error" + ex);
            }


        }
    }
}
