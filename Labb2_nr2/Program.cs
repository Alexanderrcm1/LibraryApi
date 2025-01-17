
using System.Text.Json.Serialization;
using Labb2_nr2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Labb2_nr2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers().AddJsonOptions(options =>
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            var connectionString = builder.Configuration.GetConnectionString("BooksDb");

			if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Azure")
			{
                builder.Configuration.AddUserSecrets("f1fb7eb6-e742-4f72-9131-baefe152ac69");
                var connBuilder = new SqlConnectionStringBuilder(connectionString)
                {
                    Password = builder.Configuration["DbPassword"]
                };
                connectionString = connBuilder.ConnectionString;
            }
            builder.Services.AddDbContext<LibraryDbContext>(opt =>
	            opt.UseSqlServer(connectionString));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            app.Run();
        }
    }
}
