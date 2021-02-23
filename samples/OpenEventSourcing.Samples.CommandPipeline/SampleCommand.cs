using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Samples.CommandPipeline
{
    public class SampleCommand : Command
    {
        public SampleCommand(string subject) 
            : base(subject, OpenEventSourcing.CorrelationId.New(), 1, Actor.From("OpenEventSourcing.Samples.CommandPipeline"))
        {
        }
    }
}
