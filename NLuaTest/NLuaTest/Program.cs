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
            Test1();
            Test2();
        }

        private static void Test1()
        {
            using (var lua = new Lua())
            {
                var result = lua.DoFile("test.lua");
            }
        }

        private static void Test2()
        {
            using (var lua = new Lua())
            {
                lua.State.RequireF("luasql.mysql", luaopen_luasql_mysql, true);
                var result = lua.DoFile("test.lua");
            }
        }
        
        [DllImport("mysql.so")]
        private static extern int luaopen_luasql_mysql(IntPtr state);
    }
}