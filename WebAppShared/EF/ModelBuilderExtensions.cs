using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using WebAppShared.Types;

namespace WebAppShared.EF;

public static class ModelBuilderExtensions
{
    public static void ApplyForeignKeyAttributes(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                var fkAttribute = property.GetCustomAttribute<ForeignKeyOfAttribute>();
                if (fkAttribute != null)
                {
                    modelBuilder.Entity(entityType.ClrType).HasOne(fkAttribute.RelatedType)
                        .WithMany()
                        .HasForeignKey(property.Name);
                }
            }
        }
    }

    /// <summary>
    /// Extracts all entities with ITenancyQueryFilter then applies the modelBuilder.QueryFilter and set TenantId as ForeignKey to the TenantId property.
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="tenantId"></param>
    public static void ApplyTenantIdQueryFilters(this ModelBuilder modelBuilder, int tenantId)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.GetInterfaces().Any(i => i == typeof(ITenancyQueryFilter)))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                    Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(Expression.Parameter(entityType.ClrType, "e"), "TenantId"),
                            Expression.Property(Expression.Parameter(typeof(int), "tenantId"), "Id")
                        ),
                        Expression.Parameter(entityType.ClrType, "e"),
                        Expression.Parameter(typeof(int), "tenantId")
                    )
                );
            }
        }
    }

    /// <summary>
    /// Extracts all entities with ITenancyQueryFilter then sets TenantId as ForeignKey to Tenant.Id
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="tenantClrType"></param>
    public static void ApplyTenantIdForeignKeys(this ModelBuilder modelBuilder, Type tenantClrType)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.GetInterfaces().Any(i => i == typeof(ITenancyQueryFilter)))
            {
                modelBuilder.Entity(entityType.ClrType).HasOne(tenantClrType)
                    .WithMany()
                    .HasForeignKey("TenantId");
            }
        }
    }

    #region Apply tenancy query filter automatically from the ITenancyQueryFilter interface
    /// <summary>
    /// Extracts all entities with ITenancyQueryFilter then applies the modelBuilder.QueryFilter
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="tenantId"></param>
    /*public static void ApplyTenancyFilter(this ModelBuilder modelBuilder, int tenantId)
    {
        var typesToFilter = Assembly.GetExecutingAssembly()  // or use another Assembly instance if needed
            .GetTypes()
            .Where(t => t.Namespace == "UnlockFeedApp.Models.DbEntities")  // replace with your specific namespace
            .Where(t => typeof(ITenancyQueryFilter).IsAssignableFrom(t) && !t.IsInterface);

        foreach (var type in typesToFilter)
        {
            var method = SetGlobalQueryMethod.MakeGenericMethod(type);
            method.Invoke(null, [modelBuilder, tenantId]);
        }
    }*/

    public static void ApplyTenancyFilter(this ModelBuilder modelBuilder, int? tenantId = null)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if the entity type has a TenantId property
            var tenantIdProperty = entityType.ClrType.GetProperty("TenantId");
            if (tenantIdProperty != null && tenantIdProperty.PropertyType == typeof(int)) // or string, etc., depending on your TenantId type
            {
                int tenantIdInt = tenantId ?? -1;
                // Create the filter expression dynamically
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyExpression = Expression.Property(parameter, tenantIdProperty);
                var tenantIdValue = Expression.Constant(tenantIdInt);
                var equalsExpression = Expression.Equal(propertyExpression, tenantIdValue);

                var lambda = Expression.Lambda(equalsExpression, parameter);

                // Apply the global filter
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    static readonly MethodInfo SetGlobalQueryMethod = typeof(ModelBuilderExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");

    [UsedImplicitly]
    public static void SetGlobalQuery<T>(ModelBuilder builder, int tenantId) where T : class, ITenancyQueryFilter
    {
        builder.Entity<T>().HasQueryFilter(e => e.TenantId == tenantId);
    }
    #endregion
}