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
