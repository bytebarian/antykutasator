using System;
using Antykutasator.Helpers;

namespace Antykutasator.Services
{
    public class MouseService : IMouseService
    {
        public DateTime LastDeviceActivityTime => InterceptMouse.lastDeviceActivityTame;

        public MouseService()
        {
            //InterceptMouse.Start();
        }
    }
}
