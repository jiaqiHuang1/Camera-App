using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camera_App
{
    public partial class SettingsPage_BLE : ContentPage
    {
        private readonly IAdapter _adapter;
        private readonly IBluetoothLE _bluetoothLE;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly HttpClient _httpClient;

        // Bluetooth Device List
        public ObservableCollection<IDevice> BluetoothDevices { get; private set; } = new ObservableCollection<IDevice>();

        private IDevice _selectedBluetoothDevice;

        public SettingsPage_BLE()
        {
            InitializeComponent();
            _bluetoothLE = CrossBluetoothLE.Current;
            _adapter = CrossBluetoothLE.Current.Adapter;
            _httpClient = new HttpClient();

            BluetoothDeviceListView.ItemsSource = BluetoothDevices;
        }


        // Handles button click events for connection type selection
        private async void OnConnectButtonClicked(object sender, EventArgs e)
        {
            BluetoothListLabel.IsVisible = true;
            BluetoothDeviceListView.IsVisible = true;
            await StartBluetoothScanAsync();
        }

        // Start Bluetooth scanning
        private async Task StartBluetoothScanAsync()
        {
            BluetoothDevices.Clear();

            if (_bluetoothLE.State != BluetoothState.On)
            {
                await DisplayAlert("Bluetooth", "Please enable Bluetooth in Settings.", "OK");
                return;
            }

            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "Location permission is required for Bluetooth scanning", "OK");
                return;
            }

            if (_adapter.IsScanning)
            {
                await _adapter.StopScanningForDevicesAsync();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            _adapter.DeviceDiscovered += OnBluetoothDeviceDiscovered;

            try
            {
                await _adapter.StartScanningForDevicesAsync(cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                await DisplayAlert("Bluetooth", "Device search timed out. Please try again.", "OK");
            }
            finally
            {
                _adapter.DeviceDiscovered -= OnBluetoothDeviceDiscovered;
            }
        }

        // Event Handling Procedures for Device Discovery
        private void OnBluetoothDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            var device = args.Device;
            if (!BluetoothDevices.Any(d => d.Id == device.Id))
            {
                BluetoothDevices.Add(device);
            }
        }

        // Event handler for selecting a Bluetooth device for connection
        private async void OnDeviceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedBluetoothDevice = e.SelectedItem as IDevice;
            if (_selectedBluetoothDevice == null) return;

            try
            {
                var connectionTask = _adapter.ConnectToDeviceAsync(_selectedBluetoothDevice);
                if (await Task.WhenAny(connectionTask, Task.Delay(10000)) == connectionTask)
                {
                    await DisplayAlert("Bluetooth", $"Connected to {_selectedBluetoothDevice.Name ?? "Unnamed Device"}", "OK");
                }
                else
                {
                    await DisplayAlert("Bluetooth", "Connection timed out. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Bluetooth", $"Failed to connect: {ex.Message}", "OK");
            }
            finally
            {
                BluetoothDeviceListView.SelectedItem = null;
            }
        }

        
    }
}
