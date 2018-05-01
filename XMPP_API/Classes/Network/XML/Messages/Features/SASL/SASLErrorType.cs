namespace XMPP_API.Classes.Network.XML.Messages.Features.SASL
{
    // https://xmpp.org/rfcs/rfc6120.html#sasl-errors
    public enum SASLErrorType
    {
        UNKNOWN_ERROR,
        ABORTED,
        INCORRECT_ENCODING,
        INVALID_AUTHZID,
        INVALID_MECHANISM,
        MECHANISM_TOO_WAEK,
        NOT_AUTHORIZED,
        TEMPORARY_AUTH_FAILURE,
        ACCOUNT_DISABLED,
        CREDENTIALS_EXPIRED,
        ENCRYPTION_REQUIRED,
        MALFORMED_REQUEST,
    }
}
