using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static void TheBarbershopProblem()
    {
        Thread[] threads = new Thread[CustomerThread.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                CustomerThread thread = new(ownI);
                thread.Imitate();
            });
        }

        new Thread(() =>
        {
            new BarberThread(0).Imitate();
        }).Start();

        foreach (var thread in threads)
        {
            thread.Start();
        }

    }

    public class CustomerThread(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 5;
        public const int WAITING_ROOM_SIZE = 3;
        public const int BARBER_ROOM_SIZE = 1; // only one barber yet

        public static Semaphore sem = new(0, int.MaxValue);
        public static Semaphore barber = new(1, int.MaxValue);

        public static int numberOfCustomers = 0;

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
                mutex.WaitOne();
                if (numberOfCustomers == WAITING_ROOM_SIZE + BARBER_ROOM_SIZE)
                {
                    Console.WriteLine($"there was max number of customers in room. thread {number} balked");
                    mutex.Release();
                    return;
                }
                numberOfCustomers++;
                Console.WriteLine($"thread {number} sat. number of waiting customers: {numberOfCustomers}");
                mutex.Release();
                GetHairCut();
            }
        }

        public void GetHairCut()
        {
            BarberThread.customer.Release();
            barber.WaitOne();
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(100);
                Console.WriteLine($"thread {number} is getting haircut");
            }
            mutex.WaitOne();
            numberOfCustomers--;
            mutex.Release();
            // queue.Release();
        }
    }

    public class BarberThread(int number) : MockThread(number)
    {
        public static Semaphore customer = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            customer.WaitOne();
            MakeHairCut();
            CustomerThread.barber.Release();
        }

        public void MakeHairCut()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(100);
                Console.WriteLine($"barber makes a haircut");
            }
        }
    }

}
