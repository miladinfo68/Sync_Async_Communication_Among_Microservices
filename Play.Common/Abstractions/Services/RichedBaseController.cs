using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Play.Common.Abstractions.Repositories;
using Play.Common.Entities;

namespace Play.Common.Abstractions.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class RichedBaseController<T,U> : 
        ControllerBase 
        where T : ControllerBase 
        where U : IEntity
    {
        protected readonly IPublishEndpoint _bus;
        protected readonly ILogger<T> _logger;
        protected readonly IBaseRepository<U> _store;
        public RichedBaseController(IBaseRepository<U> store, IPublishEndpoint bus, ILogger<T> logger)
        {
            _store = store;
            _bus = bus;
            _logger = logger;
        }
    }
}