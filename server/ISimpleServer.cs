// Copyright (c) 2020 kirakira
// dart-step-by-step is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND,
// EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,
// MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using MQTTnet;
using MQTTnet.Client.Publishing;
using System.Threading.Tasks;

namespace server
{
    public interface ISimpleServer
    {
        int Port { get; }

        Task StartAsync(int port);

        Task StopAsync();

        Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage message);

        Task PublishAsync(params MqttApplicationMessage[] messages);
    }
}
