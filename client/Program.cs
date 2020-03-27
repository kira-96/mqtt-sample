// Copyright (c) 2020 kirakira
// mqtt-sample is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND,
// EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,
// MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 5)
                return;

            string server = args[0];
            int port = int.Parse(args[1]);
            string id = args[2];
            string topic = args[3];
            string message = args[4];

            IMqttClient client = new MqttFactory().CreateMqttClient();
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithCleanSession()
                .WithClientId(id)
                .WithCredentials("Guest", "")
                .WithKeepAlivePeriod(TimeSpan.FromMinutes(1))
                .WithTcpServer(server, port)
                .Build();


            CancellationTokenSource tokenSource = new CancellationTokenSource();

            var result = await client.ConnectAsync(options, tokenSource.Token);

            client
                .UseApplicationMessageReceivedHandler(e =>
                {
                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();
                })
                .UseConnectedHandler(e => { Console.WriteLine("Connect Successfully."); })
                .UseDisconnectedHandler(async e =>
                {
                    Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                    await Task.Delay(2000);

                    try
                    {
                        await client.ConnectAsync(options, CancellationToken.None);
                    }
                    catch
                    {
                        Console.WriteLine("### RECONNECTING FAILED ###");
                    }
                });

            await client.SubscribeAsync(topic);

            await client.PublishAsync(
                new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build(), CancellationToken.None);

            Console.ReadKey(true);
        }
    }
}
