using System;
using System.Collections.Generic;
using System.Text;

namespace HypervisorSharp.Mac
{
    public class HvMacVcpu
    {
        private readonly uint Vcpu;

        internal HvMacVcpu() => MacWrapper.HandleError(MacWrapper.hv_vcpu_create(out Vcpu, HvVcpuOptions.Default));
        ~HvMacVcpu() => MacWrapper.HandleError(MacWrapper.hv_vcpu_destroy(Vcpu));

        public ulong this[HvReg reg]
        {
            get
            {
                MacWrapper.HandleError(MacWrapper.hv_vcpu_read_register(Vcpu, reg, out var value));
                return value;
            }
            set => MacWrapper.HandleError(MacWrapper.hv_vcpu_write_register(Vcpu, reg, value));
        }

        public ulong this[HvVmcsField field]
        {
            get
            {
                MacWrapper.HandleError(MacWrapper.hv_vmx_vcpu_read_vmcs(Vcpu, field, out var value));
                return value;
            }
            set => MacWrapper.HandleError(MacWrapper.hv_vmx_vcpu_write_vmcs(Vcpu, field, value));
        }

        public void Enter() => MacWrapper.HandleError(MacWrapper.hv_vcpu_run(Vcpu));
        public void Flush() => MacWrapper.HandleError(MacWrapper.hv_vcpu_flush(Vcpu));
        public void InvalidateTlb() => MacWrapper.HandleError(MacWrapper.hv_vcpu_invalidate_tlb(Vcpu));
    }
}
