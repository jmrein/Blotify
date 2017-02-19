using System;
using System.Runtime.InteropServices;

namespace Blotify
{
	internal static class NativeMethods
	{
		[DllImport("gdi32")]
		public static extern int DeleteObject(IntPtr o);
	}
}