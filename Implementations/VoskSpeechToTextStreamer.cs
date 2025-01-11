using System;
using System.IO;
using System.Text.Json;
using Vosk;
using NAudio.Wave;
using ValheimVoicecommander.Interfaces;

namespace ValheimVoicecommander.Implementations
{
    public class VoskSpeechToTextStreamer : ISpeechToTextStreamer
    {
        private ITextPhraseRecognizer? textPhraseRecognizer;
        private readonly VoskRecognizer recognizer;
        private readonly WaveFormat waveFormat;

        public VoskSpeechToTextStreamer()
        {
            // Initialize Vosk model and recognizer
            var modelPath = "models/vosk-model-small-en-us-0.15";
            var model = new Model(modelPath);
            waveFormat = new WaveFormat(16000, 1); // 16kHz, mono
            recognizer = new VoskRecognizer(model, waveFormat.SampleRate);
        }

        public void SetNext(ITextPhraseRecognizer textPhraseRecognizer)
        {
            this.textPhraseRecognizer = textPhraseRecognizer;
        }

        public void ProcessNextAudioChunk(byte[] audioData, int audioDataLen)
        {
            using (var ms = new MemoryStream(audioData, 0, audioDataLen))
            using (var waveStream = new RawSourceWaveStream(ms, waveFormat))
            {
                int bytesRead;
                var buffer = new byte[4096];
                while ((bytesRead = waveStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (recognizer.AcceptWaveform(buffer, bytesRead))
                    {
                        var result = recognizer.Result();
                        ProcessResult(result);
                    }
                }
            }
        }

        private void ProcessResult(string result)
        {
            var jsonResult = JsonDocument.Parse(result);
            var finalResult = jsonResult.RootElement.GetProperty("text").GetString();

            if (!string.IsNullOrEmpty(finalResult))
            {
                var words = finalResult.Split(' ');
                foreach (var word in words)
                {
                    textPhraseRecognizer?.ProcessNextWord(word);
                }
            }
        }
    }
}