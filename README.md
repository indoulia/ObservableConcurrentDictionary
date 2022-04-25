# ObservableConcurrentDictionary
It extends the existing Concurrent Dictionary so that you can add timeout to individual Items and when its expires you will get timeout event fired.

## How to use

### Initiaize Dictionary
You can initialize the dictionary as usual
```bash
var oDict = new ConcurrentObservableDictionary<Key, Value>();
```

### Attach an event
```bash
oDict.OnTimeOut += EventsSample<Key, Value>.TimeOut; 
```

### Declare and define a method
Create a method as TimeOut, which will be called when event fired.
```bash
public static class EventsSample<TKey, TValue>
{
    public static void TimeOut(object sender, TimeOutEventArgs<TKey, TValue> e)
    {
        Console.WriteLine(e.Item.Key + "  Fired at " + DateTime.Now.ToString());
    }
}
```

### Add a property in your Value object
This property is a must if you want to use the timeout feature. If you dont provide it and pass timeout value, will get exception
```bash
//Create a Cancellation Token Source
//so that the timeout can be postponded or task can be cancelled immidiately
var cancellationTokenSource = new CancellationTokenSource();

//Your Value pair should have a extra property to manage the cancellation token
var valueObject = new ValueClass() { Value = 1, CancellationTokenSource = cancellationTokenSource };
```

### TryAdd Method usage
    A. If want to use timeout       
        oDict.TryAdd(key, valueObject, timeoutinmilliseconds); //You can add different timeout value for different items in milliseconds
       
    B. If dont want to use timeout        
        oDict.TryAdd(key, valueObject);
      
        
### TryRemove usage
```bash
oDict.TryRemove(i.ToString());
```

### TryResetTimer usage
If you want to reset the timer for an item, this methid will come handy.
```bash
oDict.TryResetTimer(key, timeoutinmilliseconds);
```
