using System.Runtime.InteropServices;

namespace Antykutasator.Helpers
{
    public static class NativeMethodsHelper
    {
        [DllImport("user32")]
        public static extern void LockWorkStation();

        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

    }
}
