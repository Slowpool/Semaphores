using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Semaphores;
public class Fifo()
{
    Queue<Semaphore> queue = [];
    Semaphore mutex = new(1, 2);
    public void Append(Semaphore semaphore, int ownI)
    {
        mutex.WaitOne();
        Console.WriteLine($"thread {ownI} enqueued\n"); //
        queue.Enqueue(semaphore);
        mutex.Release();
        semaphore.WaitOne();
    }
    // after append and before next thread locks himself
    public void Next()
    {
        mutex.WaitOne();
        try
        {
            var sem = queue.Dequeue();
            mutex.Release();
            sem.Release();
        }
        catch
        {
            mutex.Release();

        }
    }
}

public partial class Program
{
    private static Fifo fifo = new();
    public static void FifoMeth()
    {
        Thread[] threads = new Thread[4];
        for (int i = 0; i < threads.Length; i++)
        {
            Semaphore ownSemaphore = new(0, 5);

            int ownI = i;
            threads[ownI] = new Thread(() =>
            {

                fifo.Append(ownSemaphore, ownI);

                PrintContinuously($"{ownI}", 300); //

                fifo.Next();


            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
        //Thread.Sleep(500);
        fifo.Next();
    }
}
