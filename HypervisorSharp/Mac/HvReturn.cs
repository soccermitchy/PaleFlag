using System;
using System.Collections.Generic;
using System.Text;

namespace HypervisorSharp.Mac
{
    internal enum HvReturn : uint
    {
        Success = 0,
        Error = 0xFAE94001,
        Busy = 0xFAE94002,
        BadArgument = 0xFAE94003,
        NoResources = 0xFAE94005,
        NoDevice = 0xFAE94006,
        Unsupported = 0xFAE9400F
    }
}
