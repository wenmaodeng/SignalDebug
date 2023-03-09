using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalDebug.ViewModels
{
    public class ShareDirectoryModel: ObservableObject
    {
        public ShareDirectoryModel()
        {
            DirectoryInfos = new List<SignalDebug.Models.DirectoryInfo>();
            OpenDirectoryCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "OpenDirectory");
                },
                canExecute: () =>
                {
                    return true;
                }
            );
            DeleteDirectoryCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "DeleteDirectory");
                },
                canExecute: () =>
                {
                    return true;
                }
            );
        }
        public ICommand OpenDirectoryCommand { private set; get; }
        public ICommand DeleteDirectoryCommand { private set; get; }
        List<SignalDebug.Models.DirectoryInfo> directoryinfos = new List<SignalDebug.Models.DirectoryInfo>();
        public List<SignalDebug.Models.DirectoryInfo> DirectoryInfos
        {
            set { SetProperty(ref directoryinfos, value); }
            get { return directoryinfos; }
        }
        SignalDebug.Models.DirectoryInfo currentDirectoryInfo = null;
        public SignalDebug.Models.DirectoryInfo CurrentDirectoryInfo
        {
            set { SetProperty(ref currentDirectoryInfo, value); }
            get { return currentDirectoryInfo; }
        }
    }
}
