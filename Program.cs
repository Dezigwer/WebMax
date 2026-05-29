using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WebMax
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
        
        private const int SW_RESTORE = 9;
        private const int WM_USER_OPEN_FILE = 0x0400 + 100;

        [STAThread]
        static void Main(string[] args)
        {
            // Уникальное имя мьютекса (поменяй на своё уникальное)
            const string mutexName = "WebMax_SingleInstance_Mutex_{B4E2D8A1-3F5C-4E8D-9A2B-1C7F8E6D4B3A}";
            bool createdNew;
            
            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                if (createdNew)
                {
                    // Первый экземпляр — запускаем приложение
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    
                    var form = new Form1();
                    
                    // Если файл передан через аргументы при первом запуске
                    if (args.Length > 0 && File.Exists(args[0]))
                    {
                        form.SetFileToOpen(args[0]);
                    }
                    
                    Application.Run(form);
                }
                else
                {
                    // Второй экземпляр — находим и активируем существующее окно
                    IntPtr hWnd = FindWindow(null, "WebMax");
                    if (hWnd != IntPtr.Zero)
                    {
                        // Разворачиваем окно если оно свернуто
                        ShowWindow(hWnd, SW_RESTORE);
                        SetForegroundWindow(hWnd);
                        
                        // Передаём путь к файлу
                        if (args.Length > 0 && File.Exists(args[0]))
                        {
                            SendMessage(hWnd, WM_USER_OPEN_FILE, IntPtr.Zero, args[0]);
                        }
                    }
                }
            }
        }
    }
}
