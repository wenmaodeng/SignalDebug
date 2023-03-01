using SignalDebug.Models;
using SignalDebug.ViewModels;

namespace SignalDebug.Views;

public partial class SaveDataInfoPage : ContentPage
{
	public SaveDataInfoPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        var temp = this.BindingContext as SaveDataInfoModel;
        temp.InitData();

        MessagingCenter.Subscribe<SaveDataInfoModel>(this, "AddSignalInfo", async (sender) =>
        {
            if (string.IsNullOrEmpty(temp.DataInfo.DataId))
            {
                await DisplayAlert("提示", "请先添加数据源后再添加信号!", "确定");
            }
            else
            {
                SaveSignalInfoModel saveSignalInfoModel = new SaveSignalInfoModel(temp.dataSignalDatabase);
                saveSignalInfoModel.SignalInfo = new SignalInfo();
                saveSignalInfoModel.SignalInfo.DataId = temp.DataInfo.DataId;
                saveSignalInfoModel.Title = "添加信号";
                await Navigation.PushAsync(new SaveSignalInfoPage { BindingContext = saveSignalInfoModel });
            }
        });

        MessagingCenter.Subscribe<SaveDataInfoModel>(this, "EditSignalInfo", async (sender) =>
        {
            if (string.IsNullOrEmpty(temp.SignalInfo.DataId))
            {
                await DisplayAlert("提示", "请选择一条信号进行编辑!", "确定");
            }
            else
            {
                SaveSignalInfoModel saveSignalInfoModel = new SaveSignalInfoModel(temp.dataSignalDatabase);
                saveSignalInfoModel.SignalInfo = temp.SignalInfo;
                saveSignalInfoModel.Title = "编辑信号";
                await Navigation.PushAsync(new SaveSignalInfoPage { BindingContext = saveSignalInfoModel });
            }
        });
        MessagingCenter.Subscribe<SaveDataInfoModel>(this, "DeleteSignalInfo", async (sender) =>
        {
            if (string.IsNullOrEmpty(temp.SignalInfo.DataId))
            {
                await DisplayAlert("提示", "请选择一条信号进行删除!", "确定");
            }
            else
            {
                bool rel = await DisplayAlert("提示", $"确认删除信号:{temp.SignalInfo.SignalName}吗?", "确认", "取消");
                if (rel)
                {
                    var temp = this.BindingContext as SaveDataInfoModel;
                    temp.DeleteSignalInfo();
                    temp.InitData();
                }
            }
        });
        base.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        MessagingCenter.Unsubscribe<SaveDataInfoModel>(this, "AddSignalInfo");
        MessagingCenter.Unsubscribe<SaveDataInfoModel>(this, "EditSignalInfo");
        MessagingCenter.Unsubscribe<SaveDataInfoModel>(this, "DeleteSignalInfo");

        base.OnDisappearing();
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var signalInfo = e.SelectedItem as SignalInfo;
        SaveDataInfoModel saveDataInfoModel = this.BindingContext as SaveDataInfoModel;
        saveDataInfoModel.SignalInfo = signalInfo;
    }
}