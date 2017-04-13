using System;

namespace Antykutasator.Services
{
    public interface IMouseService
    {
        DateTime LastDeviceActivityTime { get; }
    }
}
