using Microsoft.Azure.Cosmos;

namespace event_sourcing_demo.presistence.azure
{
    public interface IDbContainerProvider
    {
        Container GetContainer(string containerName);
    }
}