using SignalDebug.Models;
using SignalDebug.Services;
using SignalDebug.ViewModels;

namespace SignalDebug.Views;

public partial class MainPage : ContentPage
{
    DataSignalDatabase dataSignalDatabase;
    BluetoothLeService bluetoothLeService;

    public MainPage(DataSignalDatabase _dataSignalDatabase, BluetoothLeService _bluetoothLeService)
	{
		InitializeComponent();
        dataSignalDatabase = _dataSignalDatabase;
        bluetoothLeService = _bluetoothLeService;
        var temp = new DataInfoListModel(_dataSignalDatabase);
        this.BindingContext = temp;
    }

    protected override async void OnAppearing()
    {
        var temp = this.BindingContext as DataInfoListModel;
        temp.InitData();

        if(!BluetoothLeService.IsBluetoothLePower)
        {
            BluetoothLeService.IsBluetoothLePower= await bluetoothLeService.CheckAndRequestBluetoothPermission();
        }

        MessagingCenter.Subscribe<DataInfoListModel>(this, "AddDataInfo", async (sender) =>
        {
            SaveDataInfoModel saveDataInfoModel = new SaveDataInfoModel(dataSignalDatabase);
            saveDataInfoModel.DataInfo = new DataInfo();
            saveDataInfoModel.Title = "添加数据源";
            await Navigation.PushAsync(new SaveDataInfoPage { BindingContext = saveDataInfoModel });
        });
        MessagingCenter.Subscribe<DataInfoListModel>(this, "EditDataInfo", async (sender) =>
        {
            SaveDataInfoModel saveDataInfoModel = new SaveDataInfoModel(dataSignalDatabase);
            if (string.IsNullOrEmpty((this.BindingContext as DataInfoListModel).DataInfo.DataId))
            {
                await DisplayAlert("提示", "请选择一条数据源进行编辑!", "确定");
            }
            else
            {
                saveDataInfoModel.DataInfo = (this.BindingContext as DataInfoListModel).DataInfo;
                saveDataInfoModel.Title = "编辑数据源";
                await Navigation.PushAsync(new SaveDataInfoPage { BindingContext = saveDataInfoModel });
            }
        });
        MessagingCenter.Subscribe<DataInfoListModel>(this, "DeleteDataInfo", async (sender) =>
        {
            if (string.IsNullOrEmpty((this.BindingContext as DataInfoListModel).DataInfo.DataId))
            {
                await DisplayAlert("提示", "请选择一条数据源进行删除!", "确定");
            }
            else
            {
                bool rel = await DisplayAlert("提示", $"确认删除数据源:{(this.BindingContext as DataInfoListModel).DataInfo.DataName}吗?", "确认", "取消");
                if (rel)
                {
                    var temp = this.BindingContext as DataInfoListModel;
                    temp.DeleteDataInfo();
                    temp.InitData();
                }
            }
        });
        base.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        MessagingCenter.Unsubscribe<DataInfoListModel>(this, "AddDataInfo");
        MessagingCenter.Unsubscribe<DataInfoListModel>(this, "EditDataInfo");
        MessagingCenter.Unsubscribe<DataInfoListModel>(this, "DeleteDataInfo");
        base.OnDisappearing();
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var datainfo = e.SelectedItem as DataInfo;
        DataInfoListModel dataInfoListModel = this.BindingContext as DataInfoListModel;
        dataInfoListModel.DataInfo = datainfo;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        string dataId = button.CommandParameter.ToString();
        var temp = this.BindingContext as DataInfoListModel;
        DataDebugModel dataDebugModel = new DataDebugModel();
        dataDebugModel.DataInfo = await temp.GetDataInfo(dataId);
        dataDebugModel.SignalInfos = await temp.GetSignalInfos(dataId);
        await Navigation.PushAsync(new DataDebugPage { BindingContext = dataDebugModel });
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var datainfo = e.CurrentSelection.FirstOrDefault() as DataInfo;
        DataInfoListModel dataInfoListModel = this.BindingContext as DataInfoListModel;
        dataInfoListModel.DataInfo = datainfo;
    }


    private async void recorddata_Clicked(object sender, EventArgs e)
    {
        string temp = FileSystem.Current.AppDataDirectory + "/datas/";
        var directorys = Directory.GetDirectories(temp);
        if (directorys == null || directorys.Length == 0)
            return;
        ShareDirectoryModel shareDirectoryModel = new ShareDirectoryModel();
        directorys?.ToList().ForEach(d =>
        {
            SignalDebug.Models.DirectoryInfo directoryInfo = new SignalDebug.Models.DirectoryInfo();
            directoryInfo.Directory = d.Replace(temp, string.Empty);
            directoryInfo.FullDirectory = d;
            shareDirectoryModel.DirectoryInfos.Add(directoryInfo);
        });
        await Navigation.PushAsync(new ShareDirectoryPage { BindingContext = shareDirectoryModel });
    }
}

