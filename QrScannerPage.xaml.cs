using ZXing.Net.Maui;

namespace TabletyRejestrApp;

public partial class QrScannerPage : ContentPage
{
    private bool _scanned = false;

    public QrScannerPage()
    {
        InitializeComponent();

        CameraView.Options = new BarcodeReaderOptions
        {
            AutoRotate = true,
            Multiple = false
        };
    }

    private async void BarcodesDetected(
    object sender,
    BarcodeDetectionEventArgs e)
    {
        try
        {
            if (_scanned)
                return;

            var result = e.Results.FirstOrDefault();

            if (result == null)
                return;

            string qrValue = result.Value?.Trim();
            
            if (string.IsNullOrWhiteSpace(qrValue))
                return;

            // Akceptujemy tylko TABLET:T-001
            if (!qrValue.StartsWith(
                    "TABLET:",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string tabletNumber = qrValue
                .Substring("TABLET:".Length)
                .Trim();

            if (string.IsNullOrWhiteSpace(tabletNumber))
                return;

            _scanned = true;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    await Shell.Current.GoToAsync(
                        $"..?qr={Uri.EscapeDataString(qrValue)}");
                }
                catch (Exception ex)
                {
                    await DisplayAlert(
                        "B³¹d przejœcia",
                        ex.ToString(),
                        "OK");
                }
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert(
                    "B³¹d skanera",
                    ex.ToString(),
                    "OK");
            });
        }
    }

}
