using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices; 

namespace SignPPFv1
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("0BE687F5-2EB7-434e-A06A-02CA08CF6E12")]
    interface ISignPPFv1
    {
        string GetText();

        string Signar2(string xmlIn, string node, bool p1, bool p2);

        string Signar2v1(string xmlIn);

        string BuscaNomCert();

        string BuscaDataExpired();

        string CerrarStore();
    }
}

