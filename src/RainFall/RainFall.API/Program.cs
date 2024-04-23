using AutoMapper;
using MediatR;
using Microsoft.OpenApi.Models;
using RainFall.Application.Helper;
using RainFall.Application.Interface;
using RainFall.Application.Queries;
using RainFall.Application.QueryHandlers;
using RainFall.Application.Service;
using RainFall.Domain.Models;
using RainFall.Infrastructure;
using RainFall.Infrastructure.Interface;
using RainFall.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Add HTTP client
builder.Services.AddHttpClient();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// add services to MS DI container
{
    var services = builder.Services;
    
    services.Configure<EnvironmentAgencyConfiguration>(builder.Configuration.GetSection(EnvironmentAgencyConfiguration.SectionName));

    services.AddScoped(provider =>
    {
        // configure automapper with all automapper profiles from this assembly
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(provider, type));
            cfg.AddProfile(new AutoMapperProfile());
        });

        return mapperConfig.CreateMapper();
    });
    
    services.AddScoped<IEnvironmentAgencyAgent, EnvironmentAgencyAgent>();
    services.AddScoped<IRainFallReadingService, RainFallReadingService>();
    services.AddScoped<IRequestHandler<GetRainfallReadingPerStationQuery, RainfallReadingResponse>, GetRainfallReadingPerStationQueryHandler>();
    
    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "RainFall API", Version = "1.0.0" });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();