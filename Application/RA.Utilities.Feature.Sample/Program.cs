using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RA.Utilities.Feature;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Extensions;
using RA.Utilities.Feature.Generated;
using RA.Utilities.Feature.Sample;
using RA.Utilities.Feature.Sample.Handlers;

// Handlers auto-register from this assembly's own module initializer and from every assembly whose
// module initializer has already run. The CLR loads referenced assemblies lazily, so touch the
// handler assembly BEFORE AddMediator runs to guarantee its registrations are queued.
// The mediator source generator registers the generated MediatorImpl the same way: AddMediator
// applies the queued registrations, so IMediator resolves to the generated implementation — the
// only mediator implementation in the package.
HandlersAssembly.Touch();

var services = new ServiceCollection();
services.AddSingleton<ILogger<MediatorImpl>>(NullLogger<MediatorImpl>.Instance);
services.AddMediator();
ServiceProvider provider = services.BuildServiceProvider();

// Inject the concrete generated mediator for monomorphized dispatch (each message has its own
// typed Send overload, so calls bind directly with no interface or generic dispatch). The
// IMediator interface remains available and dispatches through the same generated code.
MediatorImpl mediator = provider.GetRequiredService<MediatorImpl>();

// Handler from the separate class library assembly.
Pong pong = await mediator.Send(new Ping("hello"));
Console.WriteLine($"Send: {pong.Value}");

// Object-based dispatch by runtime type.
object pongObject = await mediator.Send((object)new Ping("object-ping"));
Console.WriteLine($"Send (object): {((Pong)pongObject!).Value}");

// Notification fan-out from the separate class library assembly.
await mediator.Publish(new OrderPlacedNotification("order-1"));

// Handler defined in this entry assembly.
string greeting = await mediator.Send(new LocalGreetingRequest("world"));
Console.WriteLine($"Send: {greeting}");

#pragma warning disable CA1303 // Do not pass literals as localized parameters
Console.WriteLine("Done.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
