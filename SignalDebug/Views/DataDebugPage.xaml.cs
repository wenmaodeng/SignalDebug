using InTheHand.Bluetooth;
using Microsoft.Maui.Graphics;
using SignalDebug.ViewModels;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using YunBang.MAUI.UI.OutDisplay;

namespace SignalDebug.Views;

public partial class DataDebugPage : ContentPage
{
    /// <summary>
    /// �ļ�Ŀ¼
    /// </summary>
    string path = FileSystem.Current.AppDataDirectory + "/datas/" + DateTime.Now.ToString("yyyy_MM_dd");
    /// <summary>
    /// �ļ����ƣ���·����
    /// </summary>
    string filename = string.Empty;
    /// <summary>
    /// ��ǰ��¼����������
    /// </summary>
    int datacount = 0;
    /// <summary>
    /// ���ӵ������豸��Ϣ
    /// </summary>
    BluetoothDevice device = null;
    /// <summary>
    /// ����Notify����
    /// </summary>
    List<GattCharacteristic> GattCharacteristics = new List<GattCharacteristic>();
    /// <summary>
    /// �źſؼ�����
    /// </summary>
    private List<Element> contentViews = new List<Element>();
    /// <summary>
    /// ҳ��󶨵�����Դ��Ϣ
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
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true))
            {
                file.WriteLine(data);
            }
            datacount++;
            if (MainThread.IsMainThread)
            {
                RecordData.Text = $"��¼��{datacount}��";
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => { RecordData.Text = $"��¼��{datacount}��"; });
            }
        }
        catch
        {

        }
    }
    protected override void OnAppearing()
    {
        InitCintrol();
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
                //if (dataDebugModel.SignalInfos[i].Enabled)
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

    /// <summary>
    /// ��������
    /// </summary>
    private void HandleData()
    {
        try
        {
            if (alldata.IsEmpty)
                return;
            if (dataDebugModel == null)
                return;
            while (alldata.TryPeek(out string temp))
            {
                try
                {
                    if (temp != dataDebugModel.DataInfo.FrameHead)
                    {
                        alldata.TryDequeue(out temp);
                        continue;
                    }
                        
                    if (alldata.Count < dataDebugModel.DataInfo.Lenth)
                    {
                        break;
                    }
                    List<string> tempstrs = new List<string>();
                    for(int i=0;i< dataDebugModel.DataInfo.Lenth;i++)
                    {
                        alldata.TryDequeue(out temp);
                        tempstrs.Add(temp);
                    }

                    dataDebugModel.SignalInfos.ForEach(s =>
                    {
                        if (s.SignalBit < tempstrs.Count - 1)
                        {
                            string value = tempstrs[s.SignalBit];
                            var element = contentViews.FirstOrDefault(x => x.ClassId == s.SignalId);
                            switch (s.DataType)
                            {
                                case Models.DataType.BoolSignal:
                                    break;
                                case Models.DataType.FloatSignal:
                                    TextDisplay textShowFloatSignal = element as TextDisplay;
                                    if (textShowFloatSignal != null)
                                    {
                                        if (MainThread.IsMainThread)
                                        {
                                            textShowFloatSignal.TextValue = value;
                                        }
                                        else
                                        {
                                            MainThread.BeginInvokeOnMainThread(() => { textShowFloatSignal.TextValue = value; });
                                        }
                                    }
                                    break;
                                case Models.DataType.IntSignal:
                                    TextDisplay textShowFloatIntSignal = element as TextDisplay;
                                    if (textShowFloatIntSignal != null)
                                    {
                                        if (MainThread.IsMainThread)
                                        {
                                            textShowFloatIntSignal.TextValue = value;
                                        }
                                        else
                                        {
                                            MainThread.BeginInvokeOnMainThread(() => { textShowFloatIntSignal.TextValue = value; });
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    });
                }
                catch
                { }
            }
        }
        catch
        {

        }
    }
    protected override void OnDisappearing()
    {
        if (GattCharacteristics != null&& GattCharacteristics.Count>0)
        {
            GattCharacteristics.ForEach(gatt =>
            {
                gatt.CharacteristicValueChanged -= Chars_CharacteristicValueChanged;
            });
            GattCharacteristics = new List<GattCharacteristic>();
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
                await DisplayAlert("��ʾ", "Ӳ��������", "ȷ��");
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
                if (count >= 5)
                {
                    await DisplayAlert("��ʾ", "��������ʧ��", "ȷ��");
                    return;
                }
            }
            device.Gatt.RequestMtu(512);
            var servs = await device.Gatt.GetPrimaryServicesAsync();
            int findNotifyCount = 0;
            for (int i = 0; i < servs.Count; i++)
            {
                var services = await servs[i].GetCharacteristicsAsync();
                for (int j = 0; j < services.Count; j++)
                {
                    var GattCharacteristicProperty = services[j].Properties & GattCharacteristicProperties.Notify;
                    if (GattCharacteristicProperty == GattCharacteristicProperties.Notify)
                    {
                        GattCharacteristics.Add(services[j]);
                        services[j].CharacteristicValueChanged += Chars_CharacteristicValueChanged;
                        await Task.Delay(500);
                        await services[j].StartNotificationsAsync();
                        await Task.Delay(500);
                        findNotifyCount++;
                        break;
                    }
                }
            }
            if (findNotifyCount == 0)
                await DisplayAlert("��ʾ", "δ�ҵ����÷�������", "ȷ��");
        }
    }
    string recdata = string.Empty;
    string rightdata = string.Empty;
    ConcurrentQueue<string> alldata = new ConcurrentQueue<string>();
    string alldatastring = string.Empty;
    private void Chars_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
    {
        recdata = Encoding.ASCII.GetString(e.Value);
        DisplayData();
        if (dataDebugModel == null)
            return;
        alldatastring += recdata;
        if (alldatastring.IndexOf(dataDebugModel.DataInfo.FrameHead) == 0)
        {
            string[] tempstrs = alldatastring.Split(',');
            if (tempstrs.Length < dataDebugModel.DataInfo.Lenth)
                return;
            rightdata = alldatastring;
            for (int i = 0; i < dataDebugModel.DataInfo.Lenth; i++)
            {
                alldata.Enqueue(tempstrs[i]);
            }
            HandleData();
            alldatastring = string.Empty;
        }
        else
        {
            alldatastring = string.Empty;
        }
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
    /// ƴ�����õ��ź��ַ���
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
        if (!string.IsNullOrEmpty(temp))
        {
            temp = temp.TrimEnd(',');
        }
        return temp;
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        string[] tempstrs = rightdata.Split(',');
        if (tempstrs.Length < 2)
            return;
        if (dataDebugModel == null)
            return;
        if (dataDebugModel.DataInfo.Lenth != tempstrs.Length)
            return;
        if (tempstrs[0] != dataDebugModel.DataInfo.FrameHead)
            return;
        string rightrecdata = string.Empty;
        if (dataDebugModel.RecoredType == 1)
        {
            rightrecdata = DateTime.Now.ToString("yyyyMMddHHmmss") + $"��{datacount + 1}��:" + GetSignals(tempstrs);
        }
        else
        {
            rightrecdata = DateTime.Now.ToString("yyyyMMddHHmmss") + $"��{datacount + 1}��:" + rightdata;
        }
        SaveData(rightrecdata);
    }

    private async void ExportData_Clicked(object sender, EventArgs e)
    {
        string temp = FileSystem.Current.AppDataDirectory + "/datas/";
        var directorys = Directory.GetDirectories(temp);
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

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (dataDebugModel != null)
        {
            if (e.Value)
            {
                dataDebugModel.RecoredType = 0;
            }
            else
            {
                dataDebugModel.RecoredType = 1;
            }
        }
    }
}