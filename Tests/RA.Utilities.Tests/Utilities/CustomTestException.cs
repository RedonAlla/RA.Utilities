using System;

namespace RA.Utilities.Tests.Utilities;

public class CustomTestException : Exception
{
    public CustomTestException() : base("Custom test exception") { }
}
