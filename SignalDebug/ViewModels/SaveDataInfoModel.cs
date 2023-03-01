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
    public class SaveDataInfoModel : ObservableObject
    {
        public ICommand SaveDataInfoCommand { private set; get; }
        public ICommand AddSignalInfoCommand { private set; get; }
        public ICommand EditSignalInfoCommand { private set; get; }
        public ICommand DeleteSignalInfoCommand { private set; get; }
        public string Title { get; set; }
        List<SignalInfo> signalInfos = new List<SignalInfo>();
        public List<SignalInfo> SignalInfos
        {
            set { SetProperty(ref signalInfos, value); }
            get { return signalInfos; }
        }
        SignalInfo signalInfo = new SignalInfo();
        public SignalInfo SignalInfo
        {
            set { SetProperty(ref signalInfo, value); }
            get { return signalInfo; }
        }
        DataInfo dataInfo = new DataInfo();
        public DataInfo DataInfo
        {
            set { SetProperty(ref dataInfo, value); }
            get { return dataInfo; }
        }
        public DataSignalDatabase dataSignalDatabase;
        public SaveDataInfoModel(DataSignalDatabase _dataSignalDatabase)
        {
            dataSignalDatabase = _dataSignalDatabase;
            SaveDataInfoCommand = new Command(
                execute: async () =>
                {
                    await dataSignalDatabase.SaveDataInfoAsync(DataInfo);
                },
                canExecute: () =>
                {
                    return true;
                });
            AddSignalInfoCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "AddSignalInfo");
                },
                canExecute: () =>
                {
                    return true;
                });
            EditSignalInfoCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "EditSignalInfo");
                },
                canExecute: () =>
                {
                    return true;
                });
            DeleteSignalInfoCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "DeleteSignalInfo");
                },
                canExecute: () =>
                {
                    return true;
                });
        }
        public async void InitData()
        {
            if (!string.IsNullOrEmpty(DataInfo.DataId))
            {
                SignalInfos = await dataSignalDatabase.GetSignalInfosAsync(DataInfo.DataId);
            }
            else
            {
                SignalInfos = new List<SignalInfo>();
            }
        }
        public async void DeleteSignalInfo()
        {
            await dataSignalDatabase.DeleteSignalInfoAsync(SignalInfo);
            SignalInfo = new SignalInfo();
        }
    }
}
