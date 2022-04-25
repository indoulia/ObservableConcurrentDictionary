using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentDictionary.ObservableDictionary
{
    public class ConcurrentObservableDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public ConcurrentObservableDictionary() : base() { }
        public ConcurrentObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public ConcurrentObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public ConcurrentObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

        public delegate void TimeOutEventHandler(object myObject, TimeOutEventArgs<TKey, TValue> myArgs);
        public event TimeOutEventHandler OnTimeOut;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Dictionary Key</param>
        /// <param name="value">Dictioanry Value</param>
        /// <param name="cancellationTokenSource">A cancellation token source</param>
        /// <param name="timeOut">An optional param, just incase you will need to monitor the timeout for a item</param>
        public bool TryAdd(TKey key, TValue value, int timeOut = 0)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            if (timeOut > 0)
            {
                if (value.GetType().GetProperty("CancellationTokenSource") != null)
                {
                    PropertyInfo p = typeof(TValue).GetProperty("CancellationTokenSource");
                    if (p != null && p.PropertyType == typeof(CancellationTokenSource))
                    {
                        cancellationTokenSource = (CancellationTokenSource)p.GetValue(value, null);
                        if (cancellationTokenSource == null)
                            throw new Exception("CancellationTokenSource property must have initialized with new before sending to TryAdd");
                    }
                }
                else
                {
                    throw new Exception("CancellationTokenSource property missing in Value param");
                }
            }

            var item = new KeyValuePair<TKey, TValue>(key, value);
            if (base.TryAdd(key, value))
            {
                if (timeOut > 0)
                {
                    try
                    {
                        cancellationTokenSource.CancelAfter(timeOut);
                        Task.Run(() => StartTimer(item, cancellationTokenSource), cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        if (cancellationTokenSource.IsCancellationRequested && base.ContainsKey(item.Key))
                        {
                            var myArgs = new TimeOutEventArgs<TKey, TValue>(NotifyCollectionChangedAction.Add.ToString(), item);
                            OnTimeOut?.Invoke(this, myArgs);
                        }
                    }
                }
                return true;
            }

            return false;
        }

        public bool TryRemove(TKey key)
        {
            if (base.TryRemove(key, out TValue dicValue))
            {
                //if (value.GetType().GetProperty("CancellationTokenSource") != null)
                //{
                //    PropertyInfo p = typeof(TValue).GetProperty("CancellationTokenSource");
                //    if (p != null && p.PropertyType == typeof(CancellationTokenSource))
                //    {
                //        var cancellationTokenSource = (CancellationTokenSource)p.GetValue(value, null);
                //        cancellationTokenSource.Cancel();
                //    }
                //}
                //return true;
                return CancelRunningTimerTask(dicValue, true);
            }
            else
                return false;
        }

        public bool TryResetTimer(TKey key, int timeOut = 0)
        {
            if (base.TryGetValue(key, out TValue dicValue))
            {
                return CancelRunningTimerTask(dicValue, false, timeOut);
                //if (dicValue.GetType().GetProperty("CancellationTokenSource") != null)
                //{
                //    PropertyInfo p = typeof(TValue).GetProperty("CancellationTokenSource");
                //    if (p != null && p.PropertyType == typeof(CancellationTokenSource))
                //    {
                //        var canellationToken = (CancellationTokenSource)p.GetValue(dicValue, null);
                //        canellationToken.CancelAfter(timeOut);
                //        return true;
                //    }
                //}
            }
            return false;
        }

        private bool CancelRunningTimerTask(TValue dicValue, bool now, int timeOut = 0)
        {

            if (dicValue.GetType().GetProperty("CancellationTokenSource") != null)
            {
                PropertyInfo p = typeof(TValue).GetProperty("CancellationTokenSource");
                if (p != null && p.PropertyType == typeof(CancellationTokenSource))
                {
                    var cancellationTokenSource = (CancellationTokenSource)p.GetValue(dicValue, null);

                    if (now)
                        cancellationTokenSource.Cancel();
                    else
                    {
                        if (timeOut == 0)
                            throw new Exception("Timeout value should be greater than 0 to reset timer");
                        cancellationTokenSource.CancelAfter(timeOut);
                    }

                    return true;
                }
            }
            return false;
        }

        private void StartTimer(KeyValuePair<TKey, TValue> item, CancellationTokenSource cancellationTokenSource)
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    Task.Delay(2000, cancellationTokenSource.Token).Wait();
                }
                catch(Exception ex)
                { }
            }

            if (cancellationTokenSource.IsCancellationRequested && base.ContainsKey(item.Key))
            {
                var myArgs = new TimeOutEventArgs<TKey, TValue>(NotifyCollectionChangedAction.Add.ToString(), item);
                OnTimeOut?.Invoke(this, myArgs);
            }

            //var task = Task.Delay(2000, cancellationTokenSource.Token);
            //task.Wait(cancellationTokenSource.Token);
            //if (!task.IsCanceled)
            //    StartTimer(item, cancellationTokenSource);

            // task.Dispose();


            //Timer timer = null;
            //timer = new Timer(new TimerCallback(y =>
            //    {
            //        if (cancellationTokenSource.IsCancellationRequested)
            //        {
            //            if (base.ContainsKey(item.Key))
            //            {
            //                var myArgs = new TimeOutEventArgs<TKey, TValue>(NotifyCollectionChangedAction.Add.ToString(), item);
            //                OnTimeOut?.Invoke(this, myArgs);
            //                timer.Dispose();
            //            }
            //        }
            //    }));
            //timer.Change(1000, 1000000000);


            //bool loopCheck = true;
            //while (loopCheck)
            //{
            //    if (cancellationTokenSource.IsCancellationRequested)
            //    {
            //        NewMethod(item);
            //        loopCheck = false;
            //    }
            //    //Task.Delay(1000);
            //}

            //void MyIntervalFunction(object state)
            //{
            //    if (cancellationTokenSource.IsCancellationRequested)
            //    {
            //        if (base.ContainsKey(item.Key))
            //        {
            //            var myArgs = new TimeOutEventArgs<TKey, TValue>(NotifyCollectionChangedAction.Add.ToString(), item);
            //            OnTimeOut?.Invoke(this, myArgs);

            //        }
            //    }
            //}
        }
    }
}
