#region License
//Copyright 2014 Lukas Pokorny
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
#endregion

using System;
using System.Runtime.InteropServices;

namespace Nuane.Interop
{
	/// <summary>
	/// Represents an error that occured while loading an unmanaged library or getting a function pointer or delegate.
	/// </summary>
	public class NativeLibraryException : Exception
	{
		/// <summary>
		/// Creates a new instance of <see cref="NativeLibraryException"/>.
		/// </summary>
		public NativeLibraryException()
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="NativeLibraryException"/> with the specified error message.
		/// </summary>
		/// <param name="message">Error message.</param>
		public NativeLibraryException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="NativeLibraryException"/> with the specified error message and inner exception.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="innerException">Inner exception.</param>
		public NativeLibraryException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	/// <summary>
	/// Specifies options for <see cref="NativeLibrary.Load(name, NativeLibraryLoadOptions)"/>
	/// and <see cref="NativeLibrary.TryLoad(name, NativeLibraryLoadOptions, NativeLibrary)"/>
	/// </summary>
	[Flags]
	public enum NativeLibraryLoadOptions
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,

		/// <summary>
		/// Look for a library with a suitable name in the assembly directory.
		/// </summary>
		SearchAssemblyDirectory = 1,

		/// <summary>
		/// Look for a library with a suitable name in the executable directory.
		/// </summary>
		SearchExecutableDirectory = 2,

		/// <summary>
		/// Look for a library with a suitable name in the current directory.
		/// </summary>
		SearchCurrentDirectory = 4,

		/// <summary>
		/// Look for a library with a suitable name in the assembly directory, executable directory, current directory and system directories.
		/// </summary>
		SearchAll = SearchAssemblyDirectory | SearchExecutableDirectory | SearchCurrentDirectory,
	}

	/// <summary>
	/// Native methods for dynamic library loading.
	/// </summary>
	internal static class NativeMethods
	{
		#region Windows

		//HMODULE LoadLibrary(LPCTSTR lpFileName)
		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		//FARPROC GetProcAddress(HMODULE hModule, LPCSTR lpProcName)
		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		//BOOL FreeLibrary(HMODULE hModule);
		[DllImport("kernel32", SetLastError = true)]
		public static extern int FreeLibrary(IntPtr hModule);

		#endregion

		#region Linux, *BSD, Mac OS X, Solaris, etc.

		// resolve all undefined symbols in the library
		public const int RTLD_NOW = 2;

		// make symbols defined by this library available for symbol resolution of subsequently loaded libraries
		public const int RTLD_GLOBAL = 0x100;

		//void *dlopen(const char *filename, int flag)
		[DllImport("libdl")]
		public static extern IntPtr dlopen(string filename, int flag);

		//char *dlerror(void)
		[DllImport("libdl")]
		public static extern IntPtr dlerror();

		//void *dlsym(void *handle, const char *symbol)
		[DllImport("libdl")]
		public static extern IntPtr dlsym(IntPtr handle, string symbol);

		//int dlclose(void *handle)
		[DllImport("libdl")]
		public static extern int dlclose(IntPtr handle);

		#endregion
	}
}
