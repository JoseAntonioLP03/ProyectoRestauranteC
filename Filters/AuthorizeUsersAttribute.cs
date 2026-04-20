using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProyectoRestauranteC_.Filters
{
    public class AuthorizeUsersAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _rol;

        public AuthorizeUsersAttribute(string rol = "")
        {
            _rol = rol;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity == null || user.Identity.IsAuthenticated == false)
            {
                var tempDataFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ITempDataDictionaryFactory>();
                var tempData = tempDataFactory.GetTempData(context.HttpContext);
                tempData["MostrarAlertaLogin"] = true;

                context.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new { controller = "Acceso", action = "Login" }
                ));
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(_rol) && !user.IsInRole(_rol))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(
                        new { controller = "Acceso", action = "Denegado" }
                    ));
                    return;
                }
            }
        }
    }
}
