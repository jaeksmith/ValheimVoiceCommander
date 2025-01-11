using System.Collections.Generic;

namespace ValheimVoicecommander.Implementations
{
    public class Node
    {
        public string? CommandToken { get; set; }
        public Dictionary<string, Node> NextWords { get; } = new Dictionary<string, Node>();
    }
}