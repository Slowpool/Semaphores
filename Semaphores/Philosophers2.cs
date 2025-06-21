using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static void Philosophers2()
    {
        Thread[] threads = new Thread[PhilosopherThread2.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                PhilosopherThread2 thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class PhilosopherThread2(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 2;

        public static Semaphore[] sems = new Semaphore[THREADS_NUMBER];
        public static Semaphore activeThreads = new Semaphore(THREADS_NUMBER, int.MaxValue);

        private int LeftForkNumber
            => number;
        private int RightForkNumber
            => (number + 1) % THREADS_NUMBER;

        private Semaphore LeftFork
            => sems[LeftForkNumber];

        private Semaphore RightFork
            => sems[RightForkNumber];

        public override void Prepare()
        {
            for (int i = 0; i < THREADS_NUMBER; i++)
            {
                sems[i] = new(1, int.MaxValue);
            }
        }

        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.4)
            {
                Think();
                GetForks();
                Eat();
                PutForks();
            }
        }

        public void Think()
        {
            Console.WriteLine($"thread {number} thinks");
        }

        public void GetForks()
        {
            activeThreads.WaitOne();
            TakeForks();
        }

        public void TakeForks()
        {
            Console.WriteLine($"thread {number} gonna take forks");
            LeftFork.WaitOne();
            RightFork.WaitOne();
            Console.WriteLine($"thread {number} took forks");
        }

        public void Eat()
        {
            Console.WriteLine($"thread {number} eats");
        }

        public void PutForks()
        {
            Console.WriteLine($"thread {number} gonna put forks");
            LeftFork.Release();
            RightFork.Release();
            Console.WriteLine($"thread {number} put forks");

            activeThreads.Release();
        }
    }
}
