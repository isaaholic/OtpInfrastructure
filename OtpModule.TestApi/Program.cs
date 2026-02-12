using Microsoft.EntityFrameworkCore;
using OtpModule.Abstractions;
using OtpModule.Extensions;
using OtpModule.TestApi.Data;
using OtpModule.TestApi.Options;
using OtpModule.TestApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite("Data Source=otp.db"));

builder.Services.AddScoped<IOtpDbContext>(x =>
    x.GetRequiredService<AppDbContext>());

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection("Email"));

builder.Services.AddScoped<IOtpSender, EmailOtpSender>();

builder.Services.AddOtpModule();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
