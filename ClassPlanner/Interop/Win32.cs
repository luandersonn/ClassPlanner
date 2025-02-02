using System;
using System.Runtime.InteropServices;

namespace ClassPlanner.Interop;

internal static partial class Win32
{
    [LibraryImport("user32.dll")]
    internal static partial IntPtr GetActiveWindow();
}