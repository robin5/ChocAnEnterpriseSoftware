using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ChocAn.Services;
using System;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DeleteAction<TResource, TModel> : IDeleteAction<TResource, TModel>
        where TResource : class
        where TModel : class
    {
        private const string LogExceptionTemplate = "DeleteAction: {ex}";
        private const string LogErrorTemplate = "DetailsAction: {error}";
        private const string NotFoundMessage = $"Item not found";
        public Controller Controller { get; set; }
        public ILogger<Controller> Logger { get; set; }
        public IService<TResource, TModel> Service { get; set; }
        public IMapper Mapper { get; set; }
        public async Task<IActionResult> ActionResult(int id, string indexAction, string detailsAction)
        {
            string error;

            try
            {
                var (success, model, errorMessage) = await Service.DeleteAsync(id);
                if (success)
                {
                    return Controller.RedirectToAction(indexAction);
                }
                else
                {
                    // Record not found error
                    error = NotFoundMessage;
                    Logger?.LogError(LogErrorTemplate, error);
                }
            }
            catch (Exception ex)
            {
                // Record exception
                Logger?.LogError(LogExceptionTemplate, ex);
                error = ex.Message;
            }

            Controller.ModelState.AddModelError("Error", error);

            return Controller.RedirectToAction(detailsAction);
        }
    }
}
