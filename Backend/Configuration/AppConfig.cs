namespace Backend.Configuration;

public static class AppConfig
{
    /// <summary>
    /// Method which configures the app pipeline.
    /// </summary>
    public static async Task ConfigPipeline(this WebApplication app, WebApplicationBuilder builder)
    {
        // Error handling first
        app.UseMiddleware<ExceptionMiddleware>();

        // Dev tooling
        app.UseSwagger();
        app.UseSwaggerUI();

        // Security basics before routing
        app.UseHttpsRedirection();

        // Routing must come before CORS/auth
        app.UseRouting();

        // CORS between routing and auth
        app.UseCors("AllowAll");

        // Authn then Authz
        app.UseAuthentication();
        app.UseAuthorization();

        // Map endpoints including indentity auth
        app.MapGroup("/api/Account").WithTags("Account").MapIdentityApi<UserEntity>();
        
        app.MapControllers();

        // Migrations and seed
        await app.ConfigureDatabase();
    }
}
