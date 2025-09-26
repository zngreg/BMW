// using System.Threading.RateLimiting;
// using Microsoft.AspNetCore.RateLimiting;

// namespace BMW.Books.CatalogueService.Configuration
// {
//     public static class RateLimiterConfig
//     {
//         public static void AddGlobalRateLimiter(this IServiceCollection services)
//         {
//             services.AddRateLimiter(options =>
//             {
//                 options.GlobalLimiter =
//                     RateLimitPartition.GetFixedWindowLimiter<string>(
//                     partitionKey: context => "global",
//                     factory: _ => new FixedWindowRateLimiterOptions
//                     {
//                         PermitLimit = 10,
//                         Window = TimeSpan.FromSeconds(10),
//                         QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                         QueueLimit = 2
//                     });
//             });
//         }
//     }
// }
