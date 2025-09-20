using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using R3;
using UnityEngine;

namespace Core.uProf
{
    public sealed class NativeProcess : IDisposable
    {
        public Observable<string> StdOut => _stdOut;
        private readonly Subject<string> _stdOut = new();

        public Observable<string> StdErr => _stdErr;
        private readonly Subject<string> _stdErr = new();


        private StreamWriter _standardInputWriter;
        private StreamReader _standardOutputReader;
        private StreamReader _standardErrorReader;

        public IntPtr ProcessHandle { get; private set; } = IntPtr.Zero;
        public IntPtr ThreadHandle { get; private set; } = IntPtr.Zero;
        public int ProcessId { get; private set; } = 0;
        public int ThreadId { get; private set; } = 0;

        private SafeFileHandle _stdinWriteHandle = null!;
        private SafeFileHandle _stdoutReadHandle = null!;
        private SafeFileHandle _stderrReadHandle = null!;

        private SafeFileHandle _stdinReadHandle = null!;
        private SafeFileHandle _stdoutWriteHandle = null!;
        private SafeFileHandle _stderrWriteHandle = null!;

        private readonly string _exePath;
        private readonly string _arguments;
        private readonly string _workingDirectory;

        private bool _disposed;

        public NativeProcess(string exePath, string arguments, string workingDirectory)
        {
            _exePath = exePath;
            _arguments = arguments;
            _workingDirectory = workingDirectory;
            
            CreatePipes();
        }
        private void CreatePipes()
        {
            // SECURITY_ATTRIBUTES for pipe creation (allow inheritance)
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>();
            sa.bInheritHandle = true;
            sa.lpSecurityDescriptor = IntPtr.Zero;

            // stdin pipe: child reads from stdinRead, parent writes to stdinWrite
            if (!CreatePipe(out IntPtr stdinRead, out IntPtr stdinWrite, ref sa, 0))
                ThrowLastError("CreatePipe(stdin)");

            // stdout pipe: child writes to stdoutWrite, parent reads from stdoutRead
            if (!CreatePipe(out IntPtr stdoutRead, out IntPtr stdoutWrite, ref sa, 0))
                ThrowLastError("CreatePipe(stdout)");

            // stderr pipe: child writes to stderrWrite, parent reads from stderrRead
            if (!CreatePipe(out IntPtr stderrRead, out IntPtr stderrWrite, ref sa, 0))
                ThrowLastError("CreatePipe(stderr)");

            // Ensure the parent handles are NOT inherited by the child:
            // keep child-side handles inheritable, but parent-side not
            const int HANDLE_FLAG_INHERIT = 0x00000001;
            // Clear inheritance on the parent handles
            if (!SetHandleInformation(stdinWrite, HANDLE_FLAG_INHERIT, 0))
                ThrowLastError("SetHandleInformation(stdinWrite)");
            if (!SetHandleInformation(stdoutRead, HANDLE_FLAG_INHERIT, 0))
                ThrowLastError("SetHandleInformation(stdoutRead)");
            if (!SetHandleInformation(stderrRead, HANDLE_FLAG_INHERIT, 0))
                ThrowLastError("SetHandleInformation(stderrRead)");

            // Save handles as SafeFileHandle for stream wrapping
            _stdinReadHandle = new SafeFileHandle(stdinRead, ownsHandle: true);   // child side
            _stdinWriteHandle = new SafeFileHandle(stdinWrite, ownsHandle: true); // parent side

            _stdoutReadHandle = new SafeFileHandle(stdoutRead, ownsHandle: true);   // parent side
            _stdoutWriteHandle = new SafeFileHandle(stdoutWrite, ownsHandle: true); // child side

            _stderrReadHandle = new SafeFileHandle(stderrRead, ownsHandle: true);   // parent side
            _stderrWriteHandle = new SafeFileHandle(stderrWrite, ownsHandle: true); // child side
        }

        public void Start()
        {
            // Prepare command line
            var cmdLine = new StringBuilder();
            // CreateProcess requires mutable string buffer for lpCommandLine
            cmdLine.Append('\"').Append(_exePath).Append('\"');
            if (!string.IsNullOrEmpty(_arguments))
            {
                cmdLine.Append(' ').Append(_arguments);
            }

            // Set up STARTUPINFO with std handles (child will inherit them)
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf<STARTUPINFO>();
            si.dwFlags = STARTF_USESTDHANDLES;
            si.hStdInput = _stdinReadHandle.DangerousGetHandle();   // child reads from this
            si.hStdOutput = _stdoutWriteHandle.DangerousGetHandle(); // child writes to this
            si.hStdError = _stderrWriteHandle.DangerousGetHandle();  // child writes to this

            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            // SECURITY attributes for process/thread (we can pass NULL)
            bool success = CreateProcessW(
                lpApplicationName: _exePath,
                lpCommandLine: cmdLine.ToString(),
                lpProcessAttributes: IntPtr.Zero,
                lpThreadAttributes: IntPtr.Zero,
                bInheritHandles: true,    // IMPORTANT: allow child to inherit handles we marked inheritable
                dwCreationFlags: CREATE_NO_WINDOW,
                lpEnvironment: IntPtr.Zero,
                lpCurrentDirectory: string.IsNullOrEmpty(_workingDirectory) ? null : _workingDirectory,
                lpStartupInfo: ref si,
                lpProcessInformation: out pi
            );

            if (!success)
            {
                ThrowLastError("CreateProcessW");
            }

            // Store process/thread handles and IDs
            ProcessHandle = pi.hProcess;
            ThreadHandle = pi.hThread;
            ProcessId = pi.dwProcessId;
            ThreadId = pi.dwThreadId;
            
            // Wrap handles in streams
            var stdoutStream = new FileStream(_stdoutReadHandle, FileAccess.Read);
            _standardOutputReader = new StreamReader(stdoutStream, Encoding.UTF8);

            var stderrStream = new FileStream(_stderrReadHandle, FileAccess.Read);
            _standardErrorReader = new StreamReader(stderrStream, Encoding.UTF8);

            var stdinStream = new FileStream(_stdinWriteHandle, FileAccess.Write);
            _standardInputWriter = new StreamWriter(stdinStream, Encoding.UTF8) { AutoFlush = true };
            
            
            UniTask.RunOnThreadPool<UniTask>(async () =>
            {
                try
                {
                    while (await _standardOutputReader.ReadLineAsync() is { } line)
                    {
                        _stdOut.OnNext(line);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Error: " + e.Message);
                }
            });
            
            UniTask.RunOnThreadPool<UniTask>(async () =>
            {
                try
                {
                    while (await _standardErrorReader.ReadLineAsync() is { } line)
                    {
                        _stdErr.OnNext(line);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Error: " + e.Message);
                }
            });
        }

        public async UniTask WriteLineAsync(string line)
        {
            await _standardInputWriter.WriteLineAsync(line);
            await _standardInputWriter.FlushAsync();
        }

        public void WaitForExit()
        {
            if (ProcessHandle == IntPtr.Zero)
                return;

            const uint INFINITE = 0xFFFFFFFF;
            uint res = WaitForSingleObject(ProcessHandle, INFINITE);
            if (res == WAIT_FAILED)
                ThrowLastError("WaitForSingleObject");
        }

        public void Kill()
        {
            if (ProcessHandle != IntPtr.Zero)
            {
                TerminateProcess(ProcessHandle, 1);
                WaitForExit();
            }
        }

        private static void ThrowLastError(string prefix)
        {
            var err = Marshal.GetLastWin32Error();
            Debug.Log(err);
            throw new System.ComponentModel.Win32Exception(err, prefix + " failed");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try { _standardInputWriter?.Dispose(); } catch { }
            try { _standardOutputReader?.Dispose(); } catch { }
            try { _standardErrorReader?.Dispose(); } catch { }

            if (_stdinReadHandle != null && !_stdinReadHandle.IsInvalid) { _stdinReadHandle.Dispose(); }
            if (_stdinWriteHandle != null && !_stdinWriteHandle.IsInvalid) { _stdinWriteHandle.Dispose(); }
            if (_stdoutReadHandle != null && !_stdoutReadHandle.IsInvalid) { _stdoutReadHandle.Dispose(); }
            if (_stdoutWriteHandle != null && !_stdoutWriteHandle.IsInvalid) { _stdoutWriteHandle.Dispose(); }
            if (_stderrReadHandle != null && !_stderrReadHandle.IsInvalid) { _stderrReadHandle.Dispose(); }
            if (_stderrWriteHandle != null && !_stderrWriteHandle.IsInvalid) { _stderrWriteHandle.Dispose(); }

            if (ProcessHandle != IntPtr.Zero) { CloseHandle(ProcessHandle); ProcessHandle = IntPtr.Zero; }
            if (ThreadHandle != IntPtr.Zero) { CloseHandle(ThreadHandle); ThreadHandle = IntPtr.Zero; }
            
            _stdOut.OnCompleted();
            _stdErr.OnCompleted();

            _stdOut.Dispose();
            _stdErr.Dispose();
        }

        #region Win32 interop

        private const int STARTF_USESTDHANDLES = 0x00000100;
        private const uint CREATE_NO_WINDOW = 0x08000000;
        private const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
        private const uint WAIT_FAILED = 0xFFFFFFFF;

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe,
            ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetHandleInformation(IntPtr hObject, int dwMask, int dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateProcessW(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        #endregion
    }
}
