using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentDictionary.ObservableDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            ///CReate object of the Dictionary
            var oDict = new ConcurrentObservableDictionary<string, DicValue>();
            ///Attach the method to be called if the event id fired
            oDict.OnTimeOut += EventsSample<string, DicValue>.TimeOut;

            for (int i = 0; i < 20; i++)
            {

          
              //Create a Cancellation Token Source
              //so that the timeout can be postponded or task can be cancelled immidiately
              var cancellationTokenSource = new CancellationTokenSource();

              //Your Value pair should have a extra property to manage the cancellation token
              var dicValue = new DicValue() { Value = 1, CancellationTokenSource = cancellationTokenSource };

              //When trying t
              //oDict.TryAdd(i.ToString(), dicValue, 2000);
              //Console.WriteLine(i.ToString()+ " First add:" + DateTime.Now.ToString());
              //Thread.Sleep(300);

              //oDict.TryRemove(i.ToString());

              dicValue.CancellationTokenSource = new CancellationTokenSource();
              oDict.TryAdd(i.ToString(), dicValue, 5000);
              //Console.WriteLine(i.ToString()+ " Second add:" + DateTime.Now.ToString());
              //Thread.Sleep(500);

              //oDict.TryResetTimer(i.ToString(), 5000);
              //Console.WriteLine(i.ToString()+ " Second Timer Reset:" + DateTime.Now.ToString());
          }


            // oDict.TryResetTimer("1", 5000);


            //Console.WriteLine(dicValue.Value.ToString());


            Console.ReadKey();

            Console.ReadLine();
        }
    }
}
