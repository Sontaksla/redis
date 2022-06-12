using System;
using System.Collections.Generic;
using System.Text;

namespace test.Redis
{
    struct TtlCache
    {
        public TtlCache(uint sec, string name)
        {
            TTLSeconds = sec;
            Name = name;
        }
        public uint TTLSeconds;
        public string Name;
    }
}
