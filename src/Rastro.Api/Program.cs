using Rastro.Application;
using Rastro.Infrastructure;
using Rastro.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);



// Servicios al contenedor
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(
    builder.Configuration,builder.Environment);


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

app.UseHttpsRedirection();


app.Run();
