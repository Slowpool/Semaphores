using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public const int POT_CAPACITY = 5;
    public const bool _ = true;
    public static Stack<bool> pot = [];
    public static void TheDiningSavages()
    {
        Thread[] threads = new Thread[SavageThread.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                SavageThread thread = new(ownI);
                thread.Imitate();
            });
        }

        new Thread(() =>
        {
            new CookThread(0).Imitate();
        }).Start();

        foreach (var thread in threads)
        {
            thread.Start();
        }

    }

    public class SavageThread(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 5;

        public static Semaphore sem = new(0, int.MaxValue);
        public static Semaphore queue = new(1, int.MaxValue);

        public override void Prepare()
        {
            for (int i = 0; i < THREADS_NUMBER; i++)
            {

            }
        }

        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.4)
            {
                GetServingFromPot();
                Eat();
            }
        }

        public void GetServingFromPot()
        {
            queue.WaitOne();
            mutex.WaitOne();
            if (pot.Count == 0)
            {
                mutex.Release();
                CookThread.sem.Release();
                SavageThread.sem.WaitOne();
            }
            else
            {
                mutex.Release();
            }
        }

        public void Eat()
        {
            pot.Pop();
            Console.WriteLine($"thread {number} ate. pot servings remained: {pot.Count}");
            queue.Release();
        }
    }

    public class CookThread(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            sem.WaitOne();
            putServingsInPot();
            SavageThread.sem.Release();
        }

        public void putServingsInPot()
        {
            mutex.WaitOne();
            for (int i = 0; i < POT_CAPACITY; i++)
            {
                Thread.Sleep(100);
                pot.Push(_);
                Console.WriteLine($"the cook put some food. pot servings: {pot.Count}");
            }
            mutex.Release();
        }
    }

}
