namespace Mcba.Middlewares;

public class AuthorizationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        int? customer = context.Session.GetInt32("Customer");

        if (customer == null)
        {
            var authAttr = context.GetEndpoint()?.Metadata?.GetMetadata<LoggedIn>();
            if (authAttr != null)
            {
                // Redirect to login page
                context.Response.Redirect("/Auth/Login");
                return;
            }
        }

        await _next(context);
    }
}
