using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CardLifeAltLaunch
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Uses decrypted secure String pinned in memory in a provided Action
        /// </summary>
        /// <param name="anAction">The action that uses the pinned secure string</param>
        /// <returns></returns>
        unsafe public static void UseSecureString(this SecureString aSecureStr, Action<string> anAction)
        {
            var aStrPointer = IntPtr.Zero;
            var aStr = String.Empty;
            var aGCHandle = GCHandle.Alloc(aStr, GCHandleType.Pinned);

            try
            {
                aStrPointer = Marshal.SecureStringToGlobalAllocUnicode(aSecureStr);
                aStr = Marshal.PtrToStringUni(aStrPointer);

                anAction(aStr);
            }
            finally
            {
                // clear the memory right away so it cannot be snooped
                fixed (char* aCharPtr = aStr)
                {
                    for (int i = 0; i < aStr.Length; i++)
                    {
                        aCharPtr[i] = '\0';
                    }
                }

                aStr = null;

                aGCHandle.Free();
                Marshal.ZeroFreeGlobalAllocUnicode(aStrPointer);
            }
        }
    }
}
