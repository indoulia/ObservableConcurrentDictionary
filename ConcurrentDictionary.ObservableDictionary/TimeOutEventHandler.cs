using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentDictionary.ObservableDictionary
{
    public class TimeOutEventArgs<TKey, TValue> : EventArgs
    {
        private string action;
        private KeyValuePair<TKey, TValue> item;

        public TimeOutEventArgs(string Action, KeyValuePair<TKey, TValue> DictItem)
        {
            this.action = Action;
            this.item = DictItem;
        }
        public string Action { get { return action; } }
        public KeyValuePair<TKey, TValue> Item { get { return item; } }
    }
}
