public class CounterService(ICounter counter)
{
    public ICounter Counter { get; } = counter;
}