using System;
using System.Collections.Generic;
using System.Text;

namespace HypervisorSharp.Mac
{
    public enum HvVmxCapability
    {
        PINBASED = 0,         // pin-based VMX capabilities
        PROCBASED = 1,        // primary proc.-based VMX capabilities
        PROCBASED2 = 2,       // second. proc.-based VMX capabilities
        ENTRY = 3,            // VM-entry VMX capabilities
        EXIT = 4,             // VM-exit VMX capabilities
        PREEMPTION_TIMER = 32 // VMX preemption timer frequency
    }
}
