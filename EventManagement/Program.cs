using EventManagement.Core.Repositories;
using EventManagement.InfraStructure;
using EventManagement.InfraStructure.Mapper;
using EventManagement.InfraStructure.Repositories;
using EventManagement.Services.Services;
using EventManagement.Services.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseSqlServer(
        "Server=(localdb)\\MSSQLLocalDB;Database=EventManagementDb;Trusted_Connection=True;",
        b => b.MigrationsAssembly("EventManagement.InfraStructure")
    ));



builder.Services.AddScoped<IEventManagementRepository, EventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISpeakerRepository, SpeakerRepository>();
builder.Services.AddScoped<IRegistrationReposiory, RegistrationRepository>();

builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SpeakerService>();
builder.Services.AddScoped<RegistrationService>();

builder.Services.AddControllers();


builder.Services.AddValidatorsFromAssemblyContaining<EventValidator>();


builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();