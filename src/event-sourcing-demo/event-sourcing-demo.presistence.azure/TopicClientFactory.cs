using Microsoft.Azure.ServiceBus;
using System;

namespace event_sourcing_demo.presistence.azure
{
    public class TopicClientFactory : ITopicClientFactory
    {
        private readonly string _connectionString;

        public TopicClientFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ITopicClient Build(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topicName));
            return new TopicClient(_connectionString, topicName);
        }
    }
}