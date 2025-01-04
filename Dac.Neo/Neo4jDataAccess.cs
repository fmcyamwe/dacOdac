extern alias CustomTypes;

using Dac.Neo.Data.Model;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Neo4j.Driver.Extensions;

namespace Dac.Neo;

    //should be in own file but for brevity
    public interface INeo4jDataAccess : IAsyncDisposable
    {
        Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);
       
        Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);

        Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null);

        Task<T> ExecuteReadScalarToModelAsync<T>(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);

        Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null);

        Task ExecuteWriteTransactionAsync(string query, IDictionary<string, object>? parameters = null);
    }

    public class Neo4jDataAccess : INeo4jDataAccess
    {
        private IAsyncSession _session;

        private ILogger<Neo4jDataAccess> _logger;

        private string _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Neo4jDataAccess"/> class.
        /// </summary>
        public Neo4jDataAccess(IDriver driver, ILogger<Neo4jDataAccess> logger) //IOptions<ApplicationSettings> appSettingsOptions)
        {
            _logger = logger;
            _database = "neo4j"; //appSettingsOptions.Value.Neo4jDatabase ?? "neo4j";  // todo** re-add
            _session = driver.AsyncSession(o => o.WithDatabase(_database));
            
        }

        /// <summary>
        /// Execute read list as an asynchronous operation.
        /// </summary>
        public async Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            return await ExecuteReadTransactionAsync<string>(query, returnObjectKey, parameters);
        }

        /// <summary>
        /// Execute read dictionary as an asynchronous operation.
        /// </summary>
        public async Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            return await ExecuteReadTransactionAsync<Dictionary<string, object>>(query, returnObjectKey, parameters);
        }

        /// <summary>
        /// Execute read scalar as an asynchronous operation.
        /// </summary>
        public async Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;

                var result = await _session.ReadTransactionAsync(async tx =>
                {
                    T scalar = default(T);

                    var res = await tx.RunAsync(query, parameters);
                    scalar = (await res.SingleAsync())[0].As<T>(); //duh trying to convert INode to T smh

                    return scalar;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }

        /// <summary>
        /// Execute read scalar model in an asynchronous operation.
        /// </summary>
        public async Task<T> ExecuteReadScalarToModelAsync<T>(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;
                
                //still complains when using T smh 
                //workaround until Dac.Neo.Model removed  sooo bad!--toReview**

                //var results = (await _session.RunReadTransactionForObjects<T>(query, parameters, returnObjectKey)).Single(); 
                //:(await _session.RunReadTransactionForObjects<DoctorDB>(query, parameters, returnObjectKey)).Single();
                
                switch (returnObjectKey)
                {
                    case "p":
                        var p = (await _session.RunReadTransactionForObjects<PatientDB>(query, parameters, returnObjectKey)).Single();
                        _logger.LogInformation("ExecuteReadScalarToModelAsync {p}",p);
                        return p.As<T>();
                    case "d":
                        var d = (await _session.RunReadTransactionForObjects<DoctorDB>(query, parameters, returnObjectKey)).Single();
                        _logger.LogInformation("ExecuteReadScalarToModelAsync {d}",d);
                        return d.As<T>();
                    case "t":
                        var t = (await _session.RunReadTransactionForObjects<Treatment>(query, parameters, returnObjectKey)).FirstOrDefault(); //prolly only first? toReview**
                        _logger.LogInformation("ExecuteReadScalarToModelAsync {d}",t);
                        return t.As<T>();
                    default:
                        _logger.LogInformation("ExecuteReadScalarToModelAsync:: ERROR unknown returnObjectKey {returnObjectKey}",returnObjectKey);
                        return default(T); //bon to see...
                }
                
                //_logger.LogInformation("ExecuteReadScalarToModelAsync {results}",results);
                
                //return results.As<T>(); //smdh cant use T ....sigh
                
               
                /*
                var result = await _session.ReadTransactionAsync(async tx =>
                {
                    T scalar = default(T);
                    var cursor = await tx.RunAsync(query, parameters);
                     //cursor.SingleAsync<INode>(returnObjectKey)
                    await foreach(var node in cursor.GetContent<INode>(returnObjectKey))//("m")
                    {
                        //scalar= 
                        node.ToObject<T>(returnObjectKey);
                    } 
                    
                    //res.FetchAsync(); //gotta use this in order to get Current as INode then cast to T with .ToObject..
                    var fetched = await cursor.FetchAsync(); 
                    while (fetched){
                        _logger.LogInformation("ExecuteReadScalarToModelAsync {cursor}",cursor.Current);
                        
                        var node = cursor.Current[returnObjectKey].As<INode>();
                        //var Title = node.GetValue<string>("title"); //huh also complains
                        
                        _logger.LogInformation("ExecuteReadScalarToModelAsync {returnObjectKey} >> toType: {type} >> {node}",returnObjectKey, typeof(T),node);
                        //return node.ToObject<T>();
                        //scalar = 
                        node.ToObject<T>();//complains smh but normal as using ReadTransactionAsync from .Driver package smdh
                        
                        //The type 'IRelationship' is defined in an assembly that is not referenced. 
                        //You must add a reference to assembly 'Neo4j.Driver, Version=4.3.2.2, Culture=neutral, PublicKeyToken=b646bc66d277ac07'.
                        scalar = (T) node; //would cast work?!? >>nope :(
                        
                        return scalar;
                    }
                    
                    //scalar = (await res.SingleAsync())[0].As<T>(); //duh trying to convert INode to T smh

                    return scalar;
                });

                return result;
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }

        /// <summary>
        /// Execute read transaction as an asynchronous operation.
        /// </summary>
        private async Task<List<T>> ExecuteReadTransactionAsync<T>(string query, string returnObjectKey, IDictionary<string, object>? parameters)
        {
            try
            {                
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;

                var result = await _session.ReadTransactionAsync(async tx =>
                {
                    var data = new List<T>();

                    var res = await tx.RunAsync(query, parameters);

                    var records = await res.ToListAsync();

                    data = records.Select(x => (T)x.Values[returnObjectKey]).ToList();

                    return data;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }
                
        /// <summary>
        /// Execute write transaction with return value
        /// </summary>
        public async Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;

                var result = await _session.WriteTransactionAsync(async tx =>
                {
                    T scalar = default(T);

                    var res = await tx.RunAsync(query, parameters);

                    scalar = (await res.SingleAsync())[0].As<T>();

                    return scalar;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }

        /// <summary>
        /// Execute write transaction with no return value
        /// </summary>
        public async Task ExecuteWriteTransactionAsync(string query, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters == null ? new Dictionary<string, object>() : parameters;
                
                await _session.WriteTransactionAsync(tx => tx.RunAsync(query, parameters));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources asynchronously.
        /// </summary>
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _session.CloseAsync();
        }
    }