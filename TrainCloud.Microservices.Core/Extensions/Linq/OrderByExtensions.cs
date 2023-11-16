using System.Linq.Expressions;
using System.Reflection;

namespace TrainCloud.Microservices.Core.Extensions.Linq;

public static class OrderByExtensions
{
    public static IOrderedQueryable<TQueryable> OrderBy<TQueryable>(this IQueryable<TQueryable> source, string property)
        => ApplyOrder<TQueryable>(source, property, "OrderBy");

    public static IOrderedQueryable<TQueryable> OrderByDescending<TQueryable>(this IQueryable<TQueryable> source, string property)
        => ApplyOrder<TQueryable>(source, property, "OrderByDescending");

    public static IOrderedQueryable<TQueryable> ThenBy<TQueryable>(this IOrderedQueryable<TQueryable> source, string property)
        => ApplyOrder<TQueryable>(source, property, "ThenBy");

    public static IOrderedQueryable<TQueryable> ThenByDescending<TQueryable>(this IOrderedQueryable<TQueryable> source, string property)
        => ApplyOrder<TQueryable>(source, property, "ThenByDescending");

    static IOrderedQueryable<TQueryable> ApplyOrder<TQueryable>(IQueryable<TQueryable> source, string property, string methodName)
    {
        string[] props = property.Split('.');
        Type type = typeof(TQueryable);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;
        foreach (string prop in props)
        {
            var nfo = type.FullName;
            PropertyInfo? pi = type.GetProperty(prop);
            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TQueryable), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

        object? result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TQueryable), type)
                .Invoke(null, new object[] { source, lambda });

        return (IOrderedQueryable<TQueryable>)result;
    }
}