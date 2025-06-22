using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static void H2O_Second()
    {
        Thread[] threads;
        threads = new Thread[HSecondThread.NUMBER_OF_THREADS];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                HSecondThread thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        threads = new Thread[OSecondThread.NUMBER_OF_THREADS];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                OSecondThread thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class HSecondThread(int number) : MockThread(number)
    {
        public const int NUMBER_OF_THREADS = OSecondThread.NUMBER_OF_THREADS * 2;
        public static Semaphore sem = new(0, int.MaxValue);
        public override void Prepare()
        {
            for (int i = 0; i < NUMBER_OF_THREADS; i++)
            {

            }
        }

        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.5)
            {
                Console.WriteLine($"H_{number} arrived");
                Thread.Sleep(2000);

                sem.WaitOne();

                mutex.WaitOne();
                OSecondThread.queue.Peek()
                                   .Release();
                mutex.Release();

                Bond();
            }
        }

        public void Bond()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(300);
                Console.WriteLine($"BOND H_{number}");
            }
            Console.WriteLine($"COMPLETED H_{number}");
        }
    }

    public class OSecondThread(int number) : MockThread(number)
    {
        public const int NUMBER_OF_THREADS = 1;
        public Semaphore sem = new(0, int.MaxValue);
        public static Queue<Semaphore> queue = new();
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.5)
            {
                Console.WriteLine($"O_{number} arrived");
                Thread.Sleep(2000);

                mutex.WaitOne();
                queue.Enqueue(sem);
                mutex.Release();
                
                // take two random hydrogens
                HSecondThread.sem.Release();
                HSecondThread.sem.Release();
                // rendezvous
                sem.WaitOne();
                sem.WaitOne();
                
                mutex.WaitOne();
                // take himself out
                queue.Dequeue();
                mutex.Release();
                
                Bond();
            }
        }

        public void Bond()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(300);
                Console.WriteLine($"BOND O_{number}");
            }
            Console.WriteLine($"COMPLETED O_{number}");
        }
    }
}
