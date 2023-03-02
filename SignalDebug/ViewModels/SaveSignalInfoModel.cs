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
    internal class SaveSignalInfoModel : ObservableObject
    {
        public ICommand SaveSignalInfoCommand { private set; get; }

        public string Title { get; set; }
        SignalInfo signalInfo = new SignalInfo();

        List<string> items = new List<string> { "布尔类型", "浮点型", "整形" };
        public List<string> Items
        { get { return items; } }
        public SignalInfo SignalInfo
        {
            set { SetProperty(ref signalInfo, value); }
            get { return signalInfo; }
        }
        DataSignalDatabase dataSignalDatabase;
        public SaveSignalInfoModel(DataSignalDatabase _dataSignalDatabase)
        {
            dataSignalDatabase = _dataSignalDatabase;
            SaveSignalInfoCommand = new Command(
                execute: async () =>
                {
                    int count = await dataSignalDatabase.SaveSignalInfoAsync(SignalInfo);
                    if (count > 0)
                        MessagingCenter.Send(this, "SaveSignalInfo", "保存成功");
                    else
                        MessagingCenter.Send(this, "SaveSignalInfo", "保存失败");
                },
                canExecute: () =>
                {
                    return true;
                });
        }
    }
}
