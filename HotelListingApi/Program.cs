using HotelListingApi.Constants;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.Handlers;
using HotelListingApi.MappingProfiles;
using HotelListingApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<HotelListingDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<HotelListingDbContext>();
builder.Services.AddAuthentication( options =>
{
    options.DefaultAuthenticateScheme = AuthenticationDefaults.BasicScheme;
    options.DefaultChallengeScheme = AuthenticationDefaults.BasicScheme;
}).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { });
builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddScoped<ICountriesServices, CountriesServices>();
builder.Services.AddScoped<IHotelsServices, HotelsServices>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<HotelMappingProfile>();
    cfg.AddProfile<CountryMappingProfile>();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGroup("api/auth").MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


//"HotelListingDbConnectionString": "Server=localhost, 1433; Database=HotelListingDb; User Id=sa; Password=StrongPassword@123; Trusted_Connection=True; MultipleActiveResultSets=True; Encrypt=false;",