using System;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using ValheimVoicecommander.Implementations;
using ValheimVoicecommander.Interfaces;

namespace ValheimVoicecommander
{
    class Program
    {
        static void Main(string[] args)
        {
            // var enumerator = new MMDeviceEnumerator();
            // var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            // Console.WriteLine($"Default audio input device: {defaultDevice.FriendlyName}");
            // var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            // List available audio input devices
            Console.WriteLine("Available audio input devices:");
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                Console.WriteLine($"{i}: {deviceInfo.ProductName}");
            }

            // Prompt the user to select a device
            Console.Write("Select an audio input device by number: ");

            int deviceNumber = int.Parse(Console.ReadLine() ?? "0");

            // Create instances of the implementations using SetupComponent
            var voiceInputStreamer = SetupComponent<VoiceInputStreamer>();
            var voskSpeechToTextStreamer = SetupComponent<VoskSpeechToTextStreamer>();
            var textPhraseRecognizer = SetupComponent<TextPhraseRecognizer>();
            var commandOperationProcessor = SetupComponent<InterceptionWindowsCommandOperationProcessor>();//ScriptWindowsCommandOperationProcessor

            // Check if any component setup failed
            if (voiceInputStreamer == null || voskSpeechToTextStreamer == null || textPhraseRecognizer == null || commandOperationProcessor == null)
            {
                Console.WriteLine("Setup failed. Exiting...");
                return;
            }

            // Set the selected device number
            voiceInputStreamer.SetDeviceNumber(deviceNumber);

            // Connect the components
            voiceInputStreamer.SetNext(voskSpeechToTextStreamer);
            voskSpeechToTextStreamer.SetNext(textPhraseRecognizer);
            textPhraseRecognizer.SetNext(commandOperationProcessor);

            // Start the voice input streaming process
            var streamingThread = new Thread(voiceInputStreamer.Run);
            streamingThread.Start();

            Console.WriteLine("Voice command application started. Press Enter to stop...");
            Console.ReadLine();

            // Stop the voice input streaming process
            voiceInputStreamer.Stop();
            streamingThread.Join();

            // Shutdown components
            ShutdownComponent(voiceInputStreamer);
            ShutdownComponent(voskSpeechToTextStreamer);
            ShutdownComponent(textPhraseRecognizer);
            ShutdownComponent(commandOperationProcessor);
        }

        static T? SetupComponent<T>() where T : class, new()
        {
            var component = new T();
            if (component is ISetupComponent setupComponent)
            {
                if (!setupComponent.Setup())
                {
                    return null;
                }
            }
            return component;
        }

        static void ShutdownComponent<T>(T component) where T : class
        {
            if (component is IShutdownComponent shutdownComponent)
            {
                shutdownComponent.Shutdown();
            }
        }
    }
}
