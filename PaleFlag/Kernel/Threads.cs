﻿using System;

namespace PaleFlag.XboxKernel {
	public partial class Kernel {
		[Export(0xFF)]
		NtStatus PsCreateSystemThreadEx(
			GuestMemory<uint> threadHandle, uint threadExtraSize, uint kernelStackSize, uint tlsDataSize, GuestMemory<uint> threadId, 
			uint startContext1, uint startContext2, bool createSuspended, bool debugStack, uint startRoutine
		) {
			Console.WriteLine($"PsCreateSystemThreadEx");
			Console.WriteLine($"\t{threadHandle.GuestAddr:X} {threadExtraSize:X} {kernelStackSize:X} {tlsDataSize:X} {threadId.GuestAddr:X}");
			Console.WriteLine($"\t{startContext1:X} {startContext2:X} {createSuspended} {debugStack} {startRoutine:X}");
			return NtStatus.Success;
		}
	}
}