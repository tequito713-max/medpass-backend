using MEDPASS.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MEDPASS API", Version = "v1" });
});

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IClinicaService, ClinicaService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<IEstudioService, EstudioService>();
builder.Services.AddScoped<IAlergiaService, AlergiaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MEDPASS API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();