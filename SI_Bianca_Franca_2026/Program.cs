using SI_Bianca_Franca_2026.Components;
using SI_Bianca_Franca_2026.Repositories.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Pessoa;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Services.Produto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IAppContextService, AppContextService>();

builder.Services.AddScoped<PaisesRepository>();
builder.Services.AddScoped<PaisesService>();

builder.Services.AddScoped<EstadosRepository>();
builder.Services.AddScoped<EstadosService>();

builder.Services.AddScoped<CidadesRepository>();
builder.Services.AddScoped<CidadesService>();

builder.Services.AddScoped<ClientesRepository>();
builder.Services.AddScoped<ClientesService>();

builder.Services.AddScoped<EmitentesRepository>();
builder.Services.AddScoped<EmitentesService>();

builder.Services.AddScoped<FornecedoresRepository>();
builder.Services.AddScoped<FornecedoresService>();

builder.Services.AddScoped<TransportadorasRepository>();
builder.Services.AddScoped<TransportadorasService>();

builder.Services.AddScoped<VeiculosRepository>();
builder.Services.AddScoped<VeiculosService>();

builder.Services.AddScoped<CategoriasRepository>();
builder.Services.AddScoped<CategoriasService>();

builder.Services.AddScoped<UnidadesMedidaRepository>();
builder.Services.AddScoped<UnidadesMedidaService>();

builder.Services.AddScoped<SkuAtributosChavesRepository>();
builder.Services.AddScoped<SkuAtributosChavesService>();

builder.Services.AddScoped<MetodosPagamentoRepository>();
builder.Services.AddScoped<MetodosPagamentoService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
