using InTheHand.Bluetooth;
using Microsoft.Maui.Graphics;
using SignalDebug.ViewModels;
using System.Diagnostics;
using System.Text;
using YunBang.MAUI.UI.OutDisplay;

namespace SignalDebug.Views;

public partial class DataDebugPage : ContentPage
{
    StreamWriter streamWriter = null;
    /// <summary>
    /// 文件目录
    /// </summary>
    string path = string.Empty;
    /// <summary>
    /// 文件名称（含路径）
    /// </summary>
    string filename = string.Empty;
    /// <summary>
    /// 当前记录的数据数量
    /// </summary>
    int datacount = 0;

    /// <summary>
    /// 0 记录全部数据 1 记录全部信号列表数据 2 记录当前信号列表数据
    /// </summary>
    int recordtype = 2;
    /// <summary>
    /// 是否记录数据
    /// </summary>
    bool IsSaveData = false;
    /// <summary>
    /// 连接的蓝牙设备信息
    /// </summary>
    BluetoothDevice device = null;
    /// <summary>
    /// 蓝牙Notify特征
    /// </summary>
    GattCharacteristic gattCharacteristic = null;
    /// <summary>
    /// 信号控件集合
    /// </summary>
    private List<Element> contentViews = new List<Element>();
    /// <summary>
    /// 页面绑定的数据源信息
    /// </summary>
    private DataDebugModel dataDebugModel = null;
    public DataDebugPage()
    {
        InitializeComponent();
        ConnectBluetooth();
    }
    private void SaveData(string data)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            if (recordtype == 2)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true))
                {
                    file.WriteLine(data);
                }
            }
            else
            {
                if (streamWriter != null)
                {
                    streamWriter.WriteLine(data);
                }
            }
            datacount++;
            if (MainThread.IsMainThread)
            {
                RecordData.Text = $"记录【{datacount}】";
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => { RecordData.Text = $"记录【{datacount}】"; });
            }
        }
        catch
        {

        }
    }
    protected override void OnAppearing()
    {
        InitCintrol();
        picker.SelectedIndex = 2;
        path = FileSystem.Current.AppDataDirectory + "/currentsignals/" + DateTime.Now.ToString("yyyy_MM_dd");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string fn = DateTime.Now.ToString("yyyyMMddHHmmss") + $"_{dataDebugModel.DataInfo.DataName}.txt";
        filename = Path.Combine(path, fn);
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
        if (tempstrs.Length < 2)
            return;
        if (dataDebugModel != null)
        {
            if (dataDebugModel.DataInfo.Lenth != tempstrs.Length)
                return;
            if (tempstrs[0] != dataDebugModel.DataInfo.FrameHead)
                return;
            if (IsSaveData)
            {
                if (recordtype == 0)
                {
                    data = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + data;
                    SaveData(data);
                }
                else if (recordtype == 1)
                {
                    data = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + GetSignals(tempstrs);
                    SaveData(data);
                }
            }
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
        if (streamWriter != null)
        {
            streamWriter?.Close();
            streamWriter?.Dispose();
            streamWriter = null;
        }
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
            rec.Text = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + recdata;
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(DisplayData);
        }
    }
    /// <summary>
    /// 拼接配置的信号字符串
    /// </summary>
    /// <param name="recdata"></param>
    /// <returns></returns>
    private string GetSignals(string[] recdata)
    {
        string temp = string.Empty;
        dataDebugModel.SignalInfos.ForEach(info =>
        {

            temp += $"{info.SignalName}:{recdata[info.SignalBit]},";
        });
        return temp;
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        if (recordtype == 2)
        {
            recdata = "1,2,3,4,5,6,7,8,9,d,w,m";
            //string[] tempstrs = recdata.Split(',');
            //if (tempstrs.Length < 2)
            //    return;
            //if (dataDebugModel == null)
            //    return;
            //if (dataDebugModel.DataInfo.Lenth != tempstrs.Length)
            //    return;
            //if (tempstrs[0] != dataDebugModel.DataInfo.FrameHead)
            //    return;
            //recdata = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + GetSignals(tempstrs);
            recdata = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + recdata;
            SaveData(recdata);
        }
        else
        {
            IsSaveData = !IsSaveData;
            if (!IsSaveData)
            {
                picker.IsEnabled = true;
                streamWriter?.Close();
                streamWriter?.Dispose();
                streamWriter = null;
            }
            else
            {
                picker.IsEnabled = false;
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
        string temp = FileSystem.Current.AppDataDirectory;
        if (recordtype == 0)
        {
            temp = FileSystem.Current.AppDataDirectory + "/alldatas/";
        }
        else if (recordtype == 1)
        {
            temp = FileSystem.Current.AppDataDirectory + "/allsignals/";
        }
        else if (recordtype == 2)
        {
            temp = FileSystem.Current.AppDataDirectory + "/currentsignals/";
        }
        await Navigation.PushAsync(new ShareDirectoryPage { DirectoryPath = temp });
    }

    private void picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (recordtype != picker.SelectedIndex)
        {
            datacount = 0;
            streamWriter?.Close();
            streamWriter?.Dispose();
            streamWriter = null;
            recordtype = picker.SelectedIndex;
            if (recordtype == 0)
            {
                path = FileSystem.Current.AppDataDirectory + "/alldatas/" + DateTime.Now.ToString("yyyy_MM_dd");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fn = DateTime.Now.ToString("yyyyMMddHHmmss") + $"_{dataDebugModel.DataInfo.DataName}.txt";
                filename = Path.Combine(path, fn);
                streamWriter = new StreamWriter(filename);
            }
            else if (recordtype == 1)
            {
                path = FileSystem.Current.AppDataDirectory + "/allsignals/" + DateTime.Now.ToString("yyyy_MM_dd");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fn = DateTime.Now.ToString("yyyyMMddHHmmss") + $"_{dataDebugModel.DataInfo.DataName}.txt";
                filename = Path.Combine(path, fn);
                streamWriter = new StreamWriter(filename);
            }
            else if (recordtype == 2)
            {
                path = FileSystem.Current.AppDataDirectory + "/currentsignals/" + DateTime.Now.ToString("yyyy_MM_dd");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fn = DateTime.Now.ToString("yyyyMMddHHmmss") + $"_{dataDebugModel.DataInfo.DataName}.txt";
                filename = Path.Combine(path, fn);
            }
        }
    }
}