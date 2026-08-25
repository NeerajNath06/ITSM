namespace ITSM.BuildingBlocks.Exceptions;

/// <summary>
/// Thrown by an Application-layer handler when a command would violate a
/// uniqueness invariant (e.g. a tenant slug or username already taken).
/// Host.Api's exception-handling middleware maps this to a 409 ProblemDetails
/// response.
/// </summary>
public sealed class ConflictException(string message) : Exception(message);
