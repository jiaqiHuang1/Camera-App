using System.Net.Sockets;
using Microsoft.Maui.Controls;
using System.IO;

namespace Camera_App
{
    public partial class SettingsPage_WLAN : ContentPage
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _serverIp;
        private const int serverPort = 12345;
        private const int maxImageSize = 5 * 1024 * 1024; // Set the maximum image size to 5MB

        public SettingsPage_WLAN()
        {
            InitializeComponent();
        }

        private async void OnConnectButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (_client == null || !_client.Connected)
                {
                    // Get the IP address entered by the user
                    _serverIp = IpEntry.Text?.Trim();
                    if (string.IsNullOrEmpty(_serverIp))
                    {
                        await DisplayAlert("Error", "Please enter a valid IP address.", "OK");
                        return;
                    }

                    // Creating a new TCP connection
                    _client = new TcpClient();
                    await _client.ConnectAsync(_serverIp, serverPort);
                    _stream = _client.GetStream();

                    // Update button status
                    ConnectButton.Text = "Disconnect";
                    CaptureButton.IsEnabled = true; // Enable photo button
                    await DisplayAlert("Connected", $"Successfully connected to {_serverIp}.", "OK");
                }
                else
                {
                    // Disconnect
                    _stream.Close();
                    _client.Close();
                    _client = null;

                    // Update button status
                    ConnectButton.Text = "Connect";
                    CaptureButton.IsEnabled = false; // Disable photo button
                    await DisplayAlert("Disconnected", "Disconnected from the server.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnCaptureButtonClicked(object sender, EventArgs e)
        {
            if (_client == null || !_client.Connected)
            {
                await DisplayAlert("Error", "Please connect to the server first.", "OK");
                return;
            }

            CaptureButton.IsEnabled = false; // Disable the photo button to prevent repeat clicks
            ActivityIndicator.IsRunning = true; // Show loading animation

            try
            {
                // Send photo request
                byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes("TAKE_PHOTO");
                await _stream.WriteAsync(requestBytes, 0, requestBytes.Length);

                // Receive image size information
                byte[] lengthPrefix = new byte[4];
                await _stream.ReadAsync(lengthPrefix, 0, lengthPrefix.Length);
                int imageSize = BitConverter.ToInt32(lengthPrefix, 0);

                // Calibrate image size
                if (imageSize <= 0 || imageSize > maxImageSize)
                {
                    throw new Exception("Invalid image size received.");
                }

                // Receive image data
                byte[] imageBytes = new byte[imageSize];
                int bytesReceived = 0;
                while (bytesReceived < imageSize)
                {
                    bytesReceived += await _stream.ReadAsync(imageBytes, bytesReceived, imageSize - bytesReceived);
                }

                // Show picture
                var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                CapturedImage.Source = imageSource;
                await DisplayAlert("Success", "Photo captured and received successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                CaptureButton.IsEnabled = true; // Re-enable the photo button
                ActivityIndicator.IsRunning = false; // Stop loading animation
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Cleaning up resources on page closure
            _stream?.Close();
            _client?.Close();
        }
    }
}
