using AutoMapper;
using ChocAn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class DataCenterController<TModel> : Controller
        where TModel : class
    {
        public ILogger<Controller> Logger { get; init; }
        public IMapper Mapper { get; init; }
        public IService<TModel> Service { get; init; }
        public DataCenterController(
            ILogger<Controller> logger,
            IMapper mapper,
            IService<TModel> service)
        {
            Logger = logger;
            Mapper = mapper;
            Service = service;
        }
    }
}
