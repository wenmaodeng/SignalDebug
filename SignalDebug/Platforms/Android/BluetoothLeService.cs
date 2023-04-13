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
            var status = await Permissions.CheckStatusAsync<BluetoothPermissions>();

            if (status == PermissionStatus.Granted)
                return true;
            status = await Permissions.RequestAsync<BluetoothPermissions>();

            if (status == PermissionStatus.Granted)
                return true;
            return false;
        }
        private class BluetoothPermissions : Permissions.BasePlatformPermission
        {
            public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
                new List<(string androidPermission, bool isRuntime)>
                {
                    (global::Android.Manifest.Permission.AccessFineLocation, true),
                    (global::Android.Manifest.Permission.AccessCoarseLocation, true),
                    (global::Android.Manifest.Permission.AccessNetworkState, true),
                    (global::Android.Manifest.Permission.Internet, true),
                    (global::Android.Manifest.Permission.Bluetooth, true),
                    (global::Android.Manifest.Permission.BluetoothAdmin, true),
                }.ToArray();
        }
    }
}
