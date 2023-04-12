using SignalDebug.ViewModels;

namespace SignalDebug.Views;

public partial class SaveSignalInfoPage : ContentPage
{
	public SaveSignalInfoPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        MessagingCenter.Subscribe<SaveSignalInfoModel, string>(this, "SaveSignalInfo", async (sender, arg) =>
        {
            await DisplayAlert("提示", $"{arg}！", "确认");
            if(arg == "保存成功")
            {
                await Shell.Current.GoToAsync("..");
            }
        });
        base.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        MessagingCenter.Unsubscribe<SaveSignalInfoModel, string>(this, "SaveSignalInfo");
        base.OnDisappearing();
    }
}