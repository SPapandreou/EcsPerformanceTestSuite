# UprofWrapper

A console application that wraps around a process and allows terminating it gracefully using the CTRL+C event. This
was implemented to allow gracefully terminating the AMDuProfCLI application, so that it can be started from within
the application supposed to be profiled. This allows profiling specific sections of code.

---

## Configuration
The path to the AMDuProfCLI.exe binary needs to be set in config.json.

## Usage

```cmd
UprofWrapper [OutputDirectory] [PID]
```
To stop the profiling and terminate the application, the string "stop" needs to be printed to the stdin.