using System;
using System.Runtime.InteropServices;
using System.Threading;
using InputInterceptorNS;
using ValheimVoicecommander.Implementations;

class Test
{

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    // Define the RECT structure
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static void TestFunc2()
    {
        bool setup = CheckInstallInitializeInputInterceptor();

        if (!setup)
        {
            return;
        }

//        Thread.Sleep(1000);

        // Find the Valheim window by its title
//        IntPtr hWnd = FindWindow(null, "Valheim");
        IntPtr hWnd = BetterFindAndBringWindowToFront();
        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("Valheim window not found.");
            return;
        }

        // Bring the Valheim window to the foreground
        // if (!SetForegroundWindow(hWnd))
        // {
        //     Console.WriteLine("Failed to bring Valheim window to the foreground.");
        //     return;
        // }

        // Allow some time for the window to come to the foreground
        Thread.Sleep(500);

        // Get the window's rectangle to determine its center
        if (!GetWindowRect(hWnd, out RECT rect))
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
            Thread.Sleep(200);

            // Simulate a right-click
            if (!mouseHook.SimulateRightButtonClick())
            {
                Console.WriteLine("Failed to simulate right-click.");
                return;
            }
        }

        // Allow some time after the right-click
        Thread.Sleep(200);

        // Create a KeyboardHook instance
        using (var keyboardHook = new KeyboardHook())
        {
            // Simulate pressing the 'M' key
            if (!keyboardHook.SimulateKeyPress(KeyCode.M))
            {
                Console.WriteLine("Failed to simulate 'M' key press.");
                return;
            }
        }

        CleanupInputInterceptor();
    }

    public static bool CheckInstallInitializeInputInterceptor()
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

    public static void CleanupInputInterceptor()
    {
        // Dispose of InputInterceptor
        if (!InputInterceptor.Dispose())
        {
            Console.WriteLine("Failed to dispose InputInterceptor.");
        }
    }

    public static void TestUsingInputInterceptor()
    {
        // Proceed with creating hooks
        using (var keyboardHook = new KeyboardHook())
        {
            // Simulate pressing the 'M' key
            keyboardHook.SimulateKeyPress(KeyCode.M);
            Console.WriteLine("Simulated 'M' key press.");
        }
    }

    public static void TestFunc()
    {
        // Get the handle to the target window (replace "Valheim" with the actual window title)
        IntPtr hWnd = FindWindow(null, "Valheim");

        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine($"Window not found! Error code: {Marshal.GetLastWin32Error()}");
            return;
        }

        // Bring the window to the front and ensure it is the active window
        if (!FocusAndWaitForWindow(hWnd))
        {
            Console.WriteLine("Failed to bring Valheim window to the foreground.");
            return;
        }

        // Small delay to ensure the game processes focus
        Thread.Sleep(200);

        // Simulate pressing and releasing the 'm' key using keybd_event
        SimulateKeyPressLegacy(0x4D); // Virtual Key Code for 'M'
    }

    // Import necessary APIs
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    // Constants for ShowWindow
    private const int SW_RESTORE = 9;

    // Constants for keybd_event
    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public static IntPtr SimpleFindAndBringWindowToFront_()
    {
        // Find the Valheim window by its title
        IntPtr hWnd = NativeMethods.FindWindow(IntPtr.Zero, "Valheim");
        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("Valheim window not found.");
            return IntPtr.Zero;
        }

        // Bring the Valheim window to the foreground
        if (!NativeMethods.SetForegroundWindow(hWnd))
        {
            Console.WriteLine("Failed to bring Valheim window to the foreground.");
            return IntPtr.Zero;
        }

        return hWnd;
    }

    public static IntPtr BetterFindAndBringWindowToFront()
    {
        IntPtr hWnd = NativeMethods.FindWindow(IntPtr.Zero, "Valheim");
        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("Valheim window not found.");
            return IntPtr.Zero;
        }

// NativeMethods.ShowWindow(hWnd, NativeMethods.SW_MINIMIZE);
// Thread.Sleep(100);
// NativeMethods.ShowWindow(hWnd, NativeMethods.SW_RESTORE);
// NativeMethods.SetForegroundWindow(hWnd);

    NativeMethods.SetWindowPos(hWnd, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);
    NativeMethods.SetWindowPos(hWnd, NativeMethods.HWND_NOTOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);
    SetForegroundWindow(hWnd);

///////////////////////////////////

    // int currentProcessId = NativeMethods.GetCurrentProcessId();
    // if (!NativeMethods.AllowSetForegroundWindow(currentProcessId))
    // {
    //     Console.WriteLine("Failed to enable foreground window access for the current process.");
    // }
    // else
    // {
    //     Console.WriteLine("Foreground window access enabled for the current process.");
    // }

    // if (!NativeMethods.AllowSetForegroundWindow(-1))
    // {
    //     Console.WriteLine("Failed to enable foreground window access for the any process.");
    // }
    // else
    // {
    //     Console.WriteLine("Foreground window access enabled for the any process.");
    // }

    // NativeMethods.SetForegroundWindow(hWnd);

////////////////////////////////

        // IntPtr foregroundThread = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), out _);
        // IntPtr currentThread = NativeMethods.GetCurrentThreadId();

        // // Attach the threads
        // if (foregroundThread != currentThread)
        // {
        //     NativeMethods.AttachThreadInput(foregroundThread, currentThread, true);
        // }

        // // Bring the window to the foreground
        // NativeMethods.SetForegroundWindow(hWnd);

        // // Detach the threads
        // if (foregroundThread != currentThread)
        // {
        //     NativeMethods.AttachThreadInput(foregroundThread, currentThread, false);
        // }

        return hWnd;
    }

    // Focus the window and ensure it is the active foreground window
    private static bool FocusAndWaitForWindow(IntPtr hWnd, int maxRetries = 10, int delayMs = 100)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            // Restore and bring the window to the foreground
            ShowWindow(hWnd, SW_RESTORE);
            SetForegroundWindow(hWnd);

            Thread.Sleep(delayMs);

            // Check if the window is the foreground window
            if (hWnd == GetForegroundWindow())
            {
                Console.WriteLine($"Valheim window is now the active foreground window (attempt {attempt + 1}).");
                return true;
            }

            Console.WriteLine($"Attempt {attempt + 1}: Valheim window is not the foreground window. Retrying...");
        }

        return false;
    }

    // Simulate a key press using keybd_event
    private static void SimulateKeyPressLegacy(byte keyCode)
    {
        // Key down
        keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);

        // Hold the key for a longer delay to mimic real input
        Thread.Sleep(100);

        // Key up
        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

        Console.WriteLine($"Key press simulated for key code: {keyCode}");
    }

}
