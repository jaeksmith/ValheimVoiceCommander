using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ValheimVoicecommander.Interfaces;

namespace ValheimVoicecommander.Implementations
{
    public class BasicWindowsCommandOperationProcessor : ICommandOperationProcessor
    {
        private ITargetWindowInteract? targetWindowInteract;
        private IntPtr targetWindowHandle = IntPtr.Zero;
        private const string TargetWindowTitle = "Valheim"; // [KEEP] "Valheim" "*TestTarget - Notepad"

        public void SetNext(ITargetWindowInteract targetWindowInteract)
        {
            this.targetWindowInteract = targetWindowInteract;
        }

        public void ProcessCommand(string commandToken)
        {
            Console.WriteLine($"Processing command: {commandToken}");

            if (commandToken == "OpenFullMap")
            {
                SendKeyToTargetWindow('M');
            }
            else if (commandToken == "ActivateForsaken")
            {
                SendKeyToTargetWindow('F');
            }
        }

        public void SendKeyToTargetWindow(char key)
        {
            if (targetWindowHandle == IntPtr.Zero || !NativeMethods.IsWindow(targetWindowHandle))
            {
                Console.WriteLine("Finding target window...");
                targetWindowHandle = NativeMethods.FindWindow(IntPtr.Zero, TargetWindowTitle);
                if (targetWindowHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Target window not found. Listing all windows:");
                    ListAllWindows();
                    return;
                }
                else
                {
                    Console.WriteLine("Target window found.");
                }
            }

            // Restore and bring the window to the foreground
            Console.WriteLine("Restoring and bringing the window to the foreground...");
            bool showWindowResult = NativeMethods.ShowWindow(targetWindowHandle, NativeMethods.SW_RESTORE);
            Console.WriteLine($"ShowWindow result: {showWindowResult}");

            IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
            Console.WriteLine($"Foreground window handle: {foregroundWindow}");
            IntPtr foregroundThread = NativeMethods.GetWindowThreadProcessId(foregroundWindow, out _);
            Console.WriteLine($"Foreground thread ID: {foregroundThread}");
            IntPtr currentThread = NativeMethods.GetCurrentThreadId();
            Console.WriteLine($"Current thread ID: {currentThread}");

            if (foregroundThread != currentThread)
            {
                Console.WriteLine("Attaching thread input...");
                bool attachInputResult = NativeMethods.AttachThreadInput(currentThread, foregroundThread, true);
                Console.WriteLine($"AttachThreadInput (attach) result: {attachInputResult}");
                if (!attachInputResult)
                {
                    Console.WriteLine($"AttachThreadInput (attach) failed with error: {Marshal.GetLastWin32Error()}");
                }
                bool setForegroundResult = NativeMethods.SetForegroundWindow(targetWindowHandle);
                Console.WriteLine($"SetForegroundWindow result: {setForegroundResult}");
                if (!setForegroundResult)
                {
                    Console.WriteLine($"SetForegroundWindow failed with error: {Marshal.GetLastWin32Error()}");
                }
                attachInputResult = NativeMethods.AttachThreadInput(currentThread, foregroundThread, false);
                Console.WriteLine($"AttachThreadInput (detach) result: {attachInputResult}");
                if (!attachInputResult)
                {
                    Console.WriteLine($"AttachThreadInput (detach) failed with error: {Marshal.GetLastWin32Error()}");
                }
            }
            else
            {
                bool setForegroundResult = NativeMethods.SetForegroundWindow(targetWindowHandle);
                Console.WriteLine($"SetForegroundWindow result: {setForegroundResult}");
                if (!setForegroundResult)
                {
                    Console.WriteLine($"SetForegroundWindow failed with error: {Marshal.GetLastWin32Error()}");
                }
            }

            // Attempt to bring the window to the top
            Console.WriteLine("Bringing window to top...");
            bool bringWindowToTopResult = NativeMethods.BringWindowToTop(targetWindowHandle);
            Console.WriteLine($"BringWindowToTop result: {bringWindowToTopResult}");

            IntPtr setFocusResult = NativeMethods.SetFocus(targetWindowHandle);
            Console.WriteLine($"SetFocus result: {setFocusResult}");
            if (setFocusResult == IntPtr.Zero)
            {
                Console.WriteLine($"SetFocus failed with error: {Marshal.GetLastWin32Error()}");
            }

            // Add a small delay to ensure the window is brought to the foreground and focused
            Thread.Sleep(100);

            // Get the window rectangle to calculate the center
            NativeMethods.RECT windowRect;
            NativeMethods.GetWindowRect(targetWindowHandle, out windowRect);
            int centerX = (windowRect.Left + windowRect.Right) / 2;
            int centerY = (windowRect.Top + windowRect.Bottom) / 2;

            // Simulate a right-click at the center of the window
            Console.WriteLine("Simulating right-click at the center of the window... (" + centerX + ", " + centerY + ")");
            NativeMethods.INPUT[] mouseInputs = new NativeMethods.INPUT[]
            {
                new NativeMethods.INPUT
                {
                    type = NativeMethods.INPUT_MOUSE,
                    u = new NativeMethods.InputUnion
                    {
                        mi = new NativeMethods.MOUSEINPUT
                        {
                            dx = centerX,
                            dy = centerY,
                            mouseData = 0,
                            dwFlags = NativeMethods.MOUSEEVENTF_ABSOLUTE | NativeMethods.MOUSEEVENTF_MOVE,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                },
                new NativeMethods.INPUT
                {
                    type = NativeMethods.INPUT_MOUSE,
                    u = new NativeMethods.InputUnion
                    {
                        mi = new NativeMethods.MOUSEINPUT
                        {
                            dx = centerX,
                            dy = centerY,
                            mouseData = 0,
                            dwFlags = NativeMethods.MOUSEEVENTF_RIGHTDOWN,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                },
                new NativeMethods.INPUT
                {
                    type = NativeMethods.INPUT_MOUSE,
                    u = new NativeMethods.InputUnion
                    {
                        mi = new NativeMethods.MOUSEINPUT
                        {
                            dx = centerX,
                            dy = centerY,
                            mouseData = 0,
                            dwFlags = NativeMethods.MOUSEEVENTF_RIGHTUP,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                }
            };

            Console.WriteLine("Calling SendInput for right-click...");
            uint mouseResult = NativeMethods.SendInput((uint)mouseInputs.Length, mouseInputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
            if (mouseResult == 0)
            {
                Console.WriteLine($"SendInput (right-click) failed with error: {Marshal.GetLastWin32Error()}");
            }
            else
            {
                Console.WriteLine($"SendInput (right-click) result: {mouseResult}");
            }

            // Add a small delay to ensure the right-click is processed
            Thread.Sleep(100);

            // Send the key press using SendInput
            Console.WriteLine("Sending key press...");
            NativeMethods.INPUT[] inputs = new NativeMethods.INPUT[]
            {
                new NativeMethods.INPUT
                {
                    type = NativeMethods.INPUT_KEYBOARD,
                    u = new NativeMethods.InputUnion
                    {
                        ki = new NativeMethods.KEYBDINPUT
                        {
                            wVk = (ushort)key,
                            dwFlags = NativeMethods.KEYEVENTF_KEYDOWN // Use the defined constant for key-down event
                        }
                    }
                }
            };

            Console.WriteLine("Calling SendInput for key down...");
            uint result = NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
            if (result == 0)
            {
                Console.WriteLine($"SendInput (key down) failed with error: {Marshal.GetLastWin32Error()}");
            }
            else
            {
                Console.WriteLine($"SendInput (key down) result: {result}");
            }

            // Add a small delay between key down and key up
            Thread.Sleep(100);

            inputs = new NativeMethods.INPUT[]
            {
                new NativeMethods.INPUT
                {
                    type = NativeMethods.INPUT_KEYBOARD,
                    u = new NativeMethods.InputUnion
                    {
                        ki = new NativeMethods.KEYBDINPUT
                        {
                            wVk = (ushort)key,
                            dwFlags = NativeMethods.KEYEVENTF_KEYUP
                        }
                    }
                }
            };

            Console.WriteLine("Calling SendInput for key up...");
            result = NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
            if (result == 0)
            {
                Console.WriteLine($"SendInput (key up) failed with error: {Marshal.GetLastWin32Error()}");
            }
            else
            {
                Console.WriteLine($"SendInput (key up) result: {result}");
            }

            // Thread.Sleep(100);

            // NativeMethods.PostMessage(targetWindowHandle, NativeMethods.WM_CHAR, (IntPtr)key, IntPtr.Zero);
        }

        private void ListAllWindows()
        {
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                StringBuilder windowText = new StringBuilder(256);
                NativeMethods.GetWindowText(hWnd, windowText, windowText.Capacity);
                string title = windowText.ToString();
                if (!string.IsNullOrEmpty(title))
                {
                    Console.WriteLine($"Window Handle: {hWnd}, Title: {title}");
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);
        }
    }
}