using System;
using System.Threading.Tasks;
using System.Threading;

public class TimerService
{
    private readonly Func<Task> _action;
    private readonly TimeSpan _interval;
    private Timer _timer;
    private bool _isRunning;

    public event EventHandler<Exception> OnError;

    public TimerService(Func<Task> action, TimeSpan interval)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _interval = interval;
    }

    public void Start()
    {
        if (_timer != null) return;

        _timer = new Timer(async _ => await ExecuteAsync(), null, TimeSpan.Zero, _interval);
    }

    public void Stop()
    {
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
            await _action.Invoke();
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