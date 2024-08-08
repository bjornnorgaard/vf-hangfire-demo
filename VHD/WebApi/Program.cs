using WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.AddHangfire();

var app = builder.Build();
app.UseHangfire();
app.Run();
