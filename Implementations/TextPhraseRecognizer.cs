using System;
using System.Collections.Generic;
using ValheimVoicecommander.Interfaces;

namespace ValheimVoicecommander.Implementations
{
    public class TextPhraseRecognizer : ITextPhraseRecognizer
    {
        private ICommandOperationProcessor? commandOperationProcessor;
        private readonly Node rootNode = new Node();
        private readonly List<Node> currentNodes = new List<Node>();

        public TextPhraseRecognizer()
        {
            InitializePhrases();
        }

        public void SetNext(ICommandOperationProcessor commandOperationProcessor)
        {
            this.commandOperationProcessor = commandOperationProcessor;
        }

        public void ProcessNextWord(string word)
        {
            word = word.ToLower();

            // Print out the received word in brackets
            Console.WriteLine($"[{word}]");

            // Process the word through the current nodes
            var newNodes = new List<Node>();
            foreach (var node in currentNodes)
            {
                if (node.NextWords.TryGetValue(word, out var nextNode))
                {
                    if (nextNode.CommandToken != null)
                    {
                        commandOperationProcessor?.ProcessCommand(nextNode.CommandToken);
                    }
                    if (nextNode.NextWords.Count > 0)
                    {
                        newNodes.Add(nextNode);
                    }
                }
            }

            // Add the root node if it has a match for the word
            if (rootNode.NextWords.TryGetValue(word, out var rootNodeNext))
            {
                if (rootNodeNext.CommandToken != null)
                {
                    commandOperationProcessor?.ProcessCommand(rootNodeNext.CommandToken);
                }
                if (rootNodeNext.NextWords.Count > 0)
                {
                    newNodes.Add(rootNodeNext);
                }
            }

            currentNodes.Clear();
            currentNodes.AddRange(newNodes);
        }

        private void InitializePhrases()
        {
            // Define the phrases and their associated command tokens
            var phrases = new Dictionary<string[], string>
            {
                { new[] { "steve", "show", "map" }, "OpenFullMap" },
                { new[] { "steve", "activate", "forsaken" }, "ActivateForsaken" },
            };

            // Convert the phrases to the node structure
            foreach (var phrase in phrases)
            {
                var currentNode = rootNode;
                foreach (var word in phrase.Key)
                {
                    var lowerWord = word.ToLower();
                    if (!currentNode.NextWords.TryGetValue(lowerWord, out var nextNode))
                    {
                        nextNode = new Node();
                        currentNode.NextWords[lowerWord] = nextNode;
                    }
                    currentNode = nextNode;
                }
                currentNode.CommandToken = phrase.Value;
            }
        }
    }
}