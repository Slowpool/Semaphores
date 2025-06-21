using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static Semaphore cashRegister = new(1, int.MaxValue);

    public static void TheHarderBarbershopProblem()
    {
        Thread[] threads = new Thread[HarderCustomerThread.THREADS_NUMBER];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            threads[i] = new Thread(() =>
            {
                HarderCustomerThread thread = new(ownI);
                thread.Imitate();
            });
        }

        Thread[] barbers = new Thread[HarderCustomerThread.NUMBER_OF_BARBER_CHAIRS];
        for (int i = 0; i < barbers.Length; i++)
        {
            int ownI = i;
            barbers[i] = new Thread(() =>
            {
                HarderBarberThread thread = new(ownI);
                thread.Imitate();
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var barber in barbers)
        {
            barber.Start();
        }

    }

    public class HarderCustomerThread(int number) : MockThread(number)
    {
        public const int THREADS_NUMBER = 30;
        public const int MAX_NUMBER_OF_CUSTOMERS = 20;
        public const int NUMBER_OF_BARBER_CHAIRS = 3;
        public const int NUMBER_OF_SOFA_SEATS = 4;

        public static int customersInShop = 0;
        public static int customersOnSofa = 0;
        public static int customersOnBarberChairs = 0;

        public static Queue<HarderCustomerThread> queueForSofa = new(MAX_NUMBER_OF_CUSTOMERS - NUMBER_OF_SOFA_SEATS - NUMBER_OF_BARBER_CHAIRS);
        public static Queue<HarderCustomerThread> queueForBarberChairs = new(NUMBER_OF_SOFA_SEATS);
        public static Semaphore customer = new(0, int.MaxValue);
        public static Semaphore customerPayment = new(0, int.MaxValue);

        public Semaphore waitingForSofa = new(0, int.MaxValue);
        public Semaphore waitingForChair = new(0, int.MaxValue);

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
                if (customersInShop == MAX_NUMBER_OF_CUSTOMERS)
                {
                    Console.WriteLine($"customer {number} balked. number of customers in shop: {customersInShop}");
                    mutex.Release();
                    return;
                }
                else
                {
                    EnterShop();
                    mutex.Release();
                }

                Thread.Sleep(100);
                SitOnSofa();
                Thread.Sleep(100);
                SitInBarberChair();
                Thread.Sleep(100);
                GetHaircut();
                Thread.Sleep(100);
                Pay();
                Thread.Sleep(100);
                ExitShop();
            }
        }

        public void EnterShop()
        {
            customersInShop++;
            Console.WriteLine($"customer {number} entered the shop");
        }

        public void SitOnSofa()
        {
            mutex.WaitOne();
            // all sofa places are taken
            if (customersOnSofa == NUMBER_OF_SOFA_SEATS)
            {
                queueForSofa.Enqueue(this);
                Console.WriteLine($"customer {number} is waiting for the spot on sofa. customers on sofa: {customersOnSofa}");
                mutex.Release();

                waitingForSofa.WaitOne();

                mutex.WaitOne();
                customersOnSofa++;
                Console.WriteLine($"customer {number} sat down on the sofa. customers on sofa: {customersOnSofa}");
            }
            // there's free sofa place
            else
            {
                customersOnSofa++;
                Console.WriteLine($"customer {number} sat down on the sofa. customers on sofa: {customersOnSofa}");
            }
            mutex.Release();

        }

        public void SitInBarberChair()
        {
            mutex.WaitOne();
            if (customersOnBarberChairs == NUMBER_OF_BARBER_CHAIRS)
            {
                queueForBarberChairs.Enqueue(this);
                Console.WriteLine($"customer {number} is waiting for the barber chair. customers on barber chairs: {customersOnBarberChairs}");
                mutex.Release();

                waitingForChair.WaitOne();

                mutex.WaitOne();
            }
            customersOnSofa--;
            Console.WriteLine($"customer {number} left the sofa. customers on sofa: {customersOnSofa}");
            customersOnBarberChairs++;
            Console.WriteLine($"customer {number} sat down on barber chair. customers on barber chairs: {customersOnBarberChairs}");

            if (queueForSofa.Count != 0)
            {
                var customer = queueForSofa.Dequeue();
                customer.waitingForSofa.Release();
                Console.WriteLine($"customer {customer.number} was allowed to sit on the sofa");
            }
            // // it must be commented out because the getting haircut and paying is atomic process. otherwise after getting haircut the seat may be taken by another customer which would get haircut before the current customer would paid
            mutex.Release();

        }

        public void GetHaircut()
        {
            customer.Release();
            HarderBarberThread.barber.WaitOne();
            Console.WriteLine($"customer {number} is getting haircut");
        }

        public void Pay()
        {
            customerPayment.Release();
            HarderBarberThread.barberPaymentAccept.WaitOne();
            Console.WriteLine($"customer {number} paid.");
        }

        public void ExitShop()
        {
            mutex.WaitOne();
            customersOnBarberChairs--;
            Console.WriteLine($"customer {number} left the barber chairs. customers on barber chairs: {customersOnBarberChairs}");

            if (queueForBarberChairs.Count != 0)
            {
                var customer = queueForBarberChairs.Dequeue();
                customer.waitingForChair.Release();
                Console.WriteLine($"customer {customer.number} was allowed to sit on barber chair");
            }

            customersInShop--;
            Console.WriteLine($"customer {number} exited the shop. customers in shop: {customersInShop}");

            mutex.Release();
        }
    }

    public class HarderBarberThread(int number) : MockThread(number)
    {
        public static Semaphore barber = new(0, int.MaxValue);
        public static Semaphore barberPaymentAccept = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(100);
            MakeHaircut();
            AcceptPayment();
        }

        public void MakeHaircut()
        {
            barber.Release();
            HarderCustomerThread.customer.WaitOne();
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(100);
                Console.WriteLine($"barber {number} makes a haircut");
            }
        }

        public void AcceptPayment()
        {
            HarderCustomerThread.customerPayment.WaitOne();
            barberPaymentAccept.Release();
            Console.WriteLine($"barber {number} received the payment");
        }
    }

}
