namespace Overwatch_Health_Analyser
{
    public class PythonInstance
    {
        public static void ShowColor(int r, int g, int b)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = @"C:\Users\Jannify\AppData\Local\Programs\Python\Python37\python.exe";
            startInfo.Arguments = "Pimoroni_LED.py " + r + " " + g + " " + b;
            process.StartInfo = startInfo;
            process.Start();
        }

    }
}
