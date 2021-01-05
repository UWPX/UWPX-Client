namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Session
{
    public enum OmemoSessionBuildError
    {
        UNKNOWN,
        TARGET_DOES_NOT_SUPPORT_OMEMO,
        REQUEST_DEVICE_LIST_TIMEOUT,
        REQUEST_BUNDLE_INFORMATION_TIMEOUT,
        REQUEST_DEVICE_LIST_IQ_ERROR,
        REQUEST_BUNDLE_INFORMATION_IQ_ERROR,
        KEY_ERROR
    }
}
