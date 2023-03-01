using CommunityToolkit.Mvvm.ComponentModel;
using SignalDebug.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SignalDebug.ViewModels
{
    public class DataDebugModel: ObservableObject
    {
        DataInfo datainfo = new DataInfo();
        public DataInfo DataInfo
        {
            set { SetProperty(ref datainfo, value); }
            get { return datainfo; }
        }
        List<SignalInfo> signalInfos = new List<SignalInfo>();
        public List<SignalInfo> SignalInfos
        {
            set { SetProperty(ref signalInfos, value); }
            get { return signalInfos; }
        }
    }
}
