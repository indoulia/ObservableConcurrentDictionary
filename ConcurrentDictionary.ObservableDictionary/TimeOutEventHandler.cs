using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentDictionary.ObservableDictionary
{
    public class TimeOutEventArgs : EventArgs
    {
        private string action;
        private object item;

        public TimeOutEventArgs(string Action, object DictItem)
        {
            this.action = Action;
            this.item = DictItem;
        }
        public string Action { get { return action; } }
        public object Item { get { return item; } }
    }
}
