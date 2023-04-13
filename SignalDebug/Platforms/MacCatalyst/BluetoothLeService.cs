using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDebug.Services
{
    public partial class BluetoothLeService
    {
        public async Task<bool> CheckAndRequestBluetoothPermission()
        {
            await Task.Delay(1000);
            return false;
        }
    }
}
