using System;

namespace Antykutasator.Services
{
    public interface IKeyboardService
    {
        DateTime LastDeviceActivityTime { get; }
    }
}
