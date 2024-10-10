using System;
using WebAppShared.Types;

namespace WebAppShared.Exceptions;

/// <summary>
/// Exception for SetOnce
/// <seealso cref="SetOnce{T}"/>
/// </summary>
public class AlreadySetException() : Exception("This variable is already set, cannot set again.");
