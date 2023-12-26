using Confluent.Kafka;

namespace OnlineShoppingApp_Consumer
{
    public static class ConsumerConfigurations
    {
        public static ConsumerConfig GetConfig()
        {
            return new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "KafkaExampleConsumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
            };
        }
    }
}
