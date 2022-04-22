using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using ChocAn.DataCenterConsole.Controllers;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DeleteAction<TModel> : IDeleteAction< TModel>
        where TModel : class
    {
        private const string LogExceptionTemplate = "DeleteAction: {ex}";
        private const string LogErrorTemplate = "DetailsAction: {error}";
        private const string NotFoundMessage = $"Item not found";
        public async Task<IActionResult> ActionResult(
            DataCenterController<TModel> controller,
            int id)
        {
            string error;

            try
            {
                var (success, model, errorMessage) = await controller.Service.DeleteAsync(id);
                if (success)
                {
                    return controller.RedirectToAction(ActionName.Index);
                }
                else
                {
                    // Record not found error
                    error = NotFoundMessage;
                    controller.Logger?.LogError(LogErrorTemplate, error);
                }
            }
            catch (Exception ex)
            {
                // Record exception
                controller.Logger?.LogError(LogExceptionTemplate, ex);
                error = ex.Message;
            }

            controller.ModelState.AddModelError("Error", error);

            return controller.RedirectToAction(ActionName.Details);
        }
    }
}
