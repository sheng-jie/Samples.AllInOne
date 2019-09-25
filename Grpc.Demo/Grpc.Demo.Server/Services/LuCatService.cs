using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.Demo.Server
{
    public class LuCatService : LuCat.LuCatBase
    {
        private static readonly List<string> Cats = new List<string>() { "Ó¢¶ÌÒø½¥²ã", "Ó¢¶Ì½ğ½¥²ã", "ÃÀ¶Ì", "À¶Ã¨", "Àê»¨Ã¨", "éÙÃ¨" };
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);
        public override Task<SuckingCatResult> SuckingCat(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new SuckingCatResult()
            {
                Message = $"ÄúÎüÁËÒ»Ö»{Cats[Rand.Next(0, Cats.Count)]}"
            });
        }
    }
}