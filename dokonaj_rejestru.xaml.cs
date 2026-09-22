using Microsoft.Maui.Controls;
using ZXing.Net.Maui;

namespace projekt;

public partial class dokonaj_rejestru : ContentPage
{
	public dokonaj_rejestru()
	{
		InitializeComponent();
	}
    private async void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        string x;
        var result = e?.Results?.FirstOrDefault(); // Jeœli e nie jest null pobierz wynik(kolekcje wyników) i weŸ pierwszy element(FirstOrDefault)
        if (result is null)
            return;
        // Wywo³anie na w¹tku UI i przypisanie do labela, kamera dzia³a na w¹tku t³a i pobranie wyniku z kamery nie mo¿e byæ przypisane do labela, poniewa¿ label dzia³a na w¹tku UI,
        // w¹tek t³a mo¿e nadal dzia³aæ i wykrywaæ kolejne kody
        // Podstawowe w¹tki: MainThead - w¹tek UI, Background Thread - w¹tek t³a, w którym dzia³a kamera, 
        MainThread.BeginInvokeOnMainThread(() =>
        {
            
            x= result.Value;
        });
        if (result != null)
        {
            x = result.Value;

            await Navigation.PushAsync(new rejestracja(x));
        }
    }
}