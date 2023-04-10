using CommunityToolkit.Mvvm.ComponentModel;
using SignalDebug.Models;
using SignalDebug.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalDebug.ViewModels
{
    public class DataInfoListModel : ObservableObject
    {
        DataInfo dataInfo = new DataInfo();
        public DataInfo DataInfo
        {
            set { SetProperty(ref dataInfo, value); }
            get { return dataInfo; }
        }
        List<DataInfo> dataInfos = new List<DataInfo>();
        public List<DataInfo> DataInfos
        {
            set { SetProperty(ref dataInfos, value); }
            get { return dataInfos; }
        }
        public ICommand AddDataInfoCommand { private set; get; }
        public ICommand EditDataInfoCommand { private set; get; }
        public ICommand DeleteDataInfoCommand { private set; get; }

        DataSignalDatabase dataSignalDatabase;
        public DataInfoListModel(DataSignalDatabase _DataSignalDatabase)
        {
            dataSignalDatabase = _DataSignalDatabase;
            AddDataInfoCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "AddDataInfo");
                },
                canExecute: () =>
                {
                    return true;
                }
            );
            EditDataInfoCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "EditDataInfo");
                },
                canExecute: () =>
                {
                    return true;
                }
            );
            DeleteDataInfoCommand = new Command(

                execute: () =>
                {
                    MessagingCenter.Send(this, "DeleteDataInfo");
                },
                canExecute: () =>
                {
                    return true;
                });
        }
        public async void InitData()
        {
            DataInfos = await dataSignalDatabase.GetDataInfosAsync();

            if(DataInfos==null|| DataInfos.Count==0|| !DataInfos.Any(d=>d.DataName=="驾考GPS"))
            {
                DataInfo dataInfo = new DataInfo();
                dataInfo.DataName = "驾考GPS";
                dataInfo.FrameHead = "$KSXT";
                dataInfo.Lenth = 23;
                int count= await dataSignalDatabase.SaveDataInfoAsync(dataInfo);
                DataInfos = await dataSignalDatabase.GetDataInfosAsync();
                dataInfo = DataInfos.FirstOrDefault(d => d.DataName == "驾考GPS");
                if (dataInfo == null)
                    return;
                SignalInfo signalInfo = new SignalInfo();
                signalInfo.DataId = dataInfo.DataId;
                signalInfo.SignalName = "东向坐标";
                signalInfo.SignalBit = 14;
                signalInfo.Sort = 1;
                signalInfo.Unit = "米";
                signalInfo.Enabled = true;
                signalInfo.DataType = DataType.FloatSignal;
                count = await dataSignalDatabase.SaveSignalInfoAsync(signalInfo);
                signalInfo = new SignalInfo();
                signalInfo.DataId = dataInfo.DataId;
                signalInfo.SignalName = "北向坐标";
                signalInfo.SignalBit = 15;
                signalInfo.Sort = 2;
                signalInfo.Unit = "米";
                signalInfo.Enabled = true;
                signalInfo.DataType = DataType.FloatSignal;
                count = await dataSignalDatabase.SaveSignalInfoAsync(signalInfo);
                signalInfo = new SignalInfo();
                signalInfo.DataId = dataInfo.DataId;
                signalInfo.SignalName = "定位状态";
                signalInfo.SignalBit = 10;
                signalInfo.Sort = 3;
                signalInfo.Unit = "";
                signalInfo.Enabled = true;
                signalInfo.DataType = DataType.IntSignal;
                count = await dataSignalDatabase.SaveSignalInfoAsync(signalInfo);
                signalInfo = new SignalInfo();
                signalInfo.DataId = dataInfo.DataId;
                signalInfo.SignalName = "定向状态";
                signalInfo.SignalBit = 11;
                signalInfo.Sort = 4;
                signalInfo.Unit = "";
                signalInfo.Enabled = true;
                signalInfo.DataType = DataType.IntSignal;
                count = await dataSignalDatabase.SaveSignalInfoAsync(signalInfo);
                signalInfo = new SignalInfo();
                signalInfo.DataId = dataInfo.DataId;
                signalInfo.SignalName = "前天线星数";
                signalInfo.SignalBit = 12;
                signalInfo.Sort = 5;
                signalInfo.Unit = "";
                signalInfo.Enabled = true;
                signalInfo.DataType = DataType.IntSignal;
                count = await dataSignalDatabase.SaveSignalInfoAsync(signalInfo);
                signalInfo = new SignalInfo();
                signalInfo.DataId = dataInfo.DataId;
                signalInfo.SignalName = "后天线星数";
                signalInfo.SignalBit = 13;
                signalInfo.Sort = 6;
                signalInfo.Unit = "";
                signalInfo.Enabled = true;
                signalInfo.DataType = DataType.IntSignal;
                count = await dataSignalDatabase.SaveSignalInfoAsync(signalInfo);
                DataInfos = await dataSignalDatabase.GetDataInfosAsync();
            }
        }
        public async void DeleteDataInfo()
        {
            await dataSignalDatabase.DeleteDataInfoAsync(DataInfo);
            DataInfo = new DataInfo();
        }
        public async Task<DataInfo> GetDataInfo(string dataId)
        {
            return await dataSignalDatabase.GetDataInfoAsync(dataId);
        }
        public async Task<List<SignalInfo>> GetSignalInfos(string dataId)
        {
            return await dataSignalDatabase.GetSignalInfosAsync(dataId);
        }
    }
}
