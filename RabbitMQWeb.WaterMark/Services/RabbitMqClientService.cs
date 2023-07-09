using RabbitMQ.Client;

namespace RabbitMQWeb.WaterMark.Services
{
    public class RabbitMqClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWaterMark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";
        private readonly ILogger<RabbitMqClientService> _logger;

        public RabbitMqClientService(ConnectionFactory connectionFactory,ILogger<RabbitMqClientService> logger)
        {
            _connectionFactory= connectionFactory;
            _logger= logger;
        }

        public IModel Connect()
        {
            _connection=_connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true }) return _channel;

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName,type:"direct",true,false);
            _channel.QueueDeclare(QueueName,true,false,false,null);
            _channel.QueueBind(exchange:ExchangeName,queue:QueueName,routingKey:RoutingWaterMark);


            _channel.QueueDeclare("queue-resize-image", true, false, false, null);
            _channel.QueueBind(exchange: ExchangeName, queue: "queue-resize-image", routingKey: "route-resize-image");

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");
        }
    }
}
