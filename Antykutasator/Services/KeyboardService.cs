using System;
using Antykutasator.Helpers;

namespace Antykutasator.Services
{
    public class KeyboardService : IKeyboardService
    {
        public DateTime LastDeviceActivityTime => InterceptKeys.lastDeviceActivityTime;

        public KeyboardService()
        {
            //InterceptKeys.Start();
        }
    }
}
