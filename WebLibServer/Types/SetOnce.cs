using System.Runtime.CompilerServices;
using WebLibServer.Exceptions;

namespace WebLibServer.Types;

/// <summary>
/// Allows the value of the object to be set only once.
/// Use with `readonly` fields to prevent the object itself to be set again.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SetOnce<T>
{
	private bool _set;
	private T _value;

	public T Value
	{
		get => _value;

		set
		{
			if (_set) throw new AlreadySetException();
			_set = true;
			_value = value;
		}
	}

	public static implicit operator T(SetOnce<T> toConvert)
	{
		return toConvert._value;
	}
}

