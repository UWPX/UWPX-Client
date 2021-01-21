namespace Omemo.Classes.Messages
{
    public interface IOmemoMessage
    {
        /// <summary>
        /// Converts the current Message to a byte array representation and returns it.
        /// </summary>
        byte[] ToByteArray();

        /// <summary>
        /// Validates all fields of the message and throws an <see cref="Omemo.Classes.Exceptions.OmemoException"/> in case something is not valid.
        /// </summary>
        void Validate();
    }
}
