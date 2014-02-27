using Nuane.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sample
{
	/// <summary>
	/// Dynamically loads advapi32.dll library and a delegate GetUserNameW function.
	/// Implements a managed wrapper for the delegate.
	/// 
	/// In this case, using P/Invoke to achieve the same effect would be easier.
	/// Why? Check out the README.md file for details.
	/// </summary>
	public class Advapi
	{
		private static readonly NativeLibrary Library;

		//advapi32.dll:
		//  BOOL WINAPI GetUserName(LPTSTR lpBuffer, LPDWORD lpnSize);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
		private delegate int GetUserNameDelegate(IntPtr buffer, ref int size);
		private static readonly GetUserNameDelegate GetUserNameMethod;
		private const string GetUserNameEntryPoint = "GetUserNameW";

		static Advapi()
		{
			Library = NativeLibrary.Load("advapi32");
			GetUserNameMethod = Library.GetDelegate<GetUserNameDelegate>(GetUserNameEntryPoint);
		}

		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		public static string GetUserName()
		{
			int size = 0;
			if (0 != GetUserNameMethod(IntPtr.Zero, ref size))
				throw new ApplicationException("Unexpected return code.");

			int error = Marshal.GetLastWin32Error();
			if (ERROR_INSUFFICIENT_BUFFER != error)
				throw new ApplicationException("Unexpected error code " + error + " .");

			IntPtr buffer = Marshal.AllocHGlobal(size * 2);
			try
			{
				if (0 == GetUserNameMethod(buffer, ref size))
					throw new ApplicationException("Function failed with error code " + Marshal.GetLastWin32Error() + " .");

				if (size <= 0)
					throw new ApplicationException("Unexpected size.");

				return Marshal.PtrToStringUni(buffer);
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}
	}

	public class Program
	{
		static void Main(string[] args)
		{
			string userName = Advapi.GetUserName();

			Console.WriteLine("User name: {0}", userName);
		}

	}
}
