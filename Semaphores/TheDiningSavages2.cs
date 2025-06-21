using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static void TheDiningSavages2()
    {
        Thread[] threads = new Thread[SavageThread2.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                SavageThread2 thread = new(ownI);
                thread.Imitate();
            });
        }

        new Thread(() =>
        {
            new CookThread2(0).Imitate();
        }).Start();

        foreach (var thread in threads)
        {
            thread.Start();
        }

    }

    public class SavageThread2(int number) : MockThread(number)
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
            mutex.WaitOne();
            if (pot.Count == 0)
            {
                CookThread2.sem.Release();
                SavageThread2.sem.WaitOne();
            }
        }

        public void Eat()
        {
            Console.WriteLine($"thread {number} ate. pot servings remained: {pot.Count}");
            pot.Pop();
            mutex.Release();
        }
    }

    public class CookThread2(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            sem.WaitOne();
            putServingsInPot();
            SavageThread2.sem.Release();
        }

        public void putServingsInPot()
        {
            for (int i = 0; i < POT_CAPACITY; i++)
            {
                Thread.Sleep(100);
                pot.Push(_);
                Console.WriteLine($"the cook put some food. pot servings: {pot.Count}");
            }
        }
    }

}
