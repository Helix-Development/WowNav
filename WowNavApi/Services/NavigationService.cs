using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using WowNavBase;

namespace WowNavApi.Services
{
    public unsafe class NavigationService : INavigationService
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate XYZ* CalculatePathDelegate(
            uint mapId,
            XYZ start,
            XYZ end,
            bool straightPath,
            out int length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void FreePathArr(XYZ* pathArr);

        static CalculatePathDelegate calculatePath;
        static FreePathArr freePathArr;

        public NavigationService()
        {
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mapsPath = $"{currentFolder}\\Navigation.dll";

            var navProcPtr = LoadLibrary(mapsPath);

            var calculatePathPtr = GetProcAddress(navProcPtr, "CalculatePath");
            calculatePath = Marshal.GetDelegateForFunctionPointer<CalculatePathDelegate>(calculatePathPtr);

            var freePathPtr = GetProcAddress(navProcPtr, "FreePathArr");
            freePathArr = Marshal.GetDelegateForFunctionPointer<FreePathArr>(freePathPtr);
        }

        public Position[] CalculatePath(uint mapId, Position start, Position end, bool straightPath)
        {
            var ret = calculatePath(mapId, start.ToXYZ(), end.ToXYZ(), straightPath, out int length);
            var list = new Position[length];
            for (var i = 0; i < length; i++)
            {
                list[i] = new Position(ret[i]);
            }
            freePathArr(ret);

            return list;
        }
    }
}