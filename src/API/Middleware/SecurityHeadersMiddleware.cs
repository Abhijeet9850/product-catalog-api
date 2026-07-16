namespace API.Middleware
{
    /// <summary>
    /// Adds common security-related response headers to every request.
    /// Mitigates MIME sniffing, clickjacking, referrer leakage, and reduces
    /// the attack surface exposed to browsers.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;

                // Prevents browsers from MIME-sniffing a response away from the declared Content-Type
                headers["X-Content-Type-Options"] = "nosniff";

                // Prevents the response from being rendered inside a <frame>/<iframe> — mitigates clickjacking
                headers["X-Frame-Options"] = "DENY";

                // Limits how much referrer information is sent with outgoing requests from this app
                headers["Referrer-Policy"] = "no-referrer";

                // Restricts which browser features/APIs this app is allowed to use
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

                // Baseline CSP: only allow resources from this same origin.
                // Swagger UI's inline scripts/styles are exempted below via 'unsafe-inline'
                // for /swagger paths specifically — tighten further if Swagger is disabled in production.
                headers["Content-Security-Policy"] = context.Request.Path.StartsWithSegments("/swagger")
                    ? "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'; img-src 'self' data:;"
                    : "default-src 'self'";

                // Removes the default header revealing the server technology
                headers.Remove("X-Powered-By");

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
            => app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}