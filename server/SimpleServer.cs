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
using MQTTnet.Client.Publishing;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class SimpleServer : ISimpleServer
    {
        private static readonly object _locker = new object();
        private static ISimpleServer _instance = null;
        public static ISimpleServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new SimpleServer();
                        }
                    }
                }

                return _instance;
            }
        }

        private readonly IMqttServer _mqttServer;

        public int Port { get; private set; }

        private Dictionary<string, string> users;

        private SimpleServer()
        {
            _mqttServer = new MqttFactory().CreateMqttServer();
            users = new Dictionary<string, string>()
            {
                { "Administrator", "^P@$$W0&D$" },
                { "Guest",         ""           },
            };
        }

        public async Task StartAsync(int port)
        {
            Port = port;
            MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(port)
                .WithConnectionValidator((cv) =>
                {
                    if (cv.ClientId.Length < 10)
                    {
                        cv.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                        return;
                    }

                    if (!users.ContainsKey(cv.Username) ||
                        users[cv.Username] != cv.Password )
                    {
                        cv.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        return;
                    }

                    cv.ReasonCode = MqttConnectReasonCode.Success;
                })
                .WithApplicationMessageInterceptor(cv => { cv.AcceptPublish = true; })
                .WithSubscriptionInterceptor(cv => { cv.AcceptSubscription = true; });
            await _mqttServer.StartAsync(optionsBuilder.Build());
            _mqttServer.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            }).UseClientConnectedHandler(e => { Console.WriteLine($"[{e.ClientId}] Connected."); })
            .UseClientDisconnectedHandler(e => { Console.WriteLine($"[{e.ClientId}] Disconnected."); });
        }

        public async Task StopAsync()
        {
            await _mqttServer.StopAsync();
        }

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage message)
        {
            return _mqttServer.PublishAsync(message);
        }

        public async Task PublishAsync(params MqttApplicationMessage[] messages)
        {
            await _mqttServer.PublishAsync(messages);
        }
    }
}
