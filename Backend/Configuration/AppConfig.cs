namespace Backend.Configuration;

public static class AppConfig
{
    /// <summary>
    /// Method which configures the app pipeline.
    /// </summary>
    public static async Task Config(this WebApplication app, WebApplicationBuilder builder)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowAll");
        app.UseHttpsRedirection();
        //app.UseAuthentication();
        //app.UseAuthorization();
        app.MapControllers();
        await app.ConfigureDatabase();
    }
}
