using System;
using System.IO;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.TOAVGBetBenchmark.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddConfig();
services.AddFanexSql(configuration);
services.AddFanexLogSentry(configuration);
services.AddInternalServices();

// Configure services
services.AddControllers();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.MapControllers();

await app.RunAsync();