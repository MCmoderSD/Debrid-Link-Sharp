namespace DebridLinkSharp.Models.Account;

public enum TwoFactorType
{
    // No value, or one that this version of the library does not know yet.
    Unknown,

    Email,

    Otp
}