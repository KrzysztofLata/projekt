namespace projekt;

public partial class settings : ContentPage
{
	public settings()
	{
		InitializeComponent(); Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsVisible = false
        });
    }
}