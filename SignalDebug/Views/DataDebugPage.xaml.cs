using InTheHand.Bluetooth;
using Microsoft.Maui.Graphics;
using SignalDebug.ViewModels;
using System.Diagnostics;
using System.Text;
using YunBang.MAUI.UI.OutDisplay;

namespace SignalDebug.Views;

public partial class DataDebugPage : ContentPage
{
    static string path = FileSystem.Current.AppDataDirectory + "\\record\\" + DateTime.Now.ToString("yyyy_MM_dd");
    static string alldatafilename = string.Empty;
    static string allsignaldatafilename = string.Empty;
    static string currentsignaldatafilename = string.Empty;
    int alldatacount = 0;
    int allsignaldatacount = 0;
    int currentsignaldatacount = 0;
    GattCharacteristic gattCharacteristic = null;
    static bool IsSaveAllData = false;
    static bool IsSaveAllSignalData = false;
    BluetoothDevice device = null;
    private static List<Element> contentViews = new List<Element>();
    private static DataDebugModel dataDebugModel = null;
    public DataDebugPage()
	{
		InitializeComponent();
        ConnectBluetooth();
    }
    private void ss()
    {
        dataDebugModel = this.BindingContext as DataDebugModel;
        contentViews.Clear();
        verticalStackLayout.Children.Clear();

        if (temp != null && dataDebugModel.SignalInfos != null && dataDebugModel.SignalInfos.Count > 0)
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
            //set();
            timer = new Timer(new TimerCallback((s) => set()), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(200));
        }
    }
    Timer timer = null;

    private void set()
    {
        var random = new Random();
        string tempstr = "3,4,5,6,7,8,4,3,8,1,2,3,2,1,2,3,2,1";

        string[] tempstrs = tempstr.Split(',');
        for (int i = 0; i < tempstrs.Length; i++)
        {
            tempstrs[i] = random.Next(20).ToString();
        }
        if (IsSaveAllData)
        {

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (string.IsNullOrEmpty(alldatafilename))
            {
                string fn = DateTime.Now.ToString("yyyyMMddHHmmss") + $"_{dataDebugModel.DataInfo.DataName}.txt";
                alldatafilename = Path.Combine(path, fn);
            }
            string ssss = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + string.Join(",", tempstrs);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(alldatafilename, true))
            {
                file.WriteLine(ssss);
                alldatacount++;
                if (MainThread.IsMainThread)
                {
                    allbutton.Text = $"记录全部数据{alldatacount}";
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() => { allbutton.Text = $"记录全部数据{alldatacount}"; });
                }

            }
        }


        if (dataDebugModel != null)
        {
            dataDebugModel.SignalInfos.ForEach(s =>
            {
                string value = tempstrs[s.SignalBit];
                var sss = contentViews.FirstOrDefault(x => x.ClassId == s.SignalId);
                switch (s.DataType)
                {
                    case Models.DataType.BoolSignal:
                        break;
                    case Models.DataType.FloatSignal:
                        TextDisplay textShowFloatSignal = sss as TextDisplay;
                        textShowFloatSignal.TextValue = value;
                        break;
                    case Models.DataType.IntSignal:
                        TextDisplay textShowFloatIntSignal = sss as TextDisplay;
                        textShowFloatIntSignal.TextValue = value;
                        break;
                    default:
                        break;
                }
            });
        }

    }

    private void set(string temp)
    {
        string[] tempstrs = temp.Split(',');
        if (dataDebugModel != null)
        {
            dataDebugModel.SignalInfos.ForEach(s =>
            {
                string value = tempstrs[s.SignalBit];
                var sss = contentViews.FirstOrDefault(x => x.ClassId == s.SignalId);
                switch (s.DataType)
                {
                    case Models.DataType.BoolSignal:
                        break;
                    case Models.DataType.FloatSignal:
                        TextDisplay textShowFloatSignal = sss as TextDisplay;
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
                        TextDisplay textShowFloatIntSignal = sss as TextDisplay;
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
    protected override void OnAppearing()
    {


        ss();
        base.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        timer?.Dispose();
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
            await DisplayAlert("提示", "未找到可用服务特征", "确认");
        }
    }
    string temp = string.Empty;
    private void Chars_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
    {
        var redata = Encoding.ASCII.GetString(e.Value);
        set(redata);

        Debug.WriteLine(redata);
        temp = DateTime.Now.ToString() + "：" + redata;
        setstr();
    }
    private void setstr()
    {
        if (MainThread.IsMainThread)
        {
            rec.Text = temp;
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(setstr);
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        IsSaveAllData = !IsSaveAllData;
        if (!IsSaveAllData)
        {
            alldatacount = 0;
            alldatafilename = string.Empty;
        }

        Button button = sender as Button;
        if (button != null)
        {
            button.BackgroundColor = IsSaveAllData ? Colors.Green : Colors.Gray;
        }
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        IsSaveAllSignalData = !IsSaveAllSignalData;
        if (!IsSaveAllSignalData)
            allsignaldatafilename = string.Empty;
        Button button = sender as Button;
        if (button != null)
        {
            button.BackgroundColor = IsSaveAllSignalData ? Colors.Green : Colors.Gray;
        }
    }

    private async void allbutton2_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShareDirectoryPage());
    }
}