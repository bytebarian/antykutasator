namespace Antykutasator.Helpers
{
    public class ApplicationStateMessage
    {
        public ApplicationStateEvent Event { get; private set; }
        public object Context { get; private set; }

        public ApplicationStateMessage(ApplicationStateEvent appEvent, object context)
        {
            Event = appEvent;
            Context = context;
        }
    }
}
