using InTheHand.Bluetooth;
using Microsoft.Maui.Graphics;
using SignalDebug.ViewModels;
using System.Diagnostics;
using System.Text;
using YunBang.MAUI.UI.OutDisplay;

namespace SignalDebug.Views;

public partial class DataDebugPage : ContentPage
{
    string path = FileSystem.Current.AppDataDirectory + "\\record\\" + DateTime.Now.ToString("yyyy_MM_dd");
    string filename = string.Empty;
    int datacount = 0;
    GattCharacteristic gattCharacteristic = null;
    /// <summary>
    /// 0 记录全部数据 1 记录全部信号列表数据 2 记录当前信号列表数据
    /// </summary>
    int recordtype = 2;
    static bool IsSaveData = false;
    BluetoothDevice device = null;
    private static List<Element> contentViews = new List<Element>();
    private static DataDebugModel dataDebugModel = null;
    public DataDebugPage()
	{
		InitializeComponent();
        ConnectBluetooth();
    }
    private void SaveData(string data)
    {
        if (string.IsNullOrEmpty(path))
            return;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (string.IsNullOrEmpty(filename))
        {
            string fn = DateTime.Now.ToString("yyyyMMddHHmmss") + $"_{dataDebugModel.DataInfo.DataName}.txt";
            filename = Path.Combine(path, fn);
        }
        data = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + data;

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true))
        {
            file.WriteLine(data);
            datacount++;
            if (MainThread.IsMainThread)
            {
                RecordData.Text = $"已记录数量【{datacount}】";
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => { RecordData.Text = $"已记录数量【{datacount}】"; });
            }
        }
    }
    protected override void OnAppearing()
    {
        InitCintrol();
        base.OnAppearing();
    }
    private void InitCintrol()
    {
        dataDebugModel = this.BindingContext as DataDebugModel;
        contentViews.Clear();
        verticalStackLayout.Children.Clear();

        if (dataDebugModel.SignalInfos != null && dataDebugModel.SignalInfos.Count > 0)
        {
            for (int i = 0; i < dataDebugModel.SignalInfos.Count; i++)
            {
                //if (temp.SignalInfos[i].Enabled)
                //{
                switch (dataDebugModel.SignalInfos[i].DataType)
                {
                    case Models.DataType.BoolSignal:
                        break;
                    case Models.DataType.FloatSignal:
                        TextDisplay textShowFloatSignal = new TextDisplay();
                        textShowFloatSignal.LableText = dataDebugModel.SignalInfos[i].SignalName;
                        textShowFloatSignal.Describe = dataDebugModel.SignalInfos[i].Unit;
                        textShowFloatSignal.ClassId = dataDebugModel.SignalInfos[i].SignalId;
                        contentViews.Add(textShowFloatSignal);
                        verticalStackLayout.Add(textShowFloatSignal);
                        break;
                    case Models.DataType.IntSignal:
                        TextDisplay textShowIntSignal = new TextDisplay();
                        textShowIntSignal.LableText = dataDebugModel.SignalInfos[i].SignalName;
                        textShowIntSignal.Describe = dataDebugModel.SignalInfos[i].Unit;
                        textShowIntSignal.ClassId = dataDebugModel.SignalInfos[i].SignalId;
                        contentViews.Add(textShowIntSignal);
                        verticalStackLayout.Add(textShowIntSignal);
                        break;
                    default:
                        break;
                }
                //}
            }
        }
    }

        //}
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="data"></param>
        private void HandleData(string data)
    {
        string[] tempstrs = data.Split(',');
        if (recordtype == 0)
        {
            if (IsSaveData)
            {

            }
        }
        else if (recordtype == 1)
        {
            if (IsSaveData)
            {

            }
        }
        else if (recordtype == 2)
        {

        }
        if (dataDebugModel != null)
        {
            dataDebugModel.SignalInfos.ForEach(s =>
            {
                string value = tempstrs[s.SignalBit];
                var element = contentViews.FirstOrDefault(x => x.ClassId == s.SignalId);
                switch (s.DataType)
                {
                    case Models.DataType.BoolSignal:
                        break;
                    case Models.DataType.FloatSignal:
                        TextDisplay textShowFloatSignal = element as TextDisplay;
                        if (MainThread.IsMainThread)
                        {
                            textShowFloatSignal.TextValue = value;
                        }
                        else
                        {
                            MainThread.BeginInvokeOnMainThread(() => { textShowFloatSignal.TextValue = value; });
                        }
                        break;
                    case Models.DataType.IntSignal:
                        TextDisplay textShowFloatIntSignal = element as TextDisplay;
                        if (MainThread.IsMainThread)
                        {
                            textShowFloatIntSignal.TextValue = value;
                        }
                        else
                        {
                            MainThread.BeginInvokeOnMainThread(() => { textShowFloatIntSignal.TextValue = value; });
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }
    protected override void OnDisappearing()
    {
        if (gattCharacteristic != null)
        {
            gattCharacteristic.CharacteristicValueChanged -= Chars_CharacteristicValueChanged;
            gattCharacteristic = null;
        }
        if (device != null && device.Gatt != null && device.Gatt.IsConnected)
        {
            device.Gatt.Disconnect();
        }
        base.OnDisappearing();
    }
    private async void ConnectBluetooth()
    {
        bool availability = false;
        int count = 0;
        while (!availability)
        {
            availability = await Bluetooth.GetAvailabilityAsync();
            await Task.Delay(500);
            count++;
            if (count > 5)
            {
                await DisplayAlert("提示", "硬件无蓝牙", "确认");
                return;
            }
        }
        await Task.Delay(3000);
        RequestDeviceOptions options = new RequestDeviceOptions();
        options.AcceptAllDevices = true;
        device = await Bluetooth.RequestDeviceAsync(options);

        if (device != null)
        {
            count = 0;
            while (!device.Gatt.IsConnected)
            {
                await device.Gatt.ConnectAsync();
                count++;
                await Task.Delay(500);
                if (count == 5)
                {
                    await DisplayAlert("提示", "连接蓝牙失败", "确认");
                    return;
                }
            }
            device.Gatt.RequestMtu(512);
            var servs = await device.Gatt.GetPrimaryServicesAsync();
            bool isfindNotify = false;
            for (int i = 0; i < servs.Count; i++)
            {
                var services = await servs[i].GetCharacteristicsAsync();
                isfindNotify = false;
                for (int j = 0; j < services.Count; j++)
                {
                    if (services[j].Properties == GattCharacteristicProperties.Notify)
                    {
                        gattCharacteristic = services[j];
                        gattCharacteristic.CharacteristicValueChanged += Chars_CharacteristicValueChanged;
                        await Task.Delay(500);
                        await gattCharacteristic.StartNotificationsAsync();
                        await Task.Delay(500);
                        isfindNotify = true;
                        break;
                    }
                }
                if (isfindNotify)
                    break;
            }
            if (!isfindNotify)
                await DisplayAlert("提示", "未找到可用服务特征", "确认");
        }
    }
    string recdata = string.Empty;
    private void Chars_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
    {
        recdata = Encoding.ASCII.GetString(e.Value);
        HandleData(recdata);
        DisplayData();
    }
    private void DisplayData()
    {
        if (MainThread.IsMainThread)
        {
            rec.Text =DateTime.Now.ToString("yyyyMMddHHmmss") +":"+ recdata;
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(DisplayData);
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        if(recordtype==2)
        {
            datacount++;
            SaveData(recdata);
        }
        else
        {
            IsSaveData = !IsSaveData;
            if (!IsSaveData)
            {
                datacount = 0;
                filename = string.Empty;
            }

            Button button = sender as Button;
            if (button != null)
            {
                button.BackgroundColor = IsSaveData ? Colors.Green : Colors.Transparent;
            }
        }
        
    }

    private async void ExportData_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShareDirectoryPage());
    }
}