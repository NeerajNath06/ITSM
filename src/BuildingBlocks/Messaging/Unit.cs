namespace ITSM.BuildingBlocks.Messaging;

/// <summary>
/// Return type for a command that has no meaningful result — <see cref="IRequestHandler{TRequest,TResponse}"/>
/// always needs a <c>TResponse</c>, and <see langword="void"/> can't fill
/// that slot.
/// </summary>
public readonly struct Unit
{
    public static readonly Unit Value = default;
}
