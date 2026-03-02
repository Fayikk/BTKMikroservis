using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;

namespace OrderApi.Policies
{
    public static class PollyPolicies
    {
            public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,       
                durationOfBreak: TimeSpan.FromSeconds(20),   
                onBreak: (outcome, duration) =>
                {
                    Console.WriteLine(
                        $"🔴 [CIRCUIT AÇIK] Ödeme servisi {duration.TotalSeconds}sn devre dışı. " +
                        $"Sebep: {outcome.Result?.StatusCode ?? (object?)outcome.Exception?.GetType().Name}"
                    );
                },
                onReset: () =>
                {
                    Console.WriteLine("🟢 [CIRCUIT KAPALI] Ödeme servisi geri döndü, istekler devam ediyor.");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("🟡 [CIRCUIT YARI AÇIK] Test isteği gönderiliyor...");
                }
            );
    }

         public static IAsyncPolicy<HttpResponseMessage> RetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()          
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"🔄 [RETRY {retryCount}/3] StatusCode: {outcome.Result?.StatusCode} " +
                        $"— {timeSpan.TotalSeconds:F0}sn bekleniyor..."
                    );
                }
            );
    }
 public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            seconds: 5,
            onTimeoutAsync: (context, timeSpan, task) =>
            {
                Console.WriteLine(
                    $"⏰ [TIMEOUT] Ödeme servisi {timeSpan.TotalSeconds}sn içinde yanıt vermedi."
                );
                return Task.CompletedTask;
            }
        );
    }
    
    }
}