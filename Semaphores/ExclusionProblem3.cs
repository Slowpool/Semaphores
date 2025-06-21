using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static void ExclusionProblem3()
    {
        Thread[] threads = new Thread[Subject2.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                Subject3 thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class Subject3(int number) : MockThread(number)
    {
        public static int Room1, Room2 = 0;

        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.4)
            {
                mutex.WaitOne();
                Room1++;
                Console.WriteLine($"thread {number} entered room1. current: {Room1}");
                mutex.Release();

                t1.WaitOne();
                Console.WriteLine($"thread {number} passed t1");
                Room2++;
                Console.WriteLine($"thread {number} entered room2. current: {Room2}");

                mutex.WaitOne();
                Room1--;
                Console.WriteLine($"thread {number} left room1. current: {Room1}");

                if (Room1 == 0)
                {
                    Console.WriteLine($"thread {number} see room1 as empty");
                    mutex.Release();
                    t2.Release();
                    Console.WriteLine($"thread {number} opened t2");
                }
                else
                {
                    mutex.Release();
                    t1.Release();
                    Console.WriteLine($"thread {number} opened t1");
                }

                t2.WaitOne();
                Room2--;
                Console.WriteLine($"thread {number} left room2. current: {Room2}");

                Action();

                if (Room2 == 0)
                {
                    t1.Release();
                    Console.WriteLine($"thread {number} opened t1");
                }
                else
                {
                    t2.Release();
                    Console.WriteLine($"thread {number} opened t2");
                }
            }
        }

        public void Action()
        {
            // mutex.WaitOne();
            Console.WriteLine($"thread {number} reads");
            // mutex.Release();
        }
    }
}
