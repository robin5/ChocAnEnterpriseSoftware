using AutoMapper;
using ChocAn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class DataCenterController<TResource, TModel> : Controller
        where TResource : class
        where TModel : class
    {
        public ILogger<Controller> Logger { get; init; }
        public IMapper Mapper { get; init; }
        public IService<TResource, TModel> Service { get; init; }
        public DataCenterController(
            ILogger<Controller> logger,
            IMapper mapper,
            IService<TResource, TModel> service)
        {
            Logger = logger;
            Mapper = mapper;
            Service = service;
        }
    }
}
