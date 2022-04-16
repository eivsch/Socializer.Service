using API.Configuration;
using DomainModel.FeedEvents.Interfaces;
using DomainModel.Generators;
using DomainModel.Posts;
using DomainModel.Users;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.ThirdPartyServices;
using Infrastructure.WebGallery;
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

#region Infrastructure
// Config options
builder.Services.AddSingleton<IWebGalleryOptions, WebGalleryOptions>();
// Repositories
builder.Services.AddSingleton<IDatabaseConnection, SocializerDbConnection>((db) =>
{
    return new SocializerDbConnection(connectionString: builder.Configuration.GetConnectionString("SocializerDb"));
});
builder.Services.AddScoped<IFeedEventRepository, FeedEventRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
// File server
builder.Services.AddScoped<IFileServerClient, WebGalleryFileServerClient>();
// Generators
builder.Services.AddScoped<IPostPictureGenerator, WebGalleryApiClient>();
builder.Services.AddScoped<IRandomTextGenerator, Gpt3Client>((gp) => new Gpt3Client(builder.Configuration.GetValue<string>("Gpt3:ApiKey")));
builder.Services.AddScoped<INameGenerator, NameParserClient>();
builder.Services.AddScoped<IFaceGenerator, ThisPersonDoesNotExistClient>();

builder.Services.AddScoped<IFaceClassifier, AzureFaceRecognitionClient>((cl) =>
{
    return new AzureFaceRecognitionClient(
        endpoint: builder.Configuration.GetValue<string>("AzureFaceRecognition:ApiEndpoint"),
        apiKey: builder.Configuration.GetValue<string>("AzureFaceRecognition:ApiKey"));
});
#endregion

// Logic
builder.Services.AddScoped<IPostManager, PostManager>();
builder.Services.AddScoped<IRandomUserPostManager, RandomUserPostManager>();
builder.Services.AddScoped<IUserRegistrationManager, UserRegistrationManager>();
builder.Services.AddScoped<IUserGenerator, UserGenerator>();

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