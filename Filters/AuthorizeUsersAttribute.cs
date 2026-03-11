using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProyectoRestauranteC_.Filters
{
    public class AuthorizeUsersAttribute : ActionFilterAttribute
    {
        private readonly string _rol;

        public AuthorizeUsersAttribute(string rol = "")
        {
            _rol = rol;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity!.IsAuthenticated)
            {
                var tempDataFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ITempDataDictionaryFactory>();
                var tempData = tempDataFactory.GetTempData(context.HttpContext);
                tempData["MostrarAlertaLogin"] = true;

                context.Result = new RedirectToActionResult("Login", "Acceso", null);
                return;
            }

            if (!string.IsNullOrEmpty(_rol) && !context.HttpContext.User.IsInRole(_rol))
            {
                context.Result = new RedirectToActionResult("Denegado", "Acceso", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
