namespace SignalDebug.Views;

public partial class ShareDirectoryPage : ContentPage
{
	public ShareDirectoryPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        var ss = Directory.GetDirectories(FileSystem.Current.AppDataDirectory + "\\record");

        foreach (var d in ss)
        {

            Button button = new Button();
            button.Text = d.Replace(FileSystem.Current.AppDataDirectory + "\\record", string.Empty);
            button.ClassId = d;
            button.Clicked += Button_Clicked;
            verticalStackLayout.Children.Add(button);
        }



        base.OnAppearing();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string path = button.ClassId;
        await Navigation.PushAsync(new ShareFilePage { Path = path });
    }

    public class DirectoryInfo
    {
        public string DirectoryName { get; set; }
        public string Path { get; set; }
    }
}