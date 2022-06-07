using System.Net;
namespace MultiThreadProject;

public class Program
{
        public const int Repetitions = 1000;
        public static void DoWork( )
        {
                for (int i = 0; i < Repetitions; i++)
                {
                        Console.Write("+");
                }
        }
        public static void DoWork(object state)
        {
                for (int i = 0; i < Repetitions; i++)
                {
                        Console.Write(state);
                }
        }
    
        public static void Main(string[] args)
        {
                Console.WriteLine("Hello World!");
                
               // TestThread();
               //TestThreadPool();
               // TestAsync();
               // TestAsync2();
               // TestAsync3();
               // TestStartNew();
               // TestAsync4();
               // TestParallel();
               // TestMonitor();
               TestLock();
        }


        //最简单的线程
        public static void TestThread()
        {
                Thread t = new Thread(new ThreadStart(DoWork));
                
                //t.IsBackground
                t.Start();
                for (int i = 0; i < Repetitions; i++)
                {
                        Console.Write("-");
                }
                t.Join();
        }
        
        //线程池
        public static void TestThreadPool()
        {
                ThreadPool.QueueUserWorkItem(DoWork, "+");
                for (int i = 0; i < Repetitions; i++)
                {
                        Console.Write("-");
                }
        }
        
        //异步任务
        public static void TestAsync()
        {
                var t = new Task(DoWork);
                t.Start();
                for (int i = 0; i < Repetitions; i++)
                {
                        Console.Write("-");
                }
                t.Wait();
        }
        
        //异步任务,带返回值
        public static void TestAsync2()
        {
                var t = new Task<string>(() => {
                                return PiCalculator.Calculate(100);
                        }
                );
                t.Start();
                int i = 0;
                while (true)
                {
                        if (t.IsCompleted)
                        {
                                Console.WriteLine();
                                break;
                        }
                        Console.Write(i++);
                }
                Console.WriteLine(t.Result);
        }
        
        
        //取消异步任务，cancelToken.Cancel()
        public static void TestAsync3()
        {
                
                var cancelToken = new CancellationTokenSource();
                var token = cancelToken.Token;
                token.Register(() => {
                                Console.WriteLine("Canceled");
                        }
                );
                var t=Task<string>.Run((() => {
                       return PiCalculator.Calculate(100,token);
                }), token);
                int i = 0;
                while (true)
                {
                        if (t.IsCompleted)
                        {
                                Console.WriteLine();
                                break;
                        }
                        if (i == 1)
                        {
                                cancelToken.Cancel();
                                t.Wait();
                        }
                        Console.Write(i++);
                }
                Console.WriteLine(t.Result);
        }
 
        
        //startNew
        public static void TestStartNew()
        {
                var t = Task.Factory.StartNew(DoWork, TaskCreationOptions.LongRunning);
                for (int i = 0; i < Repetitions; i++)
                {
                        Console.Write("-");
                }
                t.Wait();
        }
        
        //async/await
        public static async Task WriteWebRequestSizeAsync(string url)
        {
                var webRequest = WebRequest.Create(url);
                var webResponse = await webRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                        Console.WriteLine(reader.ReadToEnd());
                }
        }
        
        public static void TestAsync4()
        {
                Task t = WriteWebRequestSizeAsync("https://www.unity.com");
                while (t.Wait(100)==false)
                {
                        Console.Write(".");
                }
        }
        
        //并行迭代
        public static void TestParallel()
        {
                var numbers = Enumerable.Range(0, 100);
                Parallel.ForEach(numbers, (n) =>
                {
                        Console.WriteLine(n);
                }
                );
        }
 
 
        
        //---------------------------------------------------------------------
        //线程同步
        //---------------------------------------------------------------------
        
        static long _value = 0;
        const int _iterations = int.MaxValue;
        readonly static object _sync = new object();
        //monitor
        public static void TestMonitor()
        {
                Task task = Task.Run(Docrement);
                
                for (int i = 0; i < _iterations; i++)
                {
                       bool lockTaken = false;
                       try
                       {

                               Monitor.Enter(_sync,ref lockTaken);
                                   _value++;
                       }
                      finally
                       {
                               if (lockTaken)
                               {
                                       Monitor.Exit(_sync);
                               }
                       }
             
                }
                
                task.Wait();
                Console.WriteLine(_value);
        }
        
        public static void Docrement()
        {
                for (int i = 0; i < _iterations; i++)
                {
                        bool lockTaken = false;
                        try
                        {

                                Monitor.Enter(_sync,ref lockTaken);
                                _value--;
                        }
                        finally
                        {
                                if (lockTaken)
                                {
                                        Monitor.Exit(_sync);
                                }
                        }
                }
        }
        
        //lock
        public static void TestLock()
        {
                Task task = Task.Run(DocrementLock);
                
                for (int i = 0; i < _iterations; i++)
                {
                        lock (_sync)
                        {
                                _value++;
                        }
                }
                
                task.Wait();
                Console.WriteLine(_value);
        }
        
        public static void DocrementLock()
        {
                for (int i = 0; i < _iterations; i++)
                {
                        lock (_sync) _value--;
                }
        }
}


