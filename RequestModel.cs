using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace test.Redis
{
    class RequestModel
    {
        private readonly RedisCache cache;
        public RequestType RequestType;
        public RequestModel(string request, RedisCache cache)
        {
            this.cache = cache;
            string[] parts = request.Split(' ').Where(i => i != "").ToArray();
            string output = "";
            try
            {
                SetRequestType(parts[0]);
                switch (RequestType)
                {
                    case RequestType.GET:
                        output = cache.Get(parts[1]) ?? "null";
                        break;
                    case RequestType.SET:
                        cache.Set(parts[1], parts[2],
                        // if ttl wasn't set then ttl is 43200 seconds
                            parts.Length < 4 ? 43200 : uint.Parse(parts[3])); 
                        output = "OK";
                        break;
                    case RequestType.DEL:
                        output = cache.Del(parts[1]) == true ? "OK" : "null";
                        break;
                    default:
                        break;
                }
                Console.WriteLine("redis>" + output);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("redis>" + "Unknown usage.\n" + ex.Message);
            }
        }
        private void SetRequestType(string requestPart) =>
            RequestType = (RequestType)Enum.Parse(typeof(RequestType), requestPart);
    }
}
