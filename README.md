MongoDB.Bson.BsonSerializationException: The property 'firstName' of type 'OnlineShoppingAPI.Collection.Login' cannot use element name 'emailAddress' because it is already being used by property 'emailAddress'.
   at MongoDB.Bson.Serialization.BsonClassMap.Freeze()
   at MongoDB.Bson.Serialization.BsonClassMap.LookupClassMap(Type classType)
   at MongoDB.Bson.Serialization.BsonClassMapSerializationProvider.GetSerializer(Type type, IBsonSerializerRegistry serializerRegistry)
   at MongoDB.Bson.Serialization.BsonSerializerRegistry.CreateSerializer(Type type)
   at System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd(TKey key, Func`2 valueFactory)
   at MongoDB.Bson.Serialization.BsonSerializerRegistry.GetSerializer[T]()
   at MongoDB.Driver.MongoCollectionImpl`1..ctor(IMongoDatabase database, CollectionNamespace collectionNamespace, MongoCollectionSettings settings, ICluster cluster, IOperationExecutor operationExecutor)
   at MongoDB.Driver.MongoDatabaseImpl.GetCollection[TDocument](String name, MongoCollectionSettings settings)
   at OnlineShopping.Services.MongoDBService..ctor(IOptions`1 mongoDBSettings) in C:\Users\Administrator\Downloads\OnlineShoppingAPI-master\OnlineShoppingAPI-master\OnlineShoppingAPI\Services\MongoDBService.cs:line 30
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Span`1& arguments, Signature sig, Boolean constructor, Boolean wrapExceptions)
   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.VisitConstructor(ConstructorCallSite constructorCallSite, RuntimeResolverContext context)
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteVisitor`2.VisitCallSiteMain(ServiceCallSite callSite, TArgument argument)
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.VisitRootCache(ServiceCallSite callSite, RuntimeResolverContext context)
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteVisitor`2.VisitCallSite(ServiceCallSite callSite, TArgument argument)
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.Resolve(ServiceCallSite callSite, ServiceProviderEngineScope scope)
   at Microsoft.Extensions.DependencyInjection.ServiceProvider.CreateServiceAccessor(Type serviceType)
   at System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd(TKey key, Func`2 valueFactory)
   at Microsoft.Extensions.DependencyInjection.ServiceProvider.GetService(Type serviceType, ServiceProviderEngineScope serviceProviderEngineScope)
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.ServiceProviderEngineScope.GetService(Type serviceType)
   at Microsoft.Extensions.DependencyInjection.ActivatorUtilities.GetService(IServiceProvider sp, Type type, Type requiredBy, Boolean isDefaultParameterRequired)
   at lambda_method9(Closure , IServiceProvider , Object[] )
   at Microsoft.AspNetCore.Mvc.Controllers.ControllerActivatorProvider.<>c__DisplayClass7_0.<CreateActivator>b__0(ControllerContext controllerContext)
   at Microsoft.AspNetCore.Mvc.Controllers.ControllerFactoryProvider.<>c__DisplayClass6_0.<CreateControllerFactory>g__CreateController|0(ControllerContext controllerContext)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeInnerFilterAsync()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)

HEADERS
=======
Accept: */*
Connection: keep-alive
Host: localhost:7222
User-Agent: PostmanRuntime/7.29.2
Accept-Encoding: gzip, deflate, br
Content-Type: application/json
Content-Length: 87
Postman-Token: 2f48d8b7-6f6c-47fb-a15a-dda5415a9347
