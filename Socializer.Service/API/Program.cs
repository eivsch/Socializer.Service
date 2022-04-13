using DomainModel.FeedEvents.Interfaces;
using DomainModel.Posts;
using DomainModel.Users;
using Infrastructure;
using Infrastructure.Repositories;
using Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ClientCertificateOptions = ClientCertificateOption.Manual,
    ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
});

// Infrastructure
builder.Services.AddSingleton<IDatabaseConnection, SocializerDbConnection>((db) =>
{
    return new SocializerDbConnection(connectionString: builder.Configuration.GetConnectionString("SocializerDb"));
});
builder.Services.AddScoped<IFeedEventRepository, FeedEventRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IWebGalleryFileDownloader, WebGalleryFileDownloader>();
builder.Services.AddScoped<IWebGalleryService, WebGalleryService>((wb) =>
{
    return new WebGalleryService(
        webGalleryApiEndpoint: builder.Configuration.GetValue<string>("WebGallery:WebGalleryApiEndpoint"),
        webGalleryApiUser: builder.Configuration.GetValue<string>("WebGallery:WebGalleryUser")
    );
});

// Logic
builder.Services.AddScoped<IPostManager, PostManager>();
builder.Services.AddScoped<IRandomUserPostManager, RandomUserPostManager>();
builder.Services.AddScoped<IUserRegistrationManager, UserRegistrationManager>();

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

// TODO: Fine grain
app.UseCors(policy =>
    policy.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();
