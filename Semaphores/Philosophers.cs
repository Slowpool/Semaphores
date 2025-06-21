using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static void Philosophers()
    {
        Thread[] threads = new Thread[PhilosopherThread.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                PhilosopherThread thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class PhilosopherThread(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 5;

        public static Semaphore[] sems = new Semaphore[THREADS_NUMBER];
        public static bool[] forkIsTaken = new bool[THREADS_NUMBER];

        private int LeftForkNumber
            => number;
        private int RightForkNumber
            => (number + 1) % THREADS_NUMBER;

        private Semaphore LeftFork
            => sems[LeftForkNumber];

        private Semaphore RightFork
            => sems[RightForkNumber];

        private bool CanTakeForks
            => !forkIsTaken[LeftForkNumber] && !forkIsTaken[RightForkNumber];

        public override void Prepare()
        {
            for (int i = 0; i < THREADS_NUMBER; i++)
            {
                sems[i] = new(1, int.MaxValue);
                forkIsTaken[i] = false;
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
            bool canTakeForks;
            do
            {
                mutex.WaitOne();
                canTakeForks = CanTakeForks;
                if (canTakeForks)
                {
                    TakeForks();
                }
                else
                {
                    Console.WriteLine($"thread {number} wanted to take forks, but failed");
                }
                mutex.Release();

                Thread.Sleep(100);
            }
            while (!canTakeForks);
        }

        public void TakeForks()
        {
            Console.WriteLine($"thread {number} gonna take forks");
            LeftFork.WaitOne();
            RightFork.WaitOne();
            forkIsTaken[LeftForkNumber] = true;
            forkIsTaken[RightForkNumber] = true;
            Console.WriteLine("thread {0} took forks. current forks state: {1}", number, string.Join(", ", forkIsTaken.Select((value, i) =>
            $"[{i}] => {value.ToString().ToUpper()}")));
        }

        public void Eat()
        {
            Console.WriteLine($"thread {number} eats");
        }

        public void PutForks()
        {
            mutex.WaitOne();

            Console.WriteLine($"thread {number} gonna put forks");
            LeftFork.Release();
            RightFork.Release();
            forkIsTaken[LeftForkNumber] = false;
            forkIsTaken[RightForkNumber] = false;
            Console.WriteLine($"thread {number} put forks");

            mutex.Release();
        }


    }
}
