using System;
using System.Threading;
using System.Windows.Forms;

namespace NB21_logger;

internal sealed class SimConnectMessageWindow : NativeWindow, IDisposable
{
    private readonly SimDataConn simDataConn;
    private readonly ManualResetEventSlim windowCreated = new(false);
    private readonly ManualResetEventSlim threadStopped = new(false);
    private readonly Thread thread;
    private ApplicationContext? context;
    private bool disposed;

    public SimConnectMessageWindow(SimDataConn simDataConn)
    {
        this.simDataConn = simDataConn ?? throw new ArgumentNullException(nameof(simDataConn));
        thread = new Thread(RunMessageLoop)
        {
            IsBackground = true,
            Name = "NB21Logger.SimConnectWindow",
            ApartmentState = ApartmentState.STA
        };
    }

    public new IntPtr Handle
    {
        get
        {
            if (!windowCreated.IsSet)
            {
                throw new InvalidOperationException("Message window has not been started.");
            }

            return base.Handle;
        }
    }

    public void Start()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(SimConnectMessageWindow));
        }

        if (windowCreated.IsSet)
        {
            return;
        }

        thread.Start();
        windowCreated.Wait();
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        if (windowCreated.IsSet)
        {
            context?.ExitThread();
            threadStopped.Wait();
            thread.Join();
        }

        if (base.Handle != IntPtr.Zero)
        {
            DestroyHandle();
        }
        windowCreated.Dispose();
        threadStopped.Dispose();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == SimDataConn.WM_USER_SIMCONNECT)
        {
            simDataConn.ReceiveSimConnectMessage();
            return;
        }

        base.WndProc(ref m);
    }

    private void RunMessageLoop()
    {
        context = new ApplicationContext();
        CreateHandle(new CreateParams());
        windowCreated.Set();
        Application.Run(context);
        threadStopped.Set();
    }
}
