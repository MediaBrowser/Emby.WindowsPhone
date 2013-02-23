using System;
using System.Runtime.InteropServices;

namespace System
{
    public enum ProcessorFeature : uint
    {
        _3DNOW_INSTRUCTIONS_AVAILABLE = 7,
        CHANNELS_ENABLED = 16,
        COMPARE_EXCHANGE_DOUBLE = 2,
        COMPARE_EXCHANGE128 = 14,
        COMPARE64_EXCHANGE128 = 15,
        FLOATING_POINT_EMULATED = 1,
        FLOATING_POINT_PRECISION_ERRATA = 0,
        MMX_INSTRUCTIONS_AVAILABLE = 3,
        NX_ENABLED = 12,
        PAE_ENABLED = 9,
        RDTSC_INSTRUCTION_AVAILABLE = 8,
        SECOND_LEVEL_ADDRESS_TRANSLATION = 20,
        SSE3_INSTRUCTIONS_AVAILABLE = 13,
        VIRT_FIRMWARE_ENABLED = 21,
        XMMI_INSTRUCTIONS_AVAILABLE = 6,
        XMMI64_INSTRUCTIONS_AVAILABLE = 10,
        XSAVE_ENABLED = 17
    }
    public enum ProcessorArchitecture : ushort
    {
        INTEL = 0,
        MIPS = 1,
        ALPHA = 2,
        PPC = 3,
        SHX = 4,
        ARM = 5,
        IA64 = 6,
        ALPHA64 = 7,
        MSIL = 8,
        AMD64 = 9,
        IA32_ON_WIN64 = 10,
        UNKNOWN = 0xFFFF
    }
    public enum ProcessorType : uint
    {
        INTEL_386 = 386,
        INTEL_486 = 486,
        INTEL_PENTIUM = 586,
        INTEL_IA64 = 2200,
        AMD_X8664 = 8664,
        MIPS_R4000 = 4000,    // incl R4101 & R3910 for Windows CE
        ALPHA_21064 = 21064,
        PPC_601 = 601,
        PPC_603 = 603,
        PPC_604 = 604,
        PPC_620 = 620,
        HITACHI_SH3 = 10003,   // Windows CE
        HITACHI_SH3E = 10004,   // Windows CE
        HITACHI_SH4 = 10005, // Windows CE
        MOTOROLA_821 = 821,    // Windows CE
        SHx_SH3 = 103,    // Windows CE
        SHx_SH4 = 104,     // Windows CE
        STRONGARM = 2577,    // Windows CE - 0xA11
        ARM720 = 1824,    // Windows CE - 0x720
        ARM820 = 2080,   // Windows CE - 0x820
        ARM920 = 2336,    // Windows CE - 0x920
        ARM_7TDMI = 70001,   // Windows CE
        OPTIL = 0x494f,  // MSIL
        UNKNOWN = 0xFFFF
    }
    public class SystemInfo
    {
        public ProcessorArchitecture ProcessorArchitecture;
        public ushort ProcessorArchitectureId;
        public ProcessorType ProcessorType;
        public uint ProcessorTypeId;
        public uint NumberOfProcessors;
        public ushort ProcessorLevel;
        public ushort ProcessorRevision;
        public uint AllocationGranularity;
    };
    public class CPU
    {
     
        [StructLayout(LayoutKind.Sequential)]
        struct _SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        };

        [DllImport("kernel32.dll")]
        static extern void GetNativeSystemInfo(ref _SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);


        static _SYSTEM_INFO sysInfo;
        static SystemInfo sInfo;

        public static bool IsProcessorFeaturePresent(ProcessorFeature feature)
        {
            return IsProcessorFeaturePresent((uint)feature);
        }

        public static SystemInfo NativeInfo
        {
            get
            {
                if (sInfo==null)
                {
                    sysInfo = new _SYSTEM_INFO();
                    GetNativeSystemInfo(ref sysInfo);
                    sInfo = new SystemInfo()
                    {
                        ProcessorTypeId = sysInfo.dwProcessorType,
                        ProcessorArchitectureId = sysInfo.wProcessorArchitecture,
                        ProcessorLevel = sysInfo.wProcessorLevel,
                        ProcessorRevision= sysInfo.wProcessorRevision,
                        NumberOfProcessors = sysInfo.dwNumberOfProcessors,
                        AllocationGranularity = sysInfo.dwAllocationGranularity,
                        ProcessorArchitecture = Enum.IsDefined(typeof(ProcessorArchitecture), sysInfo.wProcessorArchitecture) ? (ProcessorArchitecture)sysInfo.wProcessorArchitecture : ProcessorArchitecture.UNKNOWN,
                        ProcessorType = Enum.IsDefined(typeof(ProcessorType), sysInfo.dwProcessorType) ? (ProcessorType)sysInfo.dwProcessorType : ProcessorType.UNKNOWN
                    };
                }
                return sInfo;
            }
        }
    }
}