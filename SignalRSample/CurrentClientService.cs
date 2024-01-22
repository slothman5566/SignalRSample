using System.Collections.Concurrent;

namespace SignalRServer
{
    public class CurrentClientService
    {

        private readonly ConcurrentDictionary<string, bool> _currentDict;

        public CurrentClientService()
        {
            _currentDict = new ConcurrentDictionary<string, bool>();


        }


        public void Push(string key)
        {
            _currentDict.TryAdd(key, true);
        }

        public void Remove(string key)
        {
            _currentDict.TryRemove(new(key, true));
        }

        public List<string> GetAll()
        {
            return _currentDict.Keys.ToList();
        }

        public void Clear()
        {
            _currentDict.Clear();
        }

    }
}
