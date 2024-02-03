using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Api.Infrastructure;
using WorkoutSample.Api.Options;
using WorkoutSample.Api.Services;
using WorkoutSample.Application;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddSignInManager()
    .AddRoles<IdentityRole<Guid>>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<WorkoutDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints()
    .AddExceptionHandler<CustomExceptionHandler>()
    .AddJWTBearerAuth(builder.Configuration.GetSection("Jwt").GetValue<string>("Key") ??
                      throw new InvalidOperationException("Set the jwt signing key"))
    .AddAuthorization(options => { });

builder.Services.SwaggerDocument(options => { options.AutoTagPathSegmentIndex = 1; });

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddDbContext<WorkoutDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WorkoutConnectionString")));

builder.Services.AddHostedService<MigrationsWorker>();

var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Run();

// builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
// builder.Services.AddAuthorizationBuilder();
//
// builder.Services.AddControllers(options =>
//     {
//         TypeConverterAttribute typeConverterAttribute = new TypeConverterAttribute(typeof(DateOnlyTypeConverter));
//         TypeDescriptor.AddAttributes(typeof(DateOnly), typeConverterAttribute);
//     })
//     .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()); });
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.MapType<DateOnly>(() => new OpenApiSchema()
//     {
//         Type = "string",
//         Format = "date",
//     });
// });
//
// builder.Services.AddDbContext<WorkoutDbContext>(options =>
// {
//     options.UseSqlite(builder.Configuration.GetConnectionString("WorkoutConnectionString"));
// });
//
// builder.Services.AddIdentityCore<ApplicationUser>()
//     .AddRoles<IdentityRole<Guid>>()
//     .AddEntityFrameworkStores<WorkoutDbContext>()
//     .AddApiEndpoints();
//
// builder.Services.AddHostedService<MigrationsWorker>();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// // app.UseHttpsRedirection();
//
// app.UseAuthorization();
//
// app.MapGroup("/users").MapIdentityApi<ApplicationUser>();
// app.MapControllers();
//
// app.Run();