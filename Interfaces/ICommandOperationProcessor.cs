namespace ValheimVoicecommander.Interfaces
{
    public interface ICommandOperationProcessor
    {
        void SetNext(ITargetWindowInteract next);
        void ProcessCommand(string commandToken);
    }
}