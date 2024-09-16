using AutoMapper;
using System.Linq.Expressions;

namespace ProLinked.Application;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TDestination, TMember> Ignore<TDestination, TMember, TResult>(
        this IMappingExpression<TDestination, TMember> mappingExpression,
        Expression<Func<TMember, TResult>> destinationMember)
    {
        return mappingExpression.ForMember(destinationMember, opts => opts.Ignore());
    }
}