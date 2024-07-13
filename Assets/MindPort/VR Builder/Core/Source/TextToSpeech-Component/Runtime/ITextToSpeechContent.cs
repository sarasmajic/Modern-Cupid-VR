namespace VRBuilder.TextToSpeech.Audio
{
    /// <summary>
    /// Interface implemented by content generated by a TTS provider.
    /// </summary>
    public interface ITextToSpeechContent
    {
        /// <summary>
        /// True if a generated file for this content exists in the project.
        /// </summary>
        bool IsCached { get; }

        /// <summary>
        /// Text content to be fed to the TTS provider.
        /// </summary>
        string Text { get; }
    }
}