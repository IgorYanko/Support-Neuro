using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

public class TimerService
{
    private readonly Func<Task> _action;
    private readonly TimeSpan _interval;
    private Timer _timer;
    private bool _isRunning;
    private bool _isActive;
    private readonly Dispatcher _dispatcher;

    public event EventHandler<Exception> OnError;

    public TimerService(Func<Task> action, TimeSpan interval, Dispatcher dispatcher)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _interval = interval;
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public void Start()
    {
        if (_isActive) return;
        _isActive = true;

        _timer = new Timer(async _ => await ExecuteAsync(), null, TimeSpan.Zero, _interval);
    }

    public void Stop()
    {
        _isActive = false;
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _timer?.Dispose();
        _timer = null;
    }

    private async Task ExecuteAsync()
    {
        if (_isRunning) return;

        try
        {
            _isRunning = true;

            await _dispatcher.InvokeAsync(async () => await _action.Invoke(), DispatcherPriority.Background);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, ex);
        }
        finally
        {
            _isRunning = false;
        }
    }
}