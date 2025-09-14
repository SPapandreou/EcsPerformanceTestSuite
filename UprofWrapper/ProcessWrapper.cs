using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EcsTestSuiteWrapper;

public class ProcessWrapper
{
    [DllImport("kernel32.dll")]
    static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

    delegate bool ConsoleCtrlDelegate(uint ctrlType);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    const uint CTRL_C_EVENT = 0;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    private readonly Process _process;

    private readonly ConsoleCtrlDelegate _handler;

    public ProcessWrapper(string binary, string args)
    {
        AllocConsole();   
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = binary,
                Arguments = args,
                UseShellExecute = false
            }
        };

        _handler = IgnoreCtrlC;

        SetConsoleCtrlHandler(_handler, true);
    }

    public void Start()
    {
        _process.Start();
    }

    public async Task Stop()
    {
        GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0);
        await _process.WaitForExitAsync();
        SetConsoleCtrlHandler(_handler, false);
    }

    private static bool IgnoreCtrlC(uint ctrlType)
    {
        if (ctrlType == CTRL_C_EVENT)
        {
            return true;
        }

        return false;
    }
}