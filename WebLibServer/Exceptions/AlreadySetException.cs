using System;
using WebLibServer.Types;

namespace WebLibServer.Exceptions;

/// <summary>
/// Exception for SetOnce
/// <seealso cref="SetOnce{T}"/>
/// </summary>
public class AlreadySetException() : Exception("This variable is already set, cannot set again.");
