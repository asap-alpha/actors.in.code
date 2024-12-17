namespace ActorsInCode.Infrastructure.Actors;

public class PersistToRedisActor:BaseActor
{
    public PersistToRedisActor()
    {
        ReceiveAsync<string>(m => HandlePersistToRedisActor(m));
    }

    public async Task HandlePersistToRedisActor(string message)
    {
       await Task.WhenAll();
    }
}