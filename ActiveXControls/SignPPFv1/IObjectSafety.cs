using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SignPPFv1
{
        [ComImport()]
        [Guid("1648EF2E-9A51-4937-89C3-5C04282A5293")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IObjectSafety
        {
            [PreserveSig()]
            int GetInterfaceSafetyOptions(ref Guid riid, out int pdwSupportedOptions, out int pdwEnabledOptions);

            [PreserveSig()]
            int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions);
        }
}
