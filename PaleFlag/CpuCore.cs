﻿using System;
using System.Diagnostics;
using HypervisorSharp;
using PaleFlag.XboxKernel;
using static System.Console;

namespace PaleFlag {
	public unsafe class CpuCore {
		const uint PagetableAddr = 0xF0000000;
		
		readonly HvMac Hv = new HvMac();
		readonly HvMacVcpu Cpu;
		readonly byte* PagetableBase;

		readonly Xbox Box;
		
		public uint this[HvReg reg] {
			get => (uint) Cpu[reg];
			set => Cpu[reg] = value;
		}

		public uint this[HvVmcsField field] {
			get => (uint) Cpu[field];
			set => Cpu[field] = value;
		}
		
		public CpuCore(Xbox box) {
			Box = box;
			Cpu = Hv.CreateVcpu();

			ulong Cap2Ctrl(ulong cap, uint ctrl) => (ctrl | (cap & 0xffffffff)) & (cap >> 32);

			void SetSegment(HvVmcsField seg, HvVmcsField ar, HvVmcsField limit, HvVmcsField @base) {
				Cpu[seg] = seg == HvVmcsField.GUEST_CS ? 1U << 3 : 2U << 3;
				Cpu[ar] = 0xC093;
				Cpu[limit] = 0xFFFFFFFFU;
				Cpu[@base] = 0;
			}
			
			var VMCS_PRI_PROC_BASED_CTLS_HLT = 1U << 7;
			var VMCS_PRI_PROC_BASED_CTLS_CR8_LOAD = 1U << 19;
			var VMCS_PRI_PROC_BASED_CTLS_CR8_STORE = 1U << 20;

			Cpu[HvVmcsField.CTRL_PIN_BASED] = Cap2Ctrl(Hv[HvVmxCapability.PINBASED], 0);
			Cpu[HvVmcsField.CTRL_CPU_BASED] = Cap2Ctrl(Hv[HvVmxCapability.PROCBASED], VMCS_PRI_PROC_BASED_CTLS_HLT | VMCS_PRI_PROC_BASED_CTLS_CR8_LOAD | VMCS_PRI_PROC_BASED_CTLS_CR8_STORE);
			Cpu[HvVmcsField.CTRL_CPU_BASED2] = Cap2Ctrl(Hv[HvVmxCapability.PROCBASED2], 0);
			Cpu[HvVmcsField.CTRL_VMENTRY_CONTROLS] = Cap2Ctrl(Hv[HvVmxCapability.ENTRY], 0);

			Cpu[HvVmcsField.CTRL_EXC_BITMAP] = 0xFFFFFFFF;
			Cpu[HvVmcsField.CTRL_CR0_MASK] = 0xFFFFFFFF;
			Cpu[HvVmcsField.CTRL_CR0_SHADOW] = 0xFFFFFFFF;
			Cpu[HvVmcsField.CTRL_CR4_MASK] = 0xFFFFFFFF;
			Cpu[HvVmcsField.CTRL_CR4_SHADOW] = 0xFFFFFFFF;

			Cpu[HvVmcsField.GUEST_LDTR] = Cpu[HvVmcsField.GUEST_TR] = 0;
			Cpu[HvVmcsField.GUEST_LDTR_LIMIT] = Cpu[HvVmcsField.GUEST_TR_LIMIT] = 0;
			Cpu[HvVmcsField.GUEST_LDTR_BASE] = Cpu[HvVmcsField.GUEST_TR_BASE] = 0;
			Cpu[HvVmcsField.GUEST_LDTR_AR] = 0x10000;
			Cpu[HvVmcsField.GUEST_TR_AR] = 0x83;
			
			Cpu[HvReg.CR4] = 0x2000;

			PagetableBase = Hv.Map(PagetableAddr, 4 * 1024 * 1024 + 4 * 1024, HvMemoryFlags.RWX);
			
			SetupPagetable();
			
			SetSegment(HvVmcsField.GUEST_CS, HvVmcsField.GUEST_CS_AR, HvVmcsField.GUEST_CS_LIMIT, HvVmcsField.GUEST_CS_BASE);
			SetSegment(HvVmcsField.GUEST_DS, HvVmcsField.GUEST_DS_AR, HvVmcsField.GUEST_DS_LIMIT, HvVmcsField.GUEST_DS_BASE);
			SetSegment(HvVmcsField.GUEST_ES, HvVmcsField.GUEST_ES_AR, HvVmcsField.GUEST_ES_LIMIT, HvVmcsField.GUEST_ES_BASE);
			SetSegment(HvVmcsField.GUEST_FS, HvVmcsField.GUEST_FS_AR, HvVmcsField.GUEST_FS_LIMIT, HvVmcsField.GUEST_FS_BASE);
			SetSegment(HvVmcsField.GUEST_GS, HvVmcsField.GUEST_GS_AR, HvVmcsField.GUEST_GS_LIMIT, HvVmcsField.GUEST_GS_BASE);
			SetSegment(HvVmcsField.GUEST_SS, HvVmcsField.GUEST_SS_AR, HvVmcsField.GUEST_SS_LIMIT, HvVmcsField.GUEST_SS_BASE);
		}
		
		void SetupPagetable() {
			var dir = (uint*) PagetableBase;
			for(var i = 0; i < 1024; ++i) {
				dir[i] = (uint) ((PagetableAddr + 4096 * (i + 1)) | 7);
				var table = (uint*) (PagetableBase + 4096 * (i + 1));
				for(var j = 0; j < 1024; ++j)
					table[j] = 0;
			}

			Cpu[HvReg.CR3] = PagetableAddr;
			Cpu[HvReg.CR0] = 0x80000000 | 0x20 | 0x01;
		}

		public byte* CreatePhysicalPages(uint addr, int count) {
			return Hv.Map(addr, (ulong) (count * 16 * 1024 * 1024), HvMemoryFlags.RWX);
		}

		public void MapPages(uint virt, uint phys, int count, bool present) {
			Debug.Assert(Cpu[HvReg.CR3] == PagetableAddr);
			var dir = (uint*) PagetableBase;
			for(var i = 0; i < count; ++i) {
				var tableOff = dir[virt >> 22] & 0xFFFFF000;
				Debug.Assert(tableOff > PagetableAddr && tableOff < PagetableAddr + 5 * 1024 * 1024);
				var table = (uint*) (PagetableBase + (tableOff - PagetableAddr));
				//WriteLine($"Setting {virt:X} to map to {phys:X}");
				table[(virt >> 12) & 0x3FF] = phys | 0x6U | (present ? 1U : 0U);
				virt += 4096;
				phys += 4096;
			}
			Cpu.InvalidateTlb();
		}

		public bool IsMapped(uint addr) {
			Debug.Assert(Cpu[HvReg.CR3] == PagetableAddr);
			var dir = (uint*) PagetableBase;
			var tableOff = dir[addr >> 22] & 0xFFFFF000;
			var table = (uint*) (PagetableBase + (tableOff - PagetableAddr));
			return (table[(addr >> 12) & 0x3FF] & 1) == 1;
		}

		public uint Virt2Phys(uint addr) {
			Debug.Assert(Cpu[HvReg.CR3] == PagetableAddr);
			var dir = (uint*) PagetableBase;
			var tableOff = dir[addr >> 22] & 0xFFFFF000;
			var table = (uint*) (PagetableBase + (tableOff - PagetableAddr));
			return (table[(addr >> 12) & 0x3FF] & 0xFFFFF000) | (addr & 0xFFF);
		}

		public void DumpRegs() {
			var regs = new[] { HvReg.RAX, HvReg.RBX, HvReg.RCX, HvReg.RDX, HvReg.RSP, HvReg.RBP, HvReg.RSI, HvReg.RDI, HvReg.RIP, HvReg.RFLAGS };
			foreach(var reg in regs)
				WriteLine($"- {reg} == {(uint) Cpu[reg]:X8}");
		}

		public void Run() {
			while(true)
				Enter();
		}

		void Enter() {
			WriteLine($"Entering {Cpu[HvReg.RIP]:X}");

			Cpu.Enter();

			var _reason = Cpu[HvVmcsField.RO_EXIT_REASON];
			if((_reason & 0x80000000) != 0)
				throw new Exception($"Failed to enter: {_reason:X8}");

			var reason = (HvExitReason) _reason;
			if(reason == HvExitReason.IRQ) {
				Box.ThreadManager.Next();
				return;
			}
			if(reason == HvExitReason.EPT_VIOLATION)
				return;
			var qual = (uint) Cpu[HvVmcsField.RO_EXIT_QUALIFIC];
			var insnLen = (uint) Cpu[HvVmcsField.RO_VMEXIT_INSTR_LEN];
			WriteLine($"Exited with {reason} at {Cpu[HvReg.RIP]:X}");

			switch(reason) {
				case HvExitReason.EXC_NMI:
					var vecVal = Cpu[HvVmcsField.RO_VMEXIT_IRQ_INFO] & 0xFFFFU;
					var errorCode = Cpu[HvVmcsField.RO_VMEXIT_IRQ_ERROR];
					switch((vecVal >> 8) & 7) {
						case 6:
							WriteLine($"Interrupt {vecVal & 0xFF}");
							break;
						case 3:
							WriteLine($"Exception {vecVal & 0xFF}");
							break;
						default:
							WriteLine($"Unknown NMI {vecVal:X}");
							break;
					}
					DumpRegs();
					Environment.Exit(0);
					break;
				case HvExitReason.EPT_VIOLATION:
					break;
				case HvExitReason.VMCALL:
					var call = (int) (Cpu[HvReg.RIP] - Xbox.KernelCallsBase) / 4;
					WriteLine($"Kernel call to {(KernelExportNames) call}");
					if(!Box.Kernel.Functions.ContainsKey(call)) {
						WriteLine($"Unimplemented kernel function 0x{call:X} - {(KernelExportNames) call}");
						Environment.Exit(0);
					}

					Box.Kernel.Functions[call]();
					break;
			}
		}
	}
}