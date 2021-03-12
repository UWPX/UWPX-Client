namespace Storage.Classes.Models.Chat
{
    public enum MessageState
    {
        SENDING = 0,
        SEND = 1,
        UNREAD = 2,
        READ = 3,
        DELIVERED = 4,
        TO_ENCRYPT = 5,
        ENCRYPT_FAILED = 6,
    }
}
