using System.Linq.Expressions;
using System.Reflection;

namespace TrainCloud.Microservices.Core.Extensions.Linq;

/// <summary>
/// Methods for dynamic ordering of an IQueryable by field name
/// </summary>
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
        Type typeOfQueryable = typeof(TQueryable);
        string[] props = property.Split('.');
        ParameterExpression paramExpr = Expression.Parameter(typeOfQueryable, "x");
        Expression expr = paramExpr;

        foreach (string prop in props)
        {
            PropertyInfo? piQueryable = typeOfQueryable.GetProperty(prop);
            if (piQueryable is not null)
            {
                expr = Expression.Property(expr, piQueryable);
                typeOfQueryable = piQueryable.PropertyType;
            }
        }

        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TQueryable), typeOfQueryable);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, paramExpr);

        object? resultObject = typeof(Queryable).GetMethods()
                                                .Single(method =>
                                                {
                                                    Type[] methodGenericArguments = method.GetGenericArguments();
                                                    ParameterInfo[] methodParameters = method.GetParameters();

                                                    if (method.Name == methodName &&
                                                       method.IsGenericMethodDefinition &&
                                                       methodGenericArguments.Length == 2 &&
                                                       methodParameters.Length == 2)
                                                    {
                                                        return true;
                                                    }

                                                    return false;
                                                })
                                                .MakeGenericMethod(typeof(TQueryable), typeOfQueryable)
                                                .Invoke(null, new object[] { source, lambda });

        IOrderedQueryable<TQueryable> result = (IOrderedQueryable<TQueryable>)resultObject!;

        return result;
    }
}