using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semaphores;
public partial class Program
{
    public static void Queue2()
    {
        Thread[] leaders = new Thread[2];
        for (int i = 0; i < leaders.Length; i++)
        {
            int ownI = i;
            leaders[ownI] = new Thread(() =>
            {
                LeadersBefore(ownI);
                PrintContinuously($"L{ownI}", 300);
                LeadersAfter(ownI);
            });
        }
        Thread[] followers = new Thread[2];
        for (int i = 0; i < followers.Length; i++)
        {
            int ownI = i;
            followers[ownI] = new Thread(() =>
            {
                FollowersBefore(ownI);
                PrintContinuously($"F{ownI}", 300);
                FollowersAfter(ownI);
            });
        }
        foreach (var thread in leaders.Concat(followers))
        {
            thread.Start();
        }
    }

    static Semaphore LeaderQueue = new(0, 2);
    static Semaphore Rendezvous = new(0, 2);
    static Semaphore FollowerQueue = new(0, 2);
    static int Followers = 0;
    static int Leaders = 0;

    public static void LeadersBefore(int threadNumber)
    {
        mutex.WaitOne();
        if(Followers > 0)
        {
            Followers--;
            FollowerQueue.Release();
        }
        else
        {
            Leaders++;
            mutex.Release();
            LeaderQueue.WaitOne();
        }

    }

    public static void LeadersAfter(int threadNumber)
    {
        Rendezvous.WaitOne();

    }

    public static void FollowersBefore(int threadNumber)
    {
        mutex.WaitOne();
        if (Leaders > 0)
        {
            Leaders--;
            LeaderQueue.Release();
        }
        else
        {
            Followers++;
            mutex.Release();
            FollowerQueue.WaitOne();
        }

    }

    public static void FollowersAfter(int threadNumber)
    {
        Rendezvous.Release();
        mutex.Release();

    }
}
