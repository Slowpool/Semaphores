using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static void SantaClaus()
    {
        Thread[] threads;

        bool useReindeers = true;
        bool useElves = true;
        if (useReindeers)
        {
            threads = new Thread[Reindeer.THREADS_NUMBER];
            for (int i = 0; i < threads.Length; i++)
            {
                int ownI = i;
                threads[i] = new Thread(() =>
                {
                    Reindeer thread = new(ownI);
                    thread.Imitate();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
        }

        Thread santa = new(() =>
        {
            Santa thread = new(0);
            thread.Imitate();
        });
        santa.Start();

        if (useElves)
        {
            threads = new Thread[Elf.THREADS_NUMBER];
            for (int i = 0; i < threads.Length; i++)
            {
                int ownI = i;
                threads[i] = new Thread(() =>
                {
                    Elf thread = new(ownI);
                    thread.Imitate();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
        }
    }

    public class Reindeer(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 9;

        public static bool allReindeersArived = false;
        public static int arrivedDeers = 0;
        public static int hitchedDeers = 0;
        public static Semaphore hitch = new(0, int.MaxValue);

        public override void Prepare()
        {
            for (int i = 0; i < THREADS_NUMBER; i++)
            {

            }
        }

        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.9)
            {
                mutex.WaitOne();
                arrivedDeers++;
                Console.WriteLine($"reindeer {number} arrived. arrived reindeers: {arrivedDeers}");
                if (arrivedDeers == THREADS_NUMBER)
                {
                    allReindeersArived = true;
                    Santa.wakeUp.Release();
                }
                mutex.Release();
                GetHitched();
            }
        }

        public void GetHitched()
        {
            hitch.WaitOne();

            mutex.WaitOne();
            hitchedDeers++;
            Console.WriteLine($"reindeer {number} got hitched. hitched reindeers: {hitchedDeers}");
            if (hitchedDeers == THREADS_NUMBER)
            {
                hitchedDeers = 0;
                Santa.@return.Release();
            }
            mutex.Release();
        }
    }

    public class Santa(int number) : MockThread(number)
    {
        public static Semaphore wakeUp = new(0, int.MaxValue);
        public static Semaphore @return = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            wakeUp.WaitOne();

            if (Reindeer.allReindeersArived)
            {
                mutex.WaitOne();
                Reindeer.hitchedDeers = 0;
                Reindeer.arrivedDeers = 0;
                Reindeer.allReindeersArived = false;
                mutex.Release();

                PrepareSleigh();
            }
            else if (Elf.elvesAreReadyToGetHelp)
            {
                // grabs as much elves as he can
                mutex.WaitOne();
                Elf.elvesRemainToHelp = Elf.elvesAwaitingForHelp;
                Elf.elvesAwaitingForHelp = 0;
                Elf.elvesAreReadyToGetHelp = false;
                mutex.Release();

                HelpElves();
            }
            else
                Console.WriteLine("ERROR santa awakened by nothing");
        }

        public void PrepareSleigh()
        {
            Console.WriteLine("santa gonna prepare sleigh");
            for (int i = 0; i < Reindeer.THREADS_NUMBER; i++)
            {
                Reindeer.hitch.Release();
            }
            @return.WaitOne();
            Console.WriteLine("CHRISTMASS");
        }

        public void HelpElves()
        {
            Console.WriteLine("santa gonna help elves");

            mutex.WaitOne();
            for (int i = Elf.elvesRemainToHelp; i != 0; i--)
            {
                Elf.help.Release();
                Console.WriteLine("one elf got help");
            }
            mutex.Release();

            @return.WaitOne();
            Console.WriteLine("HELP COMPLITED");
        }
    }

    public class Elf(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 10;
        public const int ELVES_TO_GET_HELP = 3;
        public static bool elvesAreReadyToGetHelp = false;
        public static Semaphore help = new(0, int.MaxValue);
        public static int elvesAwaitingForHelp = 0;
        public static int elvesRemainToHelp = 0;
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.9)
            {
                mutex.WaitOne();
                elvesAwaitingForHelp++;
                Console.WriteLine($"elf {number} needs help. elves need help: {elvesAwaitingForHelp}");
                if (!elvesAreReadyToGetHelp && elvesAwaitingForHelp == ELVES_TO_GET_HELP)
                {
                    elvesAreReadyToGetHelp = true;
                    Santa.wakeUp.Release();
                }
                mutex.Release();
                GetHelp();
            }
        }

        public void GetHelp()
        {
            help.WaitOne();
            Thread.Sleep(1000);

            mutex.WaitOne();
            elvesRemainToHelp--;
            Console.WriteLine($"elf {number} got help");
            // all elves who needed the help got it
            if (elvesRemainToHelp == 0)
            {
                Santa.@return.Release();
            }
            mutex.Release();

        }
    }
}
