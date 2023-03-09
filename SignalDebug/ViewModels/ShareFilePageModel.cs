using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalDebug.ViewModels
{
    public class ShareFilePageModel:ObservableObject
    {
        public ShareFilePageModel()
        {
            FileInfos = new List<Models.FileInfo>();
            OpenFileCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "OpenFile");
                },
                canExecute: () =>
                {
                    return true;
                }
            );
            ShareFileCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "ShareFile");
                },
                canExecute: () =>
                {
                    return true;
                }
            );
            DeleteFileCommand = new Command(
                execute: () =>
                {
                    MessagingCenter.Send(this, "DeleteFile");
                },
                canExecute: () =>
                {
                    return true;
                }
            );
        }
        public ICommand OpenFileCommand { private set; get; }
        public ICommand ShareFileCommand { private set; get; }
        public ICommand DeleteFileCommand { private set; get; }
        List<SignalDebug.Models.FileInfo> fileinfos = new List<SignalDebug.Models.FileInfo>();
        public List<SignalDebug.Models.FileInfo> FileInfos
        {
            set { SetProperty(ref fileinfos, value); }
            get { return fileinfos; }
        }
        SignalDebug.Models.FileInfo fileinfo = null;
        public SignalDebug.Models.FileInfo CurrentFileInfo
        {
            set { SetProperty(ref fileinfo, value); }
            get { return fileinfo; }
        }
    }
}
