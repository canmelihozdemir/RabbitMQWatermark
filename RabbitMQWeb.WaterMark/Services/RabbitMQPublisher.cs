using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQWeb.WaterMark.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMqClientService _rabbitMqClientService;

        public RabbitMQPublisher(RabbitMqClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _rabbitMqClientService.Connect();
            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);
            var bodyByte=Encoding.UTF8.GetBytes(bodyString);
            var properties=channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange:RabbitMqClientService.ExchangeName,routingKey:RabbitMqClientService.RoutingWaterMark,basicProperties:properties,body:bodyByte);
            
            
            channel.BasicPublish(exchange:RabbitMqClientService.ExchangeName, routingKey: "route-resize-image", basicProperties:properties,body:bodyByte);
        }
    }
}
