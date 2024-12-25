using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyXorNeuralNetworkApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем наш сервис в контейнер зависимостей
builder.Services.AddSingleton<NeuralNetworkService>();

// Добавляем контроллеры
builder.Services.AddControllers();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll"); // Применяем CORS перед обработкой запросов

// Включаем Swagger и Swagger UI в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Включаем маршрутизацию для контроллеров
app.MapControllers();

// Запуск приложения
app.Run();