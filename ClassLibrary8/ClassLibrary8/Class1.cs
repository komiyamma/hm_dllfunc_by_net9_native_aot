using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClassLibrary8
{
    public class Class1
    {
        [UnmanagedCallersOnly(EntryPoint = "doubleint", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static nint DoubleInt(nint i) => i * 2;

        static string ret_sum_string = "";

        [UnmanagedCallersOnly(EntryPoint = "sumstring", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr SumString(IntPtr first, IntPtr second)
        {
            // Parse strings from the passed pointers 
            string my1String = Marshal.PtrToStringUni(first) ?? "";
            string my2String = Marshal.PtrToStringUni(second) ?? "";

            // Concatenate strings 
            ret_sum_string = my1String + my2String;

            unsafe
            {
                fixed (char* p = ret_sum_string)
                {
                    return (IntPtr)p;
                }
            }
        }
    }
}
