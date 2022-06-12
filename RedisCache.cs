using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
namespace test.Redis
{
    class RedisCache : IDisposable
    {
        public event Action<KeyValuePair<string, string>> CacheAdded;
        private Dictionary<string, string> Cache;
        private List<Thread> threadPool;
        public RedisCache()
        {
            Cache = new Dictionary<string, string>();
            threadPool = new List<Thread>();
        }
        private void AddCache(KeyValuePair<string, string> pair, uint ttlSeconds)
        {
            Cache[pair.Key] = pair.Value;
            Thread thread = new Thread(ttlCache => 
            {
                TtlCache cache = (TtlCache)ttlCache;
                StartTTL(cache.Name, cache.TTLSeconds);
            });
            threadPool.Add(thread);
            thread.Start(new TtlCache(ttlSeconds, pair.Key));
            CacheAdded?.Invoke(pair);
        }
        public void Dispose()
        {
            for (int i = 0; i < threadPool.Count; i++)
                threadPool[i].Interrupt();

            if (CacheAdded != null)
                CacheAdded -= CacheAdded;
        }
        public void Set(string name, string value, uint ttlSeconds = 43200) // 43200 seconds => 12 hours
        {
            KeyValuePair<string, string> newPair = new KeyValuePair<string, string>(name, value);
            AddCache(newPair, ttlSeconds);
        }
        private void StartTTL(string name, uint ttlSeconds) // start on new thread
        {
            while (ttlSeconds > 0) 
            {
                Thread.Sleep(1000);
                ttlSeconds--;
            }
            Cache.Remove(name);
        }
        public string Get(string name) // returns value if exist
        {
            if (Cache.ContainsKey(name)) return Cache[name];

            return null;
        }
        public bool Del(string name)
        {
            return Cache.Remove(name);
        }
    }
}
