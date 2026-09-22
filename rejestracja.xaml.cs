namespace projekt;

public partial class rejestracja : ContentPage
{
	public string x;
	public rejestracja(string wartosc)
	{
		InitializeComponent();
		x = wartosc;
		xxx.Text = x;
	}
}