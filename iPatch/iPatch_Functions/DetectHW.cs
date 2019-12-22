using System;
using System.Runtime.InteropServices;
/// <summary>
/// INCOMPATIBLE UNTIL .NET 5 DO NOT USE
/// </summary>
namespace iPatch.iPatch_Functions
{
    class DetectHW
    {
        public DetectHW()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Linux();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                MacOS();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                WinNT();
            else
                throw new Exception("Bruh, WTF are you using?");

        }
        private static void WinNT()//Winders
        {

        }
        private static void MacOS()//crackOS
        {

        }
        private static void Linux()//loonix
        {

        }
    }
}
