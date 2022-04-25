using System.Threading;

namespace ConcurrentDictionary.ObservableDictionary
{
    public class DicValue
    {
        public int Value { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
