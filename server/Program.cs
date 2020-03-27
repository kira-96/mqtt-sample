// Copyright (c) 2020 kirakira
// dart-step-by-step is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND,
// EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,
// MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            ISimpleServer simpleServer = SimpleServer.Instance;

            int port = 1883;
            simpleServer.StartAsync(port);

            Console.WriteLine("MQTT server is listening on: {0}", port);
            Console.ReadKey(true);


            simpleServer.StopAsync();
        }
    }
}
