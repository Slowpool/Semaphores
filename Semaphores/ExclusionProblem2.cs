using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static int Room1, Room2 = 0;


    public static void ExclusionProblem2()
    {
        Thread[] threads = new Thread[Subject2.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                Subject2 thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class Subject2(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 4;
        public static int previous;
        public const int NULL = -1;
        private static int _Room1 = 0;
        public static int Room1
        {
            get => _Room1;
            set
            {
                if (value == 0)
                {
                    previous = NULL;
                }
                _Room1 = value;
            }
        }
        private static int _Room2 = 0;
        public static int Room2
        {
            get => _Room2;
            set
            {
                if (value == 0)
                {
                    previous = NULL;
                }
                _Room2 = value;
            }
        }
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.4)
            {
                PassRooms();
                Action();
            }
        }

        private void PassRooms()
        {
            Console.WriteLine($"thread {number} before t1");
            t1.WaitOne();
            Console.WriteLine($"thread {number} passed t1");
            t1.Release();

            mutex.WaitOne();
            Room1++;
            Console.WriteLine($"thread {number} entered room1. current: {Room1}");
            if (Room1 == THREADS_NUMBER)
            {
                Console.WriteLine("room1 closing...");
                Room1 = 0;
                t1.WaitOne();
                Console.WriteLine("room1 closed...");
                t2.Release();
                Console.WriteLine("room2 opened...");
            }
            mutex.Release();

            Console.WriteLine($"thread {number} before t2");
            t2.WaitOne();
            Console.WriteLine($"thread {number} passed t2");
            t2.Release();

            mutex.WaitOne();
            Room2++;
            Console.WriteLine($"thread {number} entered room2. current: {Room2}");
            if (Room2 == THREADS_NUMBER)
            {
                Console.WriteLine("room2 closing...");
                Room2 = 0;
                t2.WaitOne();
                Console.WriteLine("room2 closed...");
                t1.Release();
                Console.WriteLine("room1 opened...");
            }
            mutex.Release();
        }

        public void Action()
        {
            mutex.WaitOne();
            if (previous == number)
            {
                Console.WriteLine("property 3 violation");
            }
            Console.WriteLine($"thread {number} reads");
            previous = number;
            mutex.Release();
        }
    }
}
