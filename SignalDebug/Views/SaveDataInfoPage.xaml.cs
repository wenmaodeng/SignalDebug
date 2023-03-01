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
                await DisplayAlert("��ʾ", "�����������Դ��������ź�!", "ȷ��");
            }
            else
            {
                SaveSignalInfoModel saveSignalInfoModel = new SaveSignalInfoModel(temp.dataSignalDatabase);
                saveSignalInfoModel.SignalInfo = new SignalInfo();
                saveSignalInfoModel.SignalInfo.DataId = temp.DataInfo.DataId;
                saveSignalInfoModel.Title = "����ź�";
                await Navigation.PushAsync(new SaveSignalInfoPage { BindingContext = saveSignalInfoModel });
            }
        });

        MessagingCenter.Subscribe<SaveDataInfoModel>(this, "EditSignalInfo", async (sender) =>
        {
            if (string.IsNullOrEmpty(temp.SignalInfo.DataId))
            {
                await DisplayAlert("��ʾ", "��ѡ��һ���źŽ��б༭!", "ȷ��");
            }
            else
            {
                SaveSignalInfoModel saveSignalInfoModel = new SaveSignalInfoModel(temp.dataSignalDatabase);
                saveSignalInfoModel.SignalInfo = temp.SignalInfo;
                saveSignalInfoModel.Title = "�༭�ź�";
                await Navigation.PushAsync(new SaveSignalInfoPage { BindingContext = saveSignalInfoModel });
            }
        });
        MessagingCenter.Subscribe<SaveDataInfoModel>(this, "DeleteSignalInfo", async (sender) =>
        {
            if (string.IsNullOrEmpty(temp.SignalInfo.DataId))
            {
                await DisplayAlert("��ʾ", "��ѡ��һ���źŽ���ɾ��!", "ȷ��");
            }
            else
            {
                bool rel = await DisplayAlert("��ʾ", $"ȷ��ɾ���ź�:{temp.SignalInfo.SignalName}��?", "ȷ��", "ȡ��");
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