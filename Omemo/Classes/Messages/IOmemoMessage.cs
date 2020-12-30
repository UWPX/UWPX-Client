namespace Omemo.Classes.Messages
{
    public interface IOmemoMessage
    {
        /// <summary>
        /// Converts the current Message to a byte array representation and returns it.
        /// </summary>
        byte[] ToByteArray();
    }
}
