using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static int room1 = 0;
    public static int room2 = PREV_ROOM;
    public static Semaphore t1 = new(1, int.MaxValue);
    public static Semaphore t2 = new(0, int.MaxValue);
    public static int threadsCount = 3;
    public const int PREV_ROOM = int.MinValue;

    public static void ExclusionProblem()
    {
        Thread[] threads = new Thread[4];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                Subject thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class Subject(int number) : MockThread(number)
    {
        public static int previous;
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.4)
            {
                mutex.WaitOne();
                if (StarvingCheck())
                {
                    JoinSomeRoom();
                    Action();
                }
                else
                    SwitchRoom();
                mutex.Release();
            }
        }

        public bool StarvingCheck()
            => room1 != threadsCount && room2 != threadsCount;

        public void Action()
        {
            StarvingTest();
            // mutex.WaitOne();
            Console.WriteLine($"thread {number} reads");
            // mutex.Release();
        }

        public void StarvingTest()
        {
            if (previous == number)
                Console.WriteLine("property 3 violation");
            previous = number;
        }

        public void SwitchRoom()
        {
            if (room1 == threadsCount)
                SwitchToRoom2();
            else if (room2 == threadsCount)
                SwitchToRoom1();
            else
                Console.WriteLine("ERROR. switch to unknown room");
        }

        public void SwitchToRoom2()
        {
            room1 = PREV_ROOM;
            Console.WriteLine("room 1 emptied");

            t2.Release();
            Console.WriteLine("room 2 opened");

            t1.WaitOne();
            Console.WriteLine("room 1 closed");
        }

        public void SwitchToRoom1()
        {
            room2 = PREV_ROOM;
            Console.WriteLine("room 2 emptied");

            t1.Release();
            Console.WriteLine("room 1 opened");

            t2.WaitOne();
            Console.WriteLine("room 2 closed");

        }

        public void JoinSomeRoom()
        {
            if (room2 == PREV_ROOM)
            {
                Console.WriteLine("room 2 is prev");
                room2 = 0;
                JoinRoom1();
            }
            else if (room1 == PREV_ROOM)
            {
                Console.WriteLine("room 1 is prev");
                room1 = 0;
                JoinRoom2();
            }
            else if (room1 != 0 && room2 == 0)
            {
                Console.WriteLine("room 1 is not empty, room 2 is empty");
                JoinRoom1();
            }
            else if (room2 != 0 && room1 == 0)
            {
                Console.WriteLine("room 2 is not empty, room 1 is empty");
                JoinRoom2();
            }
        }

        private void JoinRoom1()
        {
            room1++;
            Console.WriteLine($"room1++. current: {room1}");
        }

        private void JoinRoom2()
        {
            room2++;
            Console.WriteLine($"room2++. current: {room2}");
        }

    }
}
