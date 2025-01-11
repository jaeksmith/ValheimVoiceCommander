namespace ValheimVoicecommander.Interfaces
{
    public interface ITextPhraseRecognizer
    {
        void SetNext(ICommandOperationProcessor next);
        void ProcessNextWord(string word);
    }
}