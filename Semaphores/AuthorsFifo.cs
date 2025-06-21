using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semaphores;
public partial class Program
{
    public class AuthorsFifo
    {
        Queue<Semaphore> queue = [];
        Semaphore mutex = new(1, 2);

        public void WaitOne(Semaphore semaphore)
        {
            mutex.WaitOne();
            queue.Enqueue(semaphore);
            mutex.Release();
            semaphore.WaitOne();
        }

        public void Release()
        {
            mutex.WaitOne();
            var sem = queue.Dequeue();
            mutex.Release();
            sem.Release();
        }
    }
    static AuthorsFifo authorsFifo = new();
    public static void AuthorsFifoMeth()
    {
        Thread[] threads = new Thread[4];
        for (int i = 0; i < threads.Length; i++)
        {
            int ownI = i;
            Semaphore ownSemaphore = new(ownI == 0 ? 1 : 0, 5);
            threads[ownI] = new Thread(() =>
            {
                Console.WriteLine($"thread {ownI} started\n"); //

                authorsFifo.WaitOne(ownSemaphore);
                authorsFifo.Release();

                PrintContinuously($"{ownI}", 300); //

            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }
}
