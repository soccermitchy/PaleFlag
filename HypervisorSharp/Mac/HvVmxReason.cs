﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HypervisorSharp.Mac
{
    public enum HvVmxReason
    {
        EXC_NMI = 0,
        IRQ = 1,
        TRIPLE_FAULT = 2,
        INIT = 3,
        SIPI = 4,
        IO_SMI = 5,
        OTHER_SMI = 6,
        IRQ_WND = 7,
        VIRTUAL_NMI_WND = 8,
        TASK = 9,
        CPUID = 10,
        GETSEC = 11,
        HLT = 12,
        INVD = 13,
        INVLPG = 14,
        RDPMC = 15,
        RDTSC = 16,
        RSM = 17,
        VMCALL = 18,
        VMCLEAR = 19,
        VMLAUNCH = 20,
        VMPTRLD = 21,
        VMPTRST = 22,
        VMREAD = 23,
        VMRESUME = 24,
        VMWRITE = 25,
        VMOFF = 26,
        VMON = 27,
        MOV_CR = 28,
        MOV_DR = 29,
        IO = 30,
        RDMSR = 31,
        WRMSR = 32,
        VMENTRY_GUEST = 33,
        VMENTRY_MSR = 34,
        MWAIT = 36,
        MTF = 37,
        MONITOR = 39,
        PAUSE = 40,
        VMENTRY_MC = 41,
        TPR_THRESHOLD = 43,
        APIC_ACCESS = 44,
        VIRTUALIZED_EOI = 45,
        GDTR_IDTR = 46,
        LDTR_TR = 47,
        EPT_VIOLATION = 48,
        EPT_MISCONFIG = 49,
        EPT_INVEPT = 50,
        RDTSCP = 51,
        VMX_TIMER_EXPIRED = 52,
        INVVPID = 53,
        WBINVD = 54,
        XSETBV = 55,
        APIC_WRITE = 56,
        RDRAND = 57,
        INVPCID = 58,
        VMFUNC = 59,
        RDSEED = 61,
        XSAVES = 63,
        XRSTORS = 64
    }
}
