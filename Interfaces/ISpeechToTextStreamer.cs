namespace ValheimVoicecommander.Interfaces
{
    public interface ISpeechToTextStreamer
    {
        void SetNext(ITextPhraseRecognizer next);

        void ProcessNextAudioChunk(byte[] audioData, int audioDataLen);
    }
}