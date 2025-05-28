using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowNetServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetManager.Instance = new NetManager();
            NetManager.Instance.StartServer(8888);
        }
    }
}
