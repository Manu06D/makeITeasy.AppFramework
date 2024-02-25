using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using makeITeasy.AppFramework.Infrastructure.EF7.Persistence;
using makeITeasy.CarCatalog.dotnet7.Infrastructure.Data;
using makeITeasy.CarCatalog.dotnet7.Models.Custom;
using makeITeasy.CarCatalog.dotnet7.Models;

using Microsoft.EntityFrameworkCore;
using makeITeasy.CarCatalog.dotnet7.Core.Ports;
using System.Linq.Expressions;
using System;
using Microsoft.EntityFrameworkCore.Query;
using DelegateDecompiler;
using Newtonsoft.Json.Schema;

namespace makeITeasy.CarCatalog.dotnet7.Infrastructure.Repositories
{
    public class SetPropertyHelper<T, TProperty>
    {
        public T SetProperty(T instance, Expression<Func<T, TProperty>> propertySelector, Expression<Func<TProperty, TProperty>> valueSelector)
        {
            var parameter = propertySelector.Parameters[0];
            var propertyAccess = propertySelector.Body;
            var value = valueSelector.Compile().Invoke(propertyAccess.Evaluate<TProperty>(instance));
            var body = Expression.Assign(propertyAccess, Expression.Constant(value, typeof(TProperty)));
            var lambda = Expression.Lambda<Func<T, T>>(body, parameter);
            return lambda.Compile()(instance);
        }
    }

    public static class ExpressionExtensions
    {
        public static T Evaluate<T>(this Expression expression, object instance)
        {
            var lambda = Expression.Lambda<Func<T>>(Expression.Convert(expression, typeof(T)));
            return lambda.Compile().Invoke();
        }
    }

    public class CarRepository : BaseEfRepository<Car, CarCatalogContext>, ICarRepository
    {
        public CarRepository(IDbContextFactory<CarCatalogContext> dbFactory, IMapper mapper) : base(dbFactory, mapper)
        {
        }

        public async Task<List<BrandGroupByCarCount>> GroupByBrandAndCountAsync()
        {
            var query = GetDbContext().Cars.GroupBy(x => x.Brand.Name).Select(x => new BrandGroupByCarCount() { BrandName = x.Key, CarCount = x.Count() });

            return await query.ToListAsync();
        }

        public async Task UpdateRangeXXX()
        {
            Expression<Func<SetPropertyCalls<Car>, SetPropertyCalls<Car>>> setPropertyCalls =
                x => x.SetProperty(e => e.Name, e => e.Name + "X");//.SetProperty(e => e.IsModernCar, e => true);

            var parameter = Expression.Parameter(typeof(Car), "x");

            var lambda = Expression.Lambda<Func<Car, Car>>(
            Expression.Block(
                Expression.Call(
                    Expression.Call(
                        Expression.Constant(new SetPropertyHelper<Car, string>()),
                        typeof(SetPropertyHelper<Car, string>).GetMethod("SetProperty"),
                        parameter,
                        Expression.Lambda<Func<Car, string>>(Expression.Property(parameter, "Name"), parameter),
                        Expression.Lambda<Func<string, string>>(Expression.Add(Expression.Property(parameter, "Name"), Expression.Constant("X")), parameter)
                    ),
                    typeof(SetPropertyHelper<Car, string>).GetMethod("SetProperty"),
                    parameter,
                    Expression.Lambda<Func<Car, bool>>(Expression.Property(parameter, "IsModernCar"), parameter),
                    Expression.Lambda<Func<bool, bool>>(Expression.Constant(true), parameter)
                ),
                parameter
            ),
            parameter
        );

            // Utilisation de l'expression
            var car = new Car { Name = "Ford"};
            Car modifiedCar = lambda.Compile()(car);


            ParameterExpression param1 = Expression.Parameter(typeof(SetPropertyCalls<Car>));
            MethodCallExpression expr = Expression.Call(
                typeof(SetPropertyCalls<Car>).GetMethod("SetProperty", new Type[] { typeof(SetPropertyCalls<Car>)}), param1);



            await GetDbContext().Cars.ExecuteUpdateAsync(setPropertyCalls);
        }

        //public Expression<Func<SetPropertyCalls<Car>, SetPropertyCalls<Car>>> Foo<Car, TProperty>()
        //{
        //    ParameterExpression param1 = Expression.Parameter(typeof(Func<Car, TProperty>));
        //    ParameterExpression param2 = Expression.Parameter(typeof(Func<Car, TProperty>));

        //    MethodCallExpression expr = Expression.Call(
        //        typeof(SetPropertyCalls<Car>).GetMethod("SetProperty", new Type[] { typeof(Func<Car, Func<TProperty, TProperty>>), typeof(SetPropertyCalls<Car>) }), param1, param2);

        //    Func<Func<Car, TProperty>, Func<Car, TProperty>, SetPropertyCalls<Car>> exprMethod = 
        //        Expression.Lambda<Func<Func<Car, TProperty>, Func<Car, TProperty>, SetPropertyCalls<Car>>>(expr, new ParameterExpression[] { param1, param2 }).Compile();

        //    var x=  Expression.Lambda<Func<Func<Car, TProperty>, Func<Car, TProperty>, SetPropertyCalls<Car>>>(Expression.Call(exprMethod.Method));

        //    return x;
        //}
    }
}
