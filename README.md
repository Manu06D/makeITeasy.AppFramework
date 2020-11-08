# makeITeasy.AppFramework 

## Build and Built on the shoulders of giants

A powerful ready-to-go .net core application framework based on most famous & useful nuget packages
* AutoMapper
* AutoFac
* mediatR

This framework rely on efcore and extension [Ef Power Tools](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools) is highly recommanded

This project, based on [Microsoft Eshop Web](https://github.com/dotnet-architecture/eShopOnWeb) architecture, will help you to build your application on a decent way. With few lines of code, you'll be able to have a ready to go project that match some hexagonal architecture principles.

Have you ever spend tons of hours thinking of the right architecture without implementing a feature ... If so, this framework is made for you :)

### Project structure

This framework has been designed with the following flow : Service => Repository => Database

                 +--------------------+        +-----------|--------+        +--------------------+                    
                 |                    |        |           |        |        |        |           |                    
                 |      Service       |------->|   IRepo   |  Repo  |------->| EFCore | Database  |                    
                 |                    |        |           |        |        |        |           |                    
                 +--------------------+        +-----------|--------+        +--------------------+                    
                                                                                                                       
                                   +---------------+                                                                   
                                   |    Models     |                                                                   
                                   +---------------+                     

To get a fast but extensible implementation, framework provides base Service and Repository classes that you need to inherit.

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

### Getting Started

#### Creating Models

Models (or Entities) are transfered through all layers and should match your database schema. With EF Core, it doesn't matter if you use code first or db first, in the end, framework only expect that class inherit from _IBaseEntity_ . _IBaseEntity_ will require you to implement the getter property DatabaseID which should point to your primary key property.

```
 public object DatabaseID { get => Id; }
```

I personnaly use [Ef Power Tools](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools) for this task taking benefits of his template rendering.

#### Creating Services

Services are classes that bring operations related an entity. Services interract with ports. This is why your domain logic should stand. The framework has a class _BaseEntityService_ that bring basic CRUD operation (GetByID, Query, Create, Update, PartialUpdate, Delete).

```
public class MyModelService : BaseEntityService<MyModel>, IMyModelService
{
}
```

#### Creating Data Context & Repository

This is the adapter part of our data acess. First we need to create our DbContext which hold the configuration for models. Once again, I highly recommand [Ef Power Tools](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools) for this task. Once the context and the DbSet for models are defined we just need to create your repository class by inheriting _BaseEfRepository_. _BaseEfRepository_ will implement ef operation to the _BaseEntityService_ methods.

```
    public class MyModelCatalogRepository<T> : BaseEfRepository<T, MyModelCatalogContext> where T : class, IBaseEntity
    {
        public MyModelCatalogRepository(MyModelCatalogContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
```

#### Querying

One query, one class, ideally. Class query need to inherit _BaseQuery<T>_ and implement the _BuildQuery_ method.
```
    public class BaseMyModelQuery : BaseQuery<MyModel>
    {
        public long? ID { get; set; }
        public override void BuildQuery()
        {
            if (ID.HasValue && ID.Value > 0)
            {
                AddFunctionToCriteria(x => x.Id == ID);
            }
        }
    }
```


#### Giving life

We setup the minimal classes required by this framework and we just need to mix them all together with a decent DI framework to go to our first test. 
I'm using [Autofac](https://autofac.org/) as DI framework cause it gives more flexibility and power than the default Microsoft one.

Check [this file](https://github.com/Manu06D/makeITeasy.AppFramework/blob/master/sample/makeITeasy.CarCatalog.Tests/ServiceRegistrationAutofacModule.cs) for an autofac DI sample 

#### Playing

After completing previous step, you should be able to perform data creation, rich queries .. 

