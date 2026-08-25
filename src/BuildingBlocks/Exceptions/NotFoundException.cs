namespace ITSM.BuildingBlocks.Exceptions;

/// <summary>
/// Thrown by an Application-layer handler when a requested aggregate
/// doesn't exist (or isn't visible under the caller's tenant scope, which
/// looks identical from the outside — RLS never distinguishes "doesn't
/// exist" from "not yours"). Host.Api's exception-handling middleware maps
/// this to a 404 ProblemDetails response.
/// </summary>
public sealed class NotFoundException(string message) : Exception(message);
