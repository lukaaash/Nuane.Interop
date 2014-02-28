using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace Nuane.Interop.UnitTests
{
	[TestClass]
	public class NativeLibraryTest
	{
		//TODO: Add more tests

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
		private delegate int GetUserNameDelegate(IntPtr buffer, ref int size);

		[TestMethod]
		public void GetUserName()
		{
			//advapi32.dll:
			//  BOOL WINAPI GetUserName(LPTSTR lpBuffer, LPDWORD lpnSize);

			int ERROR_INSUFFICIENT_BUFFER = 122;

			using (NativeLibrary lib = NativeLibrary.Load("advapi32"))
			{
				var method = lib.GetDelegate<GetUserNameDelegate>("GetUserNameW");

				int size = 0;
				Assert.AreEqual(0, method(IntPtr.Zero, ref size));
				Assert.AreEqual(ERROR_INSUFFICIENT_BUFFER, Marshal.GetLastWin32Error());
				Assert.IsTrue(size > 0);

				IntPtr buffer = Marshal.AllocHGlobal(size * 2);
				Assert.AreNotEqual(0, method(buffer, ref size));
				Assert.IsTrue(size > 0);

				string userName = Marshal.PtrToStringUni(buffer);

				Assert.AreEqual(Environment.UserName.ToLowerInvariant(), userName.ToLowerInvariant());
			}
		}
	}
}
