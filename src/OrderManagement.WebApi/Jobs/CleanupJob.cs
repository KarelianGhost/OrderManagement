using Hangfire;

namespace OrderManagement.WebApi.Jobs;

public class CleanupJob
{
    public void Execute()
    {
        // Здесь может быть логика очистки устаревших данных
        Console.WriteLine("Фоновая задача выполнена: " + DateTime.UtcNow);
    }
}