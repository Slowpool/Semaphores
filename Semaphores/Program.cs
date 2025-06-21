using Semaphores;
using System.Threading;

namespace Semaphores;

public partial class Program
{
    static void Main()
    {
        //TwoBarriers1();
        //TwoBarriers2();
        //Queue();
        //Queue2();
        //FifoMeth();
        //AuthorsFifoMeth();
        // ProducerAndConsumer();
        // ReadersAndWriters();
        // ReadersAndWriters2();
        // ReadersAndWriters3();
        // ReadersAndWriters4();
        // ExclusionProblem();
        // ExclusionProblem2();
        // ExclusionProblem3();
        // Philosophers();
        // Philosophers2();
        // Philosophers3();
        // CigarettesAndSmokers();
        // GeneralizedCigarettesAndSmokers();
        // TheDiningSavages();
        // TheDiningSavages2();
        // TheBarbershopProblem();
        // TheHarderBarbershopProblem();
        SantaClaus();
    }
    const string ThreadA = "A";
    const string ThreadB = "B";
    const string ThreadC = "C";
    protected static Semaphore pictureMutex = new(1, int.MaxValue);

    static void TwoBarriers2()
    {
        Thread[] threads =
        [
            new Thread(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    PrintContinuously($"{ThreadA}1");

                    mutex.WaitOne();
                    if (++count == n)
                        turnstile1.Release(n);
                    mutex.Release();

                    turnstile1.WaitOne();

                    PrintContinuously($"{ThreadA}2");

                    mutex.WaitOne();
                    if (--count == 0)
                        turnstile2.Release(n);
                    mutex.Release();
                    turnstile2.WaitOne();
                }
            }),
            new Thread(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    PrintContinuously($"{ThreadB}1");

                    mutex.WaitOne();
                    if (++count == n)
                        turnstile1.Release(n);
                    mutex.Release();

                    turnstile1.WaitOne();

                    PrintContinuously($"{ThreadB}2");

                    mutex.WaitOne();
                    if (--count == 0)
                        turnstile2.Release(n);
                    mutex.Release();
                    turnstile2.WaitOne();
                }
            }),
            new Thread(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    PrintContinuously($"{ThreadC}1");

                    mutex.WaitOne();
                    if (++count == n)
                        turnstile1.Release(n);
                    mutex.Release();

                    turnstile1.WaitOne();

                    PrintContinuously($"{ThreadC}2");

                    mutex.WaitOne();
                    if (--count == 0)
                        turnstile2.Release(n);
                    mutex.Release();
                    turnstile2.WaitOne();
                }
            })
        ];
        foreach (var thread in threads)
            thread.Start();
    }
    static void TwoBarriers1()
    {
        #region previous life
        ////////////Semaphore sem = new(0, 2);
        ////////////string line = "fail";

        ////////////Thread reader = new(() =>
        ////////////{
        ////////////    Console.WriteLine("Reading started...");
        ////////////    Thread.Sleep(3000);
        ////////////    line = "line";
        ////////////    sem.Release();
        ////////////});

        ////////////Thread writer = new(() =>
        ////////////{
        ////////////    sem.WaitOne();
        ////////////    Console.WriteLine(line);
        ////////////});

        ////////////writer.Start();
        ////////////reader.Start();

        //////////Semaphore sem = new(0, 2);

        //////////Thread A = new(() =>
        //////////{
        //////////    sem.WaitOne();
        //////////    Console.WriteLine("A1");
        //////////    sem.Release();
        //////////    Console.WriteLine("A2");
        //////////});

        //////////Thread B = new(() =>
        //////////{
        //////////    Console.WriteLine("B1");
        //////////    sem.Release();
        //////////    sem.WaitOne();
        //////////    Console.WriteLine("B2");
        //////////});

        //////////B.Start();
        //////////A.Start();

        ////////Semaphore aArrived = new(0, 2);
        ////////Semaphore bArrived = new(0, 2);

        ////////Thread A = new(() =>
        ////////{
        ////////    Console.WriteLine("A1");
        ////////    aArrived.Release();
        ////////    bArrived.WaitOne();
        ////////    Console.WriteLine("A2");
        ////////});

        ////////Thread B = new(() =>
        ////////{
        ////////    Console.WriteLine("B1");
        ////////    bArrived.Release();
        ////////    aArrived.WaitOne();
        ////////    Console.WriteLine("B2");
        ////////});

        ////////B.Start();
        ////////A.Start();


        //////Semaphore sem1 = new(0, 4);
        //////Semaphore sem2 = new(0, 4);
        //////Semaphore mutex = new(1, 3);

        //////int count = 0;

        //////Thread A = new(() =>
        //////{
        //////    #region MyRegion
        //////    sem1.Release();
        //////    sem2.WaitOne();
        //////    #endregion

        //////    mutex.WaitOne();
        //////    Console.WriteLine($"A1: {++count}");
        //////    mutex.Release();

        //////});

        //////Thread B = new(() =>
        //////{
        //////    #region MyRegion
        //////    sem2.Release();
        //////    sem1.WaitOne();
        //////    #endregion

        //////    mutex.WaitOne();
        //////    Console.WriteLine($"B1: {++count}");
        //////    mutex.Release();

        //////});

        //////A.Start();
        //////B.Start();

        ////Semaphore mutex = new(2, 4);

        ////Thread A = new(() =>
        ////{
        ////    mutex.WaitOne();
        ////    for (int i = 0; i < 10; i++)
        ////    {
        ////        Thread.Sleep(300);
        ////        Console.WriteLine($"A1");
        ////    }
        ////    mutex.Release();

        ////});

        ////Thread B = new(() =>
        ////{
        ////    mutex.WaitOne();
        ////    for (int i = 0; i < 10; i++)
        ////    {
        ////        Thread.Sleep(100);
        ////        Console.WriteLine($"B1");
        ////    }
        ////    mutex.Release();

        ////});

        ////Thread C = new(() =>
        ////{
        ////    mutex.WaitOne();
        ////    for (int i = 0; i < 10; i++)
        ////    {
        ////        Thread.Sleep(100);
        ////        Console.WriteLine($"C1");
        ////    }
        ////    mutex.Release();

        ////});

        ////A.Start();
        ////B.Start();
        ////C.Start();

        //Semaphore barrier = new(0, 4);
        //Semaphore mutex = new(1, 4);
        //const int n = 3;
        //int count = 0;

        //Thread A = new(() =>
        //{
        //    PrintContinuously($"A1");

        //    mutex.WaitOne();
        //    ++count;
        //    mutex.Release();
        //    if (count == n)
        //        barrier.Release();
        //    barrier.WaitOne();
        //    barrier.Release();

        //    PrintContinuously($"A2");
        //});

        //Thread B = new(() =>
        //{
        //    PrintContinuously($"B1");

        //    mutex.WaitOne();
        //    ++count;
        //    mutex.Release();

        //    if (count == n)
        //        barrier.Release();
        //    barrier.WaitOne();
        //    barrier.Release();

        //    PrintContinuously($"B2");
        //});

        //Thread C = new(() =>
        //{
        //    PrintContinuously($"C1");

        //    mutex.WaitOne();
        //    ++count;
        //    mutex.Release();
        //    if (count == n)
        //        barrier.Release();
        //    barrier.WaitOne();
        //    barrier.Release();

        //    PrintContinuously($"C2");
        //});

        //A.Start();
        //B.Start();
        //C.Start(); 
        #endregion

        Thread[] threads = new Thread[3];
        for (int i = 0; i < threads.Length; i++)
        {
            int currentI = i;
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    PrintContinuously($"{ThreadNames[currentI]}1");

                    AfterRendezvous();

                    PrintContinuously($"{ThreadNames[currentI]}2");

                    AfterCriticalSection();
                }
            });
        }

        foreach (var thread in threads)
            thread.Start();

        Console.ReadKey();
    }

    static void Queue()
    {
        Thread[] threads = new Thread[3];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[ownI] = new Thread(() =>
            {
                QueueBefore(ownI);
                PrintContinuously($"{ownI}");
                QueueAfter(ownI);

            });
        }
        foreach (var thread in threads)
            thread.Start();
    }

    static int lastFinishedThread = 0;
    static Semaphore queue = new Semaphore(0, 2);
    static void QueueBefore(int threadNumber)
    {
        mutex.WaitOne();
        if (lastFinishedThread == threadNumber)
            mutex.Release();
        else
        {
            mutex.Release();
            queue.WaitOne();
        }
    }

    static void QueueAfter(int threadNumber)
    {
        mutex.WaitOne();
        lastFinishedThread = threadNumber; // or +
        mutex.Release();
        queue.Release();
    }

    public static Semaphore turnstile1 = new(0, 4);
    public static Semaphore turnstile2 = new(0, 4);
    public static Semaphore mutex = new(1, 4);

    public const int n = 3;
    public static int count = 0;

    //public static string[] ThreadNames = ["A", "B", "C"];
    public static string[] ThreadNames = ["", "", ""];
    public static void AfterRendezvous()
    {
        mutex.WaitOne();
        if (++count == n)
        {
            turnstile1.Release(n);
        }
        mutex.Release();

        turnstile1.WaitOne();
    }

    public static void AfterCriticalSection()
    {
        mutex.WaitOne();
        if (--count == 0)
        {
            turnstile2.Release(n);
        }
        mutex.Release();
        turnstile2.WaitOne();
    }

    public static void PrintContinuously(string message, int delay = 0)
    {
        for (int i = 0; i < 2; i++)
        {
            Thread.Sleep(delay);
            Console.Write(message);
        }
    }
}
