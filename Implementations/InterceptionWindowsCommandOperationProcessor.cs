using System;
using System.Runtime.InteropServices;
using InputInterceptorNS;
using ValheimVoicecommander.Interfaces;

namespace ValheimVoicecommander.Implementations
{
    public class InterceptionWindowsCommandOperationProcessor : ICommandOperationProcessor, ISetupComponent, IShutdownComponent
    {
        private ITargetWindowInteract? targetWindowInteract;
        private const string TargetWindowTitle = "Valheim"; // Change this to the actual window title
        private IntPtr targetWindowHandle = IntPtr.Zero;

        public void SetNext(ITargetWindowInteract targetWindowInteract)
        {
            this.targetWindowInteract = targetWindowInteract;
        }

        public bool Setup()
        {
            // Check if the driver is installed
            if (!InputInterceptor.CheckDriverInstalled())
            {
                Console.WriteLine("Interception driver is not installed.");

                // Check for administrative rights
                if (InputInterceptor.CheckAdministratorRights())
                {
                    Console.WriteLine("Installing the driver...");

                    // Attempt to install the driver
                    if (InputInterceptor.InstallDriver())
                    {
                        Console.WriteLine("Driver installed successfully. Please restart your computer.");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Driver installation failed.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Please run the application with administrative rights to install the driver.");
                    return false;
                }
            }

            // Initialize the InputInterceptor library
            if (InputInterceptor.Initialize())
            {
                Console.WriteLine("InputInterceptor initialized successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to initialize InputInterceptor.");
                return false;
            }
        }

        public void Shutdown()
        {
            // Dispose of InputInterceptor
            if (!InputInterceptor.Dispose())
            {
                Console.WriteLine("Failed to dispose InputInterceptor.");
            }
        }

        public void ProcessCommand(string commandToken)
        {
            Console.WriteLine($"Processing command: {commandToken}");

            if (commandToken == "OpenFullMap")
            {
                SendKeyToTargetWindow('M');
            }
        }

        private void SendKeyToTargetWindow(char key)
        {
            // IntPtr hWnd = Test.BetterFindAndBringWindowToFront();
            // if (hWnd == IntPtr.Zero)
            // {
            //     Console.WriteLine("Valheim window not found.");
            //     return;
            // }

            if (targetWindowHandle == IntPtr.Zero || !NativeMethods.IsWindow(targetWindowHandle))
            {
                Console.WriteLine("Finding target window...");
                targetWindowHandle = NativeMethods.FindWindow(IntPtr.Zero, TargetWindowTitle);
                if (targetWindowHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Target window not found. Listing all windows:");
                    return;
                }
                else
                {
                    Console.WriteLine("Target window found.");
                }
            }
            IntPtr hWnd = targetWindowHandle;

            // // Find the Valheim window by its title
            // IntPtr hWnd = NativeMethods.FindWindow(IntPtr.Zero, "Valheim");
            // if (hWnd == IntPtr.Zero)
            // {
            //     Console.WriteLine("Valheim window not found.");
            //     return;
            // }

            // Bring the Valheim window to the foreground
//            if (!NativeMethods.SetForegroundWindow(hWnd))
//            if (!FocusAndWaitForWindow(hWnd))
            // {
            //     Console.WriteLine("Failed to bring Valheim window to the foreground.");
            //     return;
            // }
            NativeMethods.SetWindowPos(hWnd, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);
            NativeMethods.SetWindowPos(hWnd, NativeMethods.HWND_NOTOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);
            NativeMethods.SetForegroundWindow(hWnd);

            // Allow some time for the window to come to the foreground
//            Thread.Sleep(100);

            // Get the window's rectangle to determine its center
            if (!NativeMethods.GetWindowRect(hWnd, out NativeMethods.RECT rect))
            {
                Console.WriteLine("Failed to get Valheim window dimensions.");
                return;
            }

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;
            int centerX = rect.Left + windowWidth / 2;
            int centerY = rect.Top + windowHeight / 2;

            // Create a MouseHook instance
            using (var mouseHook = new MouseHook())
            {
                // Move the cursor to the center of the window
                if (!mouseHook.SimulateMoveTo(centerX, centerY))
                {
                    Console.WriteLine("Failed to move cursor to the center of the window.");
                    return;
                }

                // Allow some time for the cursor movement
//                Thread.Sleep(200);

                // Simulate a right-click
                if (!mouseHook.SimulateRightButtonClick())
                {
                    Console.WriteLine("Failed to simulate right-click.");
                    return;
                }
            }

            // Allow some time after the right-click
//            Thread.Sleep(200);

            // Create a KeyboardHook instance
            using (var keyboardHook = new KeyboardHook())
            {
                // Simulate pressing the 'M' key
                if (!keyboardHook.SimulateKeyPress(KeyCode.M))
                {
                    Console.WriteLine("Failed to simulate '" + key + "' key press.");
                    return;
                }
            }
        }

        // // Focus the window and ensure it is the active foreground window
        // private static bool FocusAndWaitForWindow(IntPtr hWnd, int maxRetries = 10, int delayMs = 100)
        // {
        //     for (int attempt = 0; attempt < maxRetries; attempt++)
        //     {
        //         // Restore and bring the window to the foreground
        //         ShowWindow(hWnd, SW_RESTORE);
        //         SetForegroundWindow(hWnd);

        //         Thread.Sleep(delayMs);

        //         // Check if the window is the foreground window
        //         if (hWnd == GetForegroundWindow())
        //         {
        //             Console.WriteLine($"Valheim window is now the active foreground window (attempt {attempt + 1}).");
        //             return true;
        //         }

        //         Console.WriteLine($"Attempt {attempt + 1}: Valheim window is not the foreground window. Retrying...");
        //     }

        //     return false;
        // }

        // // Import necessary APIs
        // [DllImport("user32.dll")]
        // public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // [DllImport("user32.dll")]
        // public static extern bool SetForegroundWindow(IntPtr hWnd);

        // [DllImport("user32.dll")]
        // public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // [DllImport("user32.dll")]
        // public static extern IntPtr GetForegroundWindow();

        // // Constants for ShowWindow
        // private const int SW_RESTORE = 9;
    }
}