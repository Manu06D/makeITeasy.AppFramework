# makeITeasy.AppFramework 

## Build and Built on the shoulders of giants

A powerful ready-to-go .net core application framework based on most famous & useful nuget packages
* AutoMapper
* AutoFac
* mediatR

This framework rely on efcore and extension [Ef Power Tools](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools) is highly recommanded

This project, based on [Microsoft Eshop Web](https://github.com/dotnet-architecture/eShopOnWeb) architecture, will help you to build your application on a decent way. With few lines of code, you'll be able to have a ready to go project that match hexagonal architecture principle.

Have you ever spend tons of hours thinking of the right architecture without implementing a feature ... If so, this framework is made for you :)

### Features

#### Persistance & databases

Your service layer will support complete interraction with database.

##### Basic Queries

The framework supports queries with count and paging support capabilities.

```
  var getResult = await carService.QueryAsync(new BaseCarQuery() { ID = result.Entity.Id}, includeCount : true);
```

You can include connected data by using the include directive (string or c# code supported)

```
  var getResult = await carService.QueryAsync(new BaseCarQuery() { IncludeStrings = new List<string>() { "Brand.Country"} });
  var getResult = await carService.QueryAsync
                (new BaseCarQuery() { Includes=new List<Expression<Func<Car, object>>>(){ x => x.Brand.Country } }, includeCount: true);   
```

You can make queries with automatic mapping to another object. When using projection, the underlying sql request will be optimized to only get data that will be used for mapping.

```
   var getResult = await carService.QueryWithProjectionAsync<SmallCarInfo>(new BaseCarQuery() { });
```






