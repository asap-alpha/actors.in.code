namespace ActorsInCode.Infrastructure.Actors;

public class PersistToPgActor : BaseActor
{
    public PersistToPgActor()
    {
        
        ReceiveAsync<string>(message => ReceivePersistToPgActor(message));
    }

    public async Task  ReceivePersistToPgActor(string message)
    {
        
    }
}