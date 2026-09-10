using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Feature.Abstractions;

namespace RA.Utilities.Feature.Sample;

public record LocalGreetingRequest(string Name) : IRequest<string>;

public class LocalGreetingHandler : IRequestHandler<LocalGreetingRequest, string>
{
    public Task<string> HandleAsync(LocalGreetingRequest request, CancellationToken cancellationToken) =>
        Task.FromResult($"Hello, {request.Name}!");
}
