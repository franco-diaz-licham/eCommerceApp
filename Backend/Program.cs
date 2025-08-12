// create builder and add services
var builder = WebApplication.CreateBuilder(args);
builder.AddServices();

// build app, configure pipeline and run
var app = builder.Build();
await app.Config(builder);
await app.RunAsync();
