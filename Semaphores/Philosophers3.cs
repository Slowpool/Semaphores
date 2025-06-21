using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static void Philosophers3()
    {
        Thread[] threads = new Thread[PhilosopherThread3.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                PhilosopherThread3 thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class PhilosopherThread3(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 4;

        static Semaphore[] sems = new Semaphore[THREADS_NUMBER];

        const string THINKS_STATUS = "thinks";
        const string HUNGRY_STATUS = "hungry";
        const string EATS_STATUS = "eats";
        const string FULL_STATUS = "full";

        static string[] threadStatuses = new string[THREADS_NUMBER];
        // int LeftForkNumber
        //     => Left(number);
        // int RightForkNumber
        //     => Right(number);

        // Semaphore LeftFork
        //     => sems[LeftForkNumber];

        // Semaphore RightFork
        //     => sems[RightForkNumber];

        // static int Left(int i)
        //     => i;

        // static int Right(int i)
        //     => (i + 1) % THREADS_NUMBER;

        int LeftNeighbour(int i)
            => (i + THREADS_NUMBER - 1) % THREADS_NUMBER;

        int RightNeighbour(int i)
            => (i + 1) % THREADS_NUMBER;

        public override void Prepare()
        {
            for (int i = 0; i < THREADS_NUMBER; i++)
            {
                sems[i] = new(0, int.MaxValue);
                threadStatuses[i] = THINKS_STATUS;
            }
        }

        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.4)
            {
                Picture();
                Think();
                Picture();
                GetForks();
                Picture();
                Eat();
                Picture();
                PutForks();
            }
        }

        void Think()
        {
            mutex.WaitOne();
            Console.WriteLine($"thread {number} thinks");
            mutex.Release();
        }

        void GetForks()
        {
            mutex.WaitOne();
            threadStatuses[number] = HUNGRY_STATUS;
            Test(number);
            mutex.Release();

            sems[number].WaitOne();
        }

        void Eat()
        {
            mutex.WaitOne();
            threadStatuses[number] = EATS_STATUS;
            Console.WriteLine($"thread {number} actually eats");
            mutex.Release();
        }

        void PutForks()
        {
            mutex.WaitOne();
            threadStatuses[number] = THINKS_STATUS;

            Test(LeftNeighbour(number));
            Test(RightNeighbour(number));
            
            mutex.Release();
        }

        void Test(int i)
        {
            if (threadStatuses[i] == HUNGRY_STATUS && threadStatuses[LeftNeighbour(i)] != EATS_STATUS && threadStatuses[RightNeighbour(i)] != EATS_STATUS)
            {
                threadStatuses[i] = EATS_STATUS;
                Console.WriteLine($"test passed for thread {i}: {threadStatuses[i]}. EXPECTED: eats");
                sems[i].Release();
                Console.WriteLine($"{i} sem released");
            }
            else
            {
                Console.WriteLine($"test failed for thread {i}. left neighbour status: {threadStatuses[LeftNeighbour(i)]}. right neighbour status: {threadStatuses[RightNeighbour(i)]}");
            }
        }

        void Picture()
        {
            mutex.WaitOne();
            Console.Write("THREADS: ");
            for (int i = 0; i < THREADS_NUMBER; i++)
            {
                Console.Write($"{i}: {threadStatuses[i]} \t");
            }
            Console.WriteLine();
            mutex.Release();
        }
    }
}
