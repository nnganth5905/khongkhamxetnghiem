using KhamXetNghiem.Api.Services.Interfaces;
using KhamXetNghiem.Api.Utilities;

namespace KhamXetNghiem.Api.Services.Background;

public sealed class AppointmentNoShowWorker
    : BackgroundService
{
    private readonly IServiceScopeFactory
        _scopeFactory;

    private readonly ILogger<AppointmentNoShowWorker>
        _logger;

    public AppointmentNoShowWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<AppointmentNoShowWorker> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken
    )
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now =
                VietnamTime.Now;

            var next =
                now.Date.AddDays(1)
                    .AddMinutes(1);

            /*
             * Nếu app khởi động trước 00:01,
             * chạy ở 00:01 cùng ngày.
             */
            if (
                now.TimeOfDay
                <
                TimeSpan.FromMinutes(1)
            )
            {
                next =
                    now.Date.AddMinutes(1);
            }

            var delay =
                next - now;

            await Task.Delay(
                delay,
                stoppingToken
            );

            try
            {
                using var scope =
                    _scopeFactory.CreateScope();

                var service =
                    scope.ServiceProvider
                        .GetRequiredService<
                            IAppointmentService
                        >();

                var updated =
                    await service
                        .MarkExpiredNoShowsAsync(
                            stoppingToken
                        );

                _logger.LogInformation(
                    "Appointment scheduler updated {Count} expired appointments to no_show.",
                    updated
                );
            }
            catch (
                OperationCanceledException
            )
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Appointment no-show scheduler failed."
                );
            }
        }
    }
}