using Microsoft.OpenApi.Models;
using ProLinked.Domain;
using ProLinked.Application;
using ProLinked.Infrastructure;

namespace ProLinked.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddAzureBlobStoring(
            builder.Configuration.GetValue<string>("AzureStorageAccountSettings:ConnectionString") ?? throw new KeyNotFoundException()
        );
        builder.Services.AddDbConnection(
            builder.Configuration.GetConnectionString("Default") ?? throw new KeyNotFoundException()
        );
        builder.Services.AddIdentity();
        builder.Services.AddRepositories();
        builder.Services.AddDomainServices();
        builder.Services.AddProLinkedLocalization();
        builder.Services.AddProLinkedAuthentication(builder.Configuration);
        builder.Services.AddAutoMapper();
        builder.Services.AddApplicationServices();


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        ConfigureSwagger(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }

    private static void ConfigureSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(c =>
        {
            c.MapType<DateOnly>(() => new OpenApiSchema {
                Type = "string",
                Format = "date" });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ProLinked Web Api",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }
}