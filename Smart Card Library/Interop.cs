using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace SmartCard.Library
{
    class Interop
    {
        [DllImport("WinScard.dll")]
        public static extern int SCardEstablishContext(
            uint dwScope,
            IntPtr notUsed1,
            IntPtr notUsed2,
            out IntPtr phContext);

        [DllImport("WinScard.dll")]
        public static extern int SCardReleaseContext(
            IntPtr phContext);

        [DllImport("WinScard.dll", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(
            IntPtr hContext,
            byte[] mszGroups,
            byte[] mszReaders,
            ref UInt32 pcchReaders);

        [DllImport("WinScard.dll", EntryPoint = "SCardConnect", CharSet = CharSet.Auto)]
        public static extern int SCardConnect(
            IntPtr hContext,
            [MarshalAs(UnmanagedType.LPTStr)] string szReader,
            UInt32 dwShareMode,
            UInt32 dwPreferredProtocol,
            out int phCard,
            out UInt32 pdwActiveProtocol);
    }
}
