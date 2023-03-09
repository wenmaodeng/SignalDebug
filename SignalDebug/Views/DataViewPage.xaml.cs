namespace SignalDebug.Views;

public partial class DataViewPage : ContentPage
{
	public string[] Data = null;
	public DataViewPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        if(Data != null)
        {
            string temp = string.Empty;
            Data.ToList().ForEach(s =>
            {
                temp += s;
            });
            editor.Text = temp;
        }
        base.OnAppearing();
    }
}