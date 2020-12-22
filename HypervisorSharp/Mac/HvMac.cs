using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace HypervisorSharp.Mac
{
    public class HvMac
    {
        public HvMac() => MacWrapper.HandleError(MacWrapper.hv_vm_create(HvVmOptions.Default));
        ~HvMac() => MacWrapper.HandleError(MacWrapper.hv_vm_destroy());

        private unsafe byte* AllocateAligned(ulong size)
        {
            var ptr = (ulong)Marshal.AllocHGlobal((IntPtr)size + 4095);
            if ((ptr & 0xFFF) != 0)
                ptr += 0x1000 - (ptr & 0xFFF);
            /*var tmp = (uint*) ptr;
			for(var i = 0; i < (uint) size / 4; i++)
				tmp[i] = 0x90c1010fU;*/
            return (byte*)ptr;
        }

        public unsafe byte* Map(ulong address, ulong size, HvMemoryFlags flags)
        {
            Debug.Assert(size < 0x80000000);
            var addr = AllocateAligned(size);
            MacWrapper.HandleError(MacWrapper.hv_vm_map((UIntPtr)addr, address, (UIntPtr)size, flags));
            return addr;
        }

        public void Unmap(ulong address, ulong size) => MacWrapper.HandleError(MacWrapper.hv_vm_unmap(address, (UIntPtr)size));

        public void Protect(ulong address, ulong size, HvMemoryFlags flags) => MacWrapper.HandleError(MacWrapper.hv_vm_protect(address, (UIntPtr)size, flags));

        public void SyncTsc(ulong tsc) => MacWrapper.HandleError(MacWrapper.hv_vm_sync_tsc(tsc));

        public HvMacVcpu CreateVcpu() => new HvMacVcpu();

        public ulong this[HvVmxCapability cap]
        {
            get
            {
                MacWrapper.HandleError(MacWrapper.hv_vmx_read_capability(cap, out var value));
                return value;
            }
        }
    }
}
