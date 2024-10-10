using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace WebLibServer.EF;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
[UsedImplicitly]
public class ForeignKeyOfAttribute : Attribute
{
    public Type RelatedType { get; private set; }
    public string ForeignKeyPropertyName { get; private set; }

    public ForeignKeyOfAttribute(Type relatedType, string propertyName)
    {
        if (relatedType.GetProperty(propertyName) == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found on type '{relatedType.FullName}'.");
        }

        RelatedType = relatedType;
        ForeignKeyPropertyName = propertyName;
    }

    public ForeignKeyOfAttribute(Type type, Expression<Func<object>> propertyExpression)
    {
        var memberExpression = propertyExpression.Body as MemberExpression;
        if (memberExpression == null)
        {
            throw new ArgumentException($"The provided expression is not a member access expression.", nameof(propertyExpression));
        }

        if (type.GetProperty(memberExpression.Member.Name) == null)
        {
            throw new ArgumentException($"Property '{memberExpression.Member.Name}' not found on type '{type.FullName}'.");
        }

        RelatedType = type;
        ForeignKeyPropertyName = memberExpression.Member.Name;
    }
}
