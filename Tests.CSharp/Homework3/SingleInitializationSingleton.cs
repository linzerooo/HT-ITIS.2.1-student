namespace Tests.CSharp.Homework3;

public class SingleInitializationSingleton
{
    public const int DefaultDelay = 3_000;
    private static readonly object Locker = new();

    private static volatile bool _isInitialized;

    private static Lazy<SingleInitializationSingleton> _value;

    private SingleInitializationSingleton(int delay = DefaultDelay)
    {
        Delay = delay;
        // imitation of complex initialization logic
        Thread.Sleep(delay);
    }

    public int Delay { get; }

    public static SingleInitializationSingleton Instance => _value.Value;

    internal static void Reset()
    {
        _value =
            new Lazy<SingleInitializationSingleton>(() => new SingleInitializationSingleton());
        _isInitialized = false;
    }

    public static void Initialize(int delay)
    {
        if (_isInitialized) throw new InvalidOperationException("already initialized");
        _value = new Lazy<SingleInitializationSingleton>(() => new SingleInitializationSingleton(delay));
        _isInitialized = true;
    }
}