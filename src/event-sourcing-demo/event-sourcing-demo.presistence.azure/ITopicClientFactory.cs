using Microsoft.Azure.ServiceBus;

namespace event_sourcing_demo.presistence.azure
{
    public interface ITopicClientFactory
    {
        ITopicClient Build(string topicName);
    }
}