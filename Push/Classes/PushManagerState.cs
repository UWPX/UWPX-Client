namespace Push.Classes
{
    public enum PushManagerState
    {
        NOT_INITIALIZED,
        INITIALIZING,
        REQUESTING_CHANNEL,
        STORING_CHANNEL,
        SENDING_CHANNEL_URI_TO_PUSH_SERVER,
        DONE,
        ERROR,
    }
}
