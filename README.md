Nuane.Interop
=============

.NET library that makes it possible to easily load and use unmanaged DLLs without P/Invokes.
P/Invokes are useful, but somewhat limited because each DllImport has the library name hardcoded.

The NativeLibrary object can be very useful in the following scenarios:

  a) A .NET application can run as 32-bit or 64-bit process and needs to load a corresponding native DLL
     for the currently used architectire. Windows doesn't support universal binaries (a useful feature of OS X), which
	 means it must come with two veriants of each native DLL.

  b) A .NET application that needs to load dozens of unmanaged DLLs as plugins. All the DLLs use the same API,
     but P/Invokes are unsuitable for this because of hard-coded library names.

  c) A .NET application that needs to load DLLs from a specific path that is not known at compile-time.

  d) A .NET application that runs on multiple platforms (Windows and Linux/Mono, for example) and needs to load native DLLs
     that are named differently on each of the platforms.

Sample code
===========

```
using System;
using System.Runtime.InteropServices;

public class Program
{
	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
	private delegate int GetUserNameDelegate(IntPtr buffer, ref int size);

	public void Main()
	{
		// load a library
		var lib = NativeLibrary.Load("advapi32");

		// get delegate for an unmanaged function
		var getUserName = lib.GetDelegate<GetUserNameDelegate>("GetUserNameW");

		// and call it
		int size = 0;
		int result = getUserName(IntPtr.Zero, ref size);
		...
	}
}
```
