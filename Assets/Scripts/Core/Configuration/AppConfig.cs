namespace Core.Configuration
{
    public class AppConfig
    {
        public string UprofWrapperPath  { get; set; }
        public string UprofBinaryPath { get; set; }
        public string UprofTemp { get; set; }
        public string ResultDirectory { get; set; }
        public bool UprofEnable { get; set; }
        public string TestRunFileDirectory { get; set; }
    }
}