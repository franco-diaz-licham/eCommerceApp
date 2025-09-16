namespace Backend.Configuration;

public static class IdentityServices
{
    public static void AddIdentityServices(this WebApplicationBuilder builder)
    {
        //// Register identity services.
        //builder.Services.AddIdentityCore<UserEntity>(options =>
        //{
        //    options.Password.RequireNonAlphanumeric = false;
        //    options.User.RequireUniqueEmail = true;
        //})
        //.AddRoles<IdentityRole>()
        //.AddEntityFrameworkStores<DataContext>()
        //.AddSignInManager();

        //// Authentication
        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        //{
        //    // client send token as auth header
        //    options.TokenValidationParameters = new TokenValidationParameters()
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Token:Key")!)),
        //        ValidateIssuer = true,
        //        ValidIssuer = builder.Configuration.GetValue<string>("Token:Issuer")!,
        //        ValidateAudience = false
        //    };
        //});

        builder.Services.AddAuthorization();
        builder.Services.AddIdentityApiEndpoints<UserEntity>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<DataContext>()
        .AddSignInManager();
    }
}
