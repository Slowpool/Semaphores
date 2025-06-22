using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static void H2O()
    {
        Thread[] threads;
        threads = new Thread[HThread.NUMBER_OF_THREADS];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                HThread thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        threads = new Thread[OThread.NUMBER_OF_THREADS];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                OThread thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public class HThread(int number) : MockThread(number)
    {
        public const int NUMBER_OF_THREADS = OThread.NUMBER_OF_THREADS * 2;
        public static int hydrogens = 0;
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
                mutex.WaitOne();
                hydrogens++;
                Console.WriteLine($"H_{number} arrived. hydrogens: {hydrogens}");
                // TODO it should check the oxygen state.
                if (hydrogens == 2)
                {
                    hydrogens = 0;
                    
                    mutex.Release();
                    // release the second waiting hydrogen
                    sem.Release();
                    // release the waiting oxygen
                    OThread.sem.Release();
                }
                else
                {
                    mutex.Release();
                    sem.WaitOne();
                }
                Bond();
                Thread.Sleep(1000);
            }
        }

        public void Bond()
        {
            Console.WriteLine($"BOND H_{number}");
        }
    }

    public class OThread(int number) : MockThread(number)
    {
        public const int NUMBER_OF_THREADS = 3;
        public static Semaphore sem = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            if (Random.Shared.NextDouble() > 0.5)
            {

                mutex.WaitOne();
                Console.WriteLine($"O_{number} arrived");
                if (HThread.hydrogens == 2)
                {
                    Console.WriteLine($"2 hydrogens detected");
                    mutex.Release();
                }
                else
                {
                    Console.WriteLine($"O_{number} is waiting for 2 hydrogens");
                    mutex.Release();
                    sem.WaitOne();
                }

                Bond();

                Thread.Sleep(1000);
            }
        }

        public void Bond()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(300);
                Console.WriteLine($"BOND O_{number}");
            }
        }
    }
}
