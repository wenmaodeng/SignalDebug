namespace SignalDebug.Views;

public partial class ShareFilePage : ContentPage
{
    public string Path { get; set; }
    public ShareFilePage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        verticalStackLayout.Children.Clear();
        var files = Directory.GetFiles(Path);
        foreach (var file in files)
        {
            Button button = new Button();
            button.ClassId = file;
            button.Text = System.IO.Path.GetFileName(file);
            button.Clicked += Button_Clicked;
            verticalStackLayout.Children.Add(button);
        }
        base.OnAppearing();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string file = button.ClassId;
        if (File.Exists(file))
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Share text file",
                File = new ShareFile(file)
            });
        }
    }
}