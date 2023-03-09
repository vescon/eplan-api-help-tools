// http://www.codeproject.com/Articles/18399/Localizing-System-MessageBox

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using PlaceholderConfigLoader.Properties;

#pragma warning disable 618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]

namespace PlaceholderConfigLoader
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "P/Invoke methods")]
    public static class MessageBoxLocalizer
    {
        //// ReSharper disable InconsistentNaming
        //// ReSharper disable MemberCanBePrivate.Global
        
        private const int CALLWNDPROCRET = 12;
        private const int INITDIALOG = 0x0110;

        private const int MBOK = 1;
        private const int MBCancel = 2;
        private const int MBAbort = 3;
        private const int MBRetry = 4;
        private const int MBIgnore = 5;
        private const int MBYes = 6;
        private const int MBNo = 7;

        private static readonly HookProcDelegate HookProc;
        private static readonly EnumChildProcDelegate EnumProc;

        [ThreadStatic]
        private static IntPtr _hook;

        [ThreadStatic]
        private static int _buttonsCounter;

        static MessageBoxLocalizer()
        {
            HookProc = MessageBoxHookProc;
            EnumProc = MessageBoxEnumProc;
            _hook = IntPtr.Zero;

            OK = "&OK";
            Cancel = "&Cancel";
            Abort = "&Abort";
            Retry = "&Retry";
            Ignore = "&Ignore";
            Yes = "&Yes";
            No = "&No";
        }

        private delegate IntPtr HookProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate bool EnumChildProcDelegate(IntPtr hWnd, IntPtr lParam);

        public static string OK { get; set; }

        public static string Cancel { get; set; }

        public static string Abort { get; set; }

        public static string Retry { get; set; }

        public static string Ignore { get; set; }

        public static string Yes { get; set; }

        public static string No { get; set; }

        /// <summary>
        /// Enables MessageBoxManager functionality
        /// </summary>
        /// <remarks>
        /// MessageBoxManager functionality is enabled on current thread only.
        /// Each thread that needs MessageBoxManager functionality has to call this method.
        /// </remarks>
        public static void Register()
        {
            if (_hook != IntPtr.Zero)
                throw new NotSupportedException("One hook per thread allowed.");
            _hook = SetWindowsHookEx(CALLWNDPROCRET, HookProc, IntPtr.Zero, AppDomain.GetCurrentThreadId());
        }

        /// <summary>
        /// Disables MessageBoxManager functionality
        /// </summary>
        /// <remarks>
        /// Disables MessageBoxManager functionality on current thread only.
        /// </remarks>
        public static void Unregister()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hook);
                _hook = IntPtr.Zero;
            }
        }

        private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(_hook, nCode, wParam, lParam);

            var msg = (CWPRetStruct)Marshal.PtrToStructure(lParam, typeof(CWPRetStruct));
            var hook = _hook;

            if (msg.Message == INITDIALOG)
            {
                var className = new StringBuilder(10);
                GetClassName(msg.Hwnd, className, className.Capacity);
                if (className.ToString() == "#32770")
                {
                    _buttonsCounter = 0;
                    
                    EnumChildWindows(msg.Hwnd, EnumProc, IntPtr.Zero);
                    
                    if (_buttonsCounter == 1)
                    {
                        var singleCancelButton = GetDlgItem(msg.Hwnd, MBCancel);
                        if (singleCancelButton != IntPtr.Zero)
                            SetWindowText(singleCancelButton, OK);
                    }
                }
            }

            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        private static bool MessageBoxEnumProc(IntPtr hWnd, IntPtr lParam)
        {
            var className = new StringBuilder(10);
            GetClassName(hWnd, className, className.Capacity);
            if (className.ToString() == "Button")
            {
                var controlId = GetDlgCtrlID(hWnd);
                switch (controlId)
                {
                    case MBOK:
                        SetWindowText(hWnd, OK);
                        break;
                    case MBCancel:
                        SetWindowText(hWnd, Cancel);
                        break;
                    case MBAbort:
                        SetWindowText(hWnd, Abort);
                        break;
                    case MBRetry:
                        SetWindowText(hWnd, Retry);
                        break;
                    case MBIgnore:
                        SetWindowText(hWnd, Ignore);
                        break;
                    case MBYes:
                        SetWindowText(hWnd, Yes);
                        break;
                    case MBNo:
                        SetWindowText(hWnd, No);
                        break;
                }

                _buttonsCounter++;
            }

            return true;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProcDelegate lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProcDelegate lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetDlgCtrlID(IntPtr hwndCtl);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDlgItem(IntPtr hDlg, int nDlgItemId);

        [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [StructLayout(LayoutKind.Sequential)]
        private struct CWPRetStruct
        {
            public readonly IntPtr Result;
            public readonly IntPtr LParam;
            public readonly IntPtr WParam;
            public readonly uint Message;
            public readonly IntPtr Hwnd;
        }

        //// ReSharper restore InconsistentNaming
        //// ReSharper restore MemberCanBePrivate.Global
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "This helper class 'belongs' to the class above.")]
    public class MessageBoxL : IDisposable
    {
        private MessageBoxL()
        {
            MessageBoxLocalizer.Register();

            MessageBoxLocalizer.OK = Resources.OK;
            MessageBoxLocalizer.Cancel = Resources.Cancel;
            MessageBoxLocalizer.Abort = Resources.Abort;
            MessageBoxLocalizer.Retry = Resources.Retry;
            MessageBoxLocalizer.Ignore = Resources.Ignore;
            MessageBoxLocalizer.Yes = Resources.Yes;
            MessageBoxLocalizer.No = Resources.No;
        }

        public static MessageBoxResult Show(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            using (new MessageBoxL())
            {
                return MessageBox.Show(message, title, buttons, icon);
            }
        }

        public static MessageBoxResult Show(string message, string title, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            using (new MessageBoxL())
            {
                return MessageBox.Show(message, title, buttons, icon, defaultResult);
            }
        }

        public void Dispose()
        {
            MessageBoxLocalizer.Unregister();
        }
    }
}