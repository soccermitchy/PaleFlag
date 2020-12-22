using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HypervisorSharp.Mac
{
#pragma warning disable IDE1006 // Naming Styles
    internal static class MacWrapper
    {
        private const string FrameworkPath = "/System/Library/Frameworks/Hypervisor.framework/Hypervisor";

        internal static void HandleError(HvReturn ret)
        {
            switch (ret)
            {
                case HvReturn.Success:
                    return;
                case HvReturn.BadArgument:
                    throw new MacHvException("Bad Argument");
                case HvReturn.Busy:
                    throw new MacHvException("Busy");
                case HvReturn.Error:
                    throw new MacHvException("Unhelpful Error");
                case HvReturn.NoDevice:
                    throw new MacHvException("No Device");
                case HvReturn.NoResources:
                    throw new MacHvException("No Resources");
                case HvReturn.Unsupported:
                    throw new MacHvException("Unsupported");
                default:
                    throw new MacHvException("Unknown");
            }
        }

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vm_create(HvVmOptions flags);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vm_destroy();

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vm_map(UIntPtr uva, ulong gpa, UIntPtr size, HvMemoryFlags flags);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vm_unmap(ulong gpa, UIntPtr size);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vm_protect(ulong gpa, UIntPtr size, HvMemoryFlags flags);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vm_sync_tsc(ulong tsc);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_create(out uint vcpu, HvVcpuOptions flags);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_destroy(uint vcpu);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_read_register(uint vcpu, HvReg reg, out ulong value);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_write_register(uint vcpu, HvReg reg, ulong value);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_read_fpstate(uint vcpu, byte[] buffer, UIntPtr size);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_write_fpstate(uint vcpu, byte[] buffer, UIntPtr size);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_enable_native_msr(uint vcpu, uint msr, bool enable);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_read_msr(uint vcpu, uint msr, out ulong value);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_write_msr(uint vcpu, uint msr, ulong value);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_flush(uint vcpu);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_invalidate_tlb(uint vcpu);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_run(uint vcpu);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_interrupt(uint[] vcpus, uint vcpu_count);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vcpu_get_exec_time(uint vcpu, out ulong time);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vmx_vcpu_read_vmcs(uint vcpu, HvVmcsField field, out ulong value);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vmx_vcpu_write_vmcs(uint vcpu, HvVmcsField field, ulong value);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vmx_read_capability(HvVmxCapability field, out ulong value);

        [DllImport(FrameworkPath)]
        internal static extern HvReturn hv_vmx_vcpu_set_apic_address(uint vcpu, ulong gpa);
    }
}
