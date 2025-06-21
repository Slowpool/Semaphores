using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semaphores;
public partial class Program
{
    public static void Queue3()
    {
        Thread thread1 = new Thread(() =>
        {
            FollowerQueue.Release();
            LeaderQueue.WaitOne();
        });
    }
}
