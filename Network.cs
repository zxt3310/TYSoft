using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TYManager
{
    class NetworkClient
    {
        private static NetworkClient Singleton = null;
        private static Object Singleton_Lock = new Object();
        public static NetworkClient DefaultClient()
        {
            if(Singleton == null)
            {
                lock (Singleton_Lock)
                {
                    if (Singleton == null)
                    {
                        Singleton = new NetworkClient();
                    }
                }
            }
            return Singleton;       
        }
    }
}
