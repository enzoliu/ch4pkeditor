using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ch4pkeditorM
{
    public class Core
    {
        /*
         * load dll
         */
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out int lpNumberOfbytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out int lpNumberOfbytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            int dwSize,
            out int lpNumberOfbytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            Int32 nSize,
            out IntPtr lpNumberOfbytesWritten
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfbytesWritten
        );

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("USER32.DLL", EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, uint message, int wParam, uint lParam);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetForegroundWindow();

        /*
         * Singleton instance
         */
        public static Core shared = new Core();

        /*
         * Variables
         */
        private Process _process = new Process();
        private List<Process> _processList = new List<Process>();
        private IntPtr _handler = new IntPtr();
        private IntPtr _winProc = new IntPtr();
        private string _processName = "";
        private bool _initialized = false;
        private const Int32 _FULL_ACCESS = 0x1F0FFF;
        private List<String> _errorMessage = new List<string>();
        /*
         * Variable Access
         */
        public Process GetProcess()
        {
            return _process;
        }
        public IntPtr GetHandler()
        {
            return _handler;
        }
        public IntPtr GetWinProc()
        {
            return _winProc;
        }
        public bool IsInit()
        {
            return _initialized;
        }
        public bool IsActive(IntPtr handle)
        {
            IntPtr activeHandle = GetForegroundWindow();
            return (activeHandle == handle);
        }
        public string[] getErrorMessage()
        {
            string[] msgs = _errorMessage.ToArray();
            _errorMessage.Clear();
            return msgs;
        }
        private bool isProcessAlive()
        {
            if (_process.Id <= 0)
            {
                return false;
            }
            return Process.GetProcesses().Where(x => x.ProcessName.Equals(_processName)).Count() > 0;
        }
        /*
         * Methods
         */
        public List<Process> FindProcess(string find)
        {
            _processName = find;
            _processList = Process.GetProcesses().Where(x => string.Equals(x.ProcessName, find, StringComparison.OrdinalIgnoreCase)).ToList();
            return _processList;
        }
        public void SetProcess(Process p)
        {
            _process = p;
        }
        public bool Initialize()
        {
            _initialized = false;
            _errorMessage.Clear();
            if (_process.Id <= 0)
            {
                _errorMessage.Add("錯誤的Process。");
                return false;
            }
            /* open process */
            int hprocess = OpenProcess(_FULL_ACCESS, false, _process.Id);
            if (hprocess > 0)
            {
                _handler = (IntPtr)hprocess;
                _winProc = FindWindow(_process.ProcessName, _process.MainWindowTitle);
                _initialized = true;
            }
            else
            {
                _errorMessage.Add("開啟Process過程中發生錯誤。");
            }
            return _initialized;
        }

        public byte[] readMemory(IntPtr position, int buffSize)
        {
            byte[] buff = new byte[buffSize];
            if (!_initialized)
            {
                _errorMessage.Add("尚未讀取遊戲資料!");
                return buff;
            }
            if (!isProcessAlive())
            {
                _errorMessage.Add("遊戲已關閉或無法讀取。");
                return buff;
            }
            int bytesreaded;
            ReadProcessMemory(_handler, position, buff, buffSize, out bytesreaded);
            return buff;
        }

        public bool WriteMemory(IntPtr position, byte[] data)
        {
            if (!_initialized)
            {
                _errorMessage.Add("尚未讀取遊戲資料!");
                return false;
            }
            if (!isProcessAlive())
            {
                _errorMessage.Add("遊戲已關閉或無法讀取。");
                return false;
            }
            IntPtr refWritebytes = IntPtr.Zero;
            return WriteProcessMemory(_handler, position, data, data.Length, out refWritebytes);
        }
    }
}
