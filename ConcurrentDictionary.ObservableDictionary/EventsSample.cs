using System;

namespace ConcurrentDictionary.ObservableDictionary
{
    public static class EventsSample<TKey, TValue>
    {
        public static void TimeOut(object sender, TimeOutEventArgs<TKey, TValue> e)
        {
                Console.WriteLine(e.Item.Key + "  Fired at " + DateTime.Now.ToString());
        }
    }
}
