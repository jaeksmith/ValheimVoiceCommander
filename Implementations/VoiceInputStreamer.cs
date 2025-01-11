using System;
using System.Threading;
using NAudio.Wave;
using ValheimVoicecommander.Interfaces;

namespace ValheimVoicecommander.Implementations
{
    public class VoiceInputStreamer : IVoiceInputStreamer
    {
        private ISpeechToTextStreamer? speechToTextStreamer;
        private WaveInEvent? waveIn;
        private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);
        private int deviceNumber;

        public void SetNext(ISpeechToTextStreamer speechToTextStreamer)
        {
            this.speechToTextStreamer = speechToTextStreamer;
        }

        public void SetDeviceNumber(int deviceNumber)
        {
            this.deviceNumber = deviceNumber;
        }

        public void Run()
        {
            // Initialize the microphone input
            waveIn = new WaveInEvent
            {
                DeviceNumber = deviceNumber,
                WaveFormat = new WaveFormat(16000, 1) // 16kHz, mono
            };

            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();

            Console.WriteLine("VoiceInputStreamer is running...");

            // Keep the application running
            stopEvent.WaitOne();
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            // Feed audio data chunks to the speechToTextStreamer
            speechToTextStreamer?.ProcessNextAudioChunk(e.Buffer, e.BytesRecorded);
        }

        public void Stop()
        {
            waveIn?.StopRecording();
            stopEvent.Set();
        }
    }
}
