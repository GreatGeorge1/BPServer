namespace BPServer.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class RouteAttribute : System.Attribute
    {
        public byte Route;

        public RouteAttribute(byte route)
        {
            Route = route;
        }
    }
}
