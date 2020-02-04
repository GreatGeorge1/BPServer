namespace BPServer.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class Route : System.Attribute
    {
        public byte route;

        public Route(byte route)
        {
            this.route = route;
        }
    }
}
