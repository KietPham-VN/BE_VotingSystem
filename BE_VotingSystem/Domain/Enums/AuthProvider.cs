namespace BE_VotingSystem.Domain.Enums;

/// <summary>
/// Enumeration of supported authentication providers
/// </summary>
public enum AuthProvider
{
    /// <summary>
    /// Local authentication using email and password
    /// </summary>
    Local,
    
    /// <summary>
    /// Google OAuth authentication
    /// </summary>
    Google
}