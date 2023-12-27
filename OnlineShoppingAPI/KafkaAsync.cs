using Confluent.Kafka;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System.Text.Json;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShoppingAPI
{
    [Target("KafkaAsync")]
    public class KafkaAsync: AsyncTaskTarget
    {
        // Concurrent queue to hold a pool of Kafka producers

        private readonly ConcurrentQueue<IProducer<Null, string>> _producerPool;
        
        // Counter for the number of producers in the pool

        private int _pCount;
      
        // Maximum size of the producer pool
        private int _maxSize;
        
        
        // Constructor initializes the producer pool and sets the maximum size

        public KafkaAsync()
        {
            _producerPool = new ConcurrentQueue<IProducer<Null, string>>();
            _maxSize = 10;

        }

        // Layout for the Kafka topic

        [RequiredParameter]
        public Layout Topic { get; set; }

        // Bootstrap servers for Kafka

        [RequiredParameter]
        public string BootstrapServers { get; set; }

        // Method to close the target and dispose of producers in the pool
        protected override void CloseTarget()
        {
            base.CloseTarget();
            _maxSize = 0;
            while (_producerPool.TryDequeue(out var context))
            {
                context.Dispose();
            }
        }

        // Method to rent a Kafka producer from the pool

        private IProducer<Null, string> RentProducer()
        {
            if (_producerPool.TryDequeue(out var producer))
            {
                Interlocked.Decrement(ref _pCount);

                return producer;
            }

            var config = new ProducerConfig
            {
                BootstrapServers = BootstrapServers,
            };

            producer = new ProducerBuilder<Null, string>(config).Build();

            return producer;
        }

        // Method to return a Kafka producer to the pool

        private bool Return(IProducer<Null, string> producer)
        {
            if (Interlocked.Increment(ref _pCount) <= _maxSize)
            {
                _producerPool.Enqueue(producer);

                return true;
            }

            Interlocked.Decrement(ref _pCount);

            return false;
        }

        // Method to asynchronously write log events to Kafka

        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            // Extract topic and message from log event
            string topic = base.RenderLogEvent(this.Topic, logEvent);
            string msg = base.RenderLogEvent(this.Layout, logEvent);
            
            // Create JSON object with log event details

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                level = logEvent.Level.Name.ToUpper(),
                @class = logEvent.LoggerName,
                message = msg
            });
            // Rent a Kafka producer from the pool

            var producer = RentProducer();

            try
            {
                // Produce the log message to the specified Kafka topic

                await producer.ProduceAsync(topic, new Message<Null, string>()
                {
                    Value = json
                });
            }
            catch (Exception ex)
            {
                // Log any errors that occur during publishing

                InternalLogger.Error(ex, $"kafka published error.");
            }
            finally
            {
                // Return the Kafka producer to the pool or dispose of it if the pool is full

                var returned = Return(producer);
                if (!returned)
                {
                    producer.Dispose();
                }
            }
        }
    }
}