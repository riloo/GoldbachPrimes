using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoldbachPrimes
{
    class Program      
    {
        private static int currentNum = 4;
        private static int lastChecked = 0;
        private static Boolean exit = false;
        private static ConsoleKeyInfo enter;

        static void Main(string[] args)
        {
            Console.WriteLine("What number do you want to go up to?");
            int n = Int32.Parse(Console.ReadLine());

            Console.WriteLine("How many threads do you want to use?");
            int threads = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Calculating... \nPress Enter to cancel\n");

            getSums(n, threads);

            //try
            //{
            //getSums(n, threads);
            //}catch(Exception e)
            //{
            //    Console.WriteLine("Finished.");
            //}

            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        }

        static void getSums(int n, int threads)
        {
            Dictionary<int, Tuple<int, int>> goldbachPrimes = new Dictionary<int, Tuple<int, int>>();
            List<int> savedPrimes = new List<int>();
            List<Task> tasks = new List<Task>();

            var cts = new CancellationTokenSource();

            Task task = Task.Run(() =>
            {
                enter = Console.ReadKey();
               
            });

            Object lockCurrentNum = new Object();
            Object lockLastPrime = new Object();
            Object lockSavedPrimes = new Object();
            Object lockgoldbachPrimes = new Object();

            for (int i = 0; i < threads; i++)
            {
                Console.WriteLine("creating thread #" + (i + 1));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    int myNum; //num that im checking right now
                    do
                    {

                        lock (lockCurrentNum)
                        {
                            myNum = currentNum; //go to next number that i want to check
                            currentNum += 2;
                        }
                        if (myNum > n) break;

                        if (myNum > lastChecked)
                        {
                            //find primes between last prime that was checked untill the num im up to now
                            for (int j = lastChecked; j < myNum; j++)
                            {
                                if (IsPrime(j))
                                {
                                    lock (lockSavedPrimes)
                                    {
                                        if (!savedPrimes.Contains(j))
                                            savedPrimes.Add(j);
                                    }
                                }
                                lock (lockLastPrime)
                                {
                                    lastChecked = myNum;
                                }
                            }
                        }
                        for (int p = 0; p < savedPrimes.Count; p++)
                        {
                            int p2 = myNum - p;
                            if (savedPrimes.Contains(p2))
                            {
                                lock (lockgoldbachPrimes)
                                {
                                    //add num with primes to dicrionary
                                    goldbachPrimes[myNum] = Tuple.Create(p, p2);
                                }
                                break;
                            }
                        }
                        if (enter.Key == ConsoleKey.Enter)
                        {
                            exit = true;
                        }

                    } while (myNum + 2 <= n && !exit);
                }));
            }
            //if (!cts.IsCancellationRequested)
            //{
            //    Console.WriteLine("not cancelled");
            //    // throw new TaskCanceledException();
            //    cts.Cancel();
            //    cts.Dispose();
            //}


            Task.WaitAll(tasks.ToArray());
            foreach (KeyValuePair<int, Tuple<int, int>> k in goldbachPrimes.OrderBy(s => s.Key).ToList())
            {
                Console.WriteLine(" {0} + {1} = {2}", k.Value.Item1, k.Value.Item2, k.Key);
            }         

        }

        static Boolean IsPrime(int n)
        {
            Boolean result = true;
            if (n == 1)
                return false;
            if (n == 2)
                return true;
            if (n % 2 == 0)
                return false;

            for (int i = 3; i < Math.Sqrt(n); i++)
            {
                if (n % i == 0)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }


    }
}
