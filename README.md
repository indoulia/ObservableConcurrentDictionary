# ObservableConcurrentDictionary
It extends the existing Concurrent Dictionary so that you can add timeout to individual Items and when its expires you will get timeout event fired.

How to use

1. Initiaize Dictionary
You can initialize the dictionary as usual
var oDict = new ConcurrentObservableDictionary<Key, Value>();

2. Attach an event
oDict.OnTimeOut += EventsSample<Key, Value>.TimeOut; 

3. Declare and define a method
Create a method as TimeOut, which will be called when event fired.
public static class EventsSample<TKey, TValue>
{
    public static void TimeOut(object sender, TimeOutEventArgs<TKey, TValue> e)
    {
        Console.WriteLine(e.Item.Key + "  Fired at " + DateTime.Now.ToString());
    }
}

4. Add a property in your Value object
This property is a must if you want to use the timeout feature. If you dont provide it and pass timeout value, will get exception
//Create a Cancellation Token Source
//so that the timeout can be postponded or task can be cancelled immidiately
var cancellationTokenSource = new CancellationTokenSource();

//Your Value pair should have a extra property to manage the cancellation token
var valueObject = new ValueClass() { Value = 1, CancellationTokenSource = cancellationTokenSource };

5. TryAdd Method usage
    A. If want to use timeout
        oDict.TryAdd(key, valueObject, timeoutinmilliseconds); //You can add different timeout value for different items in milliseconds
    B. If dont want to use timeout
        oDict.TryAdd(key, valueObject);
        
6. TryRemove usage
oDict.TryRemove(i.ToString());

7. TryResetTimer usage
If you want to reset the timer for an item, this methid will come handy.
oDict.TryResetTimer(key, timeoutinmilliseconds);
