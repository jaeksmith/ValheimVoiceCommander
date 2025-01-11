using System.Diagnostics;
using ValheimVoicecommander.Interfaces;

namespace ValheimVoicecommander.Implementations
{
    public class ScriptWindowsCommandOperationProcessor : ICommandOperationProcessor
    {
        private ITargetWindowInteract? targetWindowInteract;

        public void SetNext(ITargetWindowInteract targetWindowInteract)
        {
            this.targetWindowInteract = targetWindowInteract;
        }

        public void ProcessCommand(string commandToken)
        {
            Console.WriteLine($"Processing command: {commandToken}");

            if (commandToken == "OpenFullMap")
            {
                Process.Start("Valheim_ShowMap.exe");
            }
            else if (commandToken == "ActivateForsaken")
            {
                Process.Start("Valheim_ActivateForsaken.exe");
            }
        }
    }
}