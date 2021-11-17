using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.GenericRepository;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface IDetailsAction<TModel> where TModel : class
    {
        public Controller Controller { get; set; }
        public IGenericRepository<TModel> Repository { get; set; }
        public IMapper Mapper { get; set; }
        public Task<IActionResult> ActionResult(int id);
    }
}
