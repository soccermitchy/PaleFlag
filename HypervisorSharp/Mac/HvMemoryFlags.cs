using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HypervisorSharp.Mac
{
    [Flags]
    [SuppressMessage("Design", "RCS1135:Declare enum member with zero value (when enum has FlagsAttribute).")]
    [SuppressMessage("Readability", "RCS1154:Sort enum members.")]
    public enum HvMemoryFlags : ulong
    {
        Read = 1 << 0,
        Write = 1 << 1,
        Exec = 1 << 2,
        RW = Read | Write,
        RX = Read | Exec,
        RWX = Read | Write | Exec
    }
}
