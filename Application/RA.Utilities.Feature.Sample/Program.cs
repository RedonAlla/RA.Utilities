using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RA.Utilities.Feature;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Extensions;
using RA.Utilities.Feature.Sample;
using RA.Utilities.Feature.Sample.Handlers;

// Handlers auto-register from this assembly's own module initializer and from every assembly whose
// module initializer has already run. The CLR loads referenced assemblies lazily, so touch the
// handler assembly BEFORE AddMediator runs to guarantee its registrations are queued.
HandlersAssembly.Touch();

var services = new ServiceCollection();
services.AddSingleton<ILogger<Mediator>>(NullLogger<Mediator>.Instance);
services.AddMediator();
ServiceProvider provider = services.BuildServiceProvider();
IMediator mediator = provider.GetRequiredService<IMediator>();

// Handler from the separate class library assembly.
Pong pong = await mediator.Send<Ping, Pong>(new Ping("hello"));
Console.WriteLine($"Send: {pong.Value}");

// Notification fan-out from the separate class library assembly.
await mediator.Publish(new OrderPlacedNotification("order-1"));

// Handler defined in this entry assembly.
string greeting = await mediator.Send<LocalGreetingRequest, string>(new LocalGreetingRequest("world"));
Console.WriteLine($"Send: {greeting}");

#pragma warning disable CA1303 // Do not pass literals as localized parameters
Console.WriteLine("Done.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
