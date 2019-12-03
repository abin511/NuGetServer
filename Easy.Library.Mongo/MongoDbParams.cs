using System.Collections.Generic;

namespace Easy.Library.Mongo
{
    public class MongoDbParams
    {
        private Dictionary<string, object> _mongoDbMap = null;

        public MongoDbParams()
        {
            _mongoDbMap = new Dictionary<string, object>();
        }

        public MongoDbParams Add(string key, object value)
        {
            if (key.StartsWith("@"))
            {
                key = key.TrimStart('@');
            }

            _mongoDbMap.Add(key, value);
            return this;
        }

        public Dictionary<string, object> Create()
        {
            return _mongoDbMap;
        }
    }
}
