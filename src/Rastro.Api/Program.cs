using Microsoft.AspNetCore.Mvc;
using Rastro.Application;
using Rastro.Infrastructure;
using Rastro.Api.Common;
using Rastro.Api.Middleware;
using Rastro.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);



// Servicios al contenedor
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(
    builder.Configuration,builder.Environment);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(op =>
    {
        op.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => new ApiError("Validation.Error", e.ErrorMessage))
                .ToList();
            var response = new ApiResponse<object> { IsSuccess = false, Errors = errors };
            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.UseDataBaseMigrationAsync(app.Environment);
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
