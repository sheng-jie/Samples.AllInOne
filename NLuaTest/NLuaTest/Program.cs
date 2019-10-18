using NLua;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NLuaTest
{
    class Program
    {
        static void Main(string[] args)
        {
            bool a = Environment.Is64BitOperatingSystem;
            IntPtr lib = IntPtr.Zero;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var path = Path.Combine(AppContext.BaseDirectory, "lib", "mysql.dll");
                lib = NativeLibrary.Load(path, Assembly.GetExecutingAssembly(), null);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var path = Path.Combine(AppContext.BaseDirectory, "lib", "mysql.so");
                lib = NativeLibrary.Load(path, Assembly.GetExecutingAssembly(), null);
            }
            IntPtr method = NativeLibrary.GetExport(lib, "luaopen_luasql_mysql");
            KeraLua.LuaFunction function = Marshal.GetDelegateForFunctionPointer<KeraLua.LuaFunction>(method);
            using (var lua = new Lua())
            {
                var b = function.GetMethodInfo();
                KeraLua.Lua state = lua.State;
                IntPtr ptr = lua.State.Handle;
                state.RequireF("mysql", function, true);
                var result = lua.DoFile("test.lua");
            }
        }
    }
}
