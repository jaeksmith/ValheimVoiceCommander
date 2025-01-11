# VoiceCommandApp

## Project Description

This project is a Windows 11 C# application that listens to the microphone input stream, converts it to text, watches for specific phrases, and performs operations when recognized phrases occur. The main components and their interactions are as follows:

VoiceInputStreamer -> SpeechToTextStreamer -> TextPhraseRecognizer -> PhraseOperationProcessor -> TargetWindowInteract

- **VoiceInputStreamer**: Listens to the microphone and streams voice audio input data to the SpeechToTextStreamer.
- **SpeechToTextStreamer**: Converts voice/speech audio data to text and streams that to the TextPhraseRecognizer.
- **TextPhraseRecognizer**: Watches the incoming stream of words for phrases, converting them to command tokens and streaming these to the PhraseOperationProcessor.
- **PhraseOperationProcessor**: Receives command tokens and calls TargetWindowInteract.
- **TargetWindowInteract**: Interacts with the target window to perform the required actions.

## Project Structure

- **Interfaces**: Contains the interface definitions for the primary components.
  - `IVoiceInputStreamer.cs`
  - `ISpeechToTextStreamer.cs`
  - `ITextPhraseRecognizer.cs`
  - `IPhraseOperationProcessor.cs`
  - `ITargetWindowInteract.cs`

## Future Work

- Implement the methods for each interface.
- Implement the main application logic.
- Add functionality to offload some processing to another server.
