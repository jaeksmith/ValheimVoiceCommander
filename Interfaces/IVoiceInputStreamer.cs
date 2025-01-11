namespace ValheimVoicecommander.Interfaces
{
    public interface IVoiceInputStreamer
    {
        void SetNext(ISpeechToTextStreamer next);
        void Run();
    }
}