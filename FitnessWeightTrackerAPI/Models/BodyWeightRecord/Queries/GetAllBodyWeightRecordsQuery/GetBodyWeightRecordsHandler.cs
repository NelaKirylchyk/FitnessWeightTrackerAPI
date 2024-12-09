using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Queries.GetAllBodyWeightRecordsQuery
{
    public class GetBodyWeightRecordsHandler : IRequestHandler<GetBodyWeightRecordsQuery, BodyWeightRecord[]>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetBodyWeightRecordsHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<BodyWeightRecord[]> Handle(GetBodyWeightRecordsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"BodyWeightRecords_{request.UserId}";
            var cachedRecords = await _cache.GetStringAsync(cacheKey);
            if (cachedRecords != null)
            {
                return JsonSerializer.Deserialize<BodyWeightRecord[]>(cachedRecords);
            }

            var records = request.AscendingOrder ?
                await _context.BodyWeightRecords.AsNoTracking()
                    .Where(record => record.UserId == request.UserId)
                    .OrderBy(record => record.Date)
                    .ToArrayAsync(cancellationToken) :
                await _context.BodyWeightRecords.AsNoTracking()
                    .Where(record => record.UserId == request.UserId)
                    .OrderByDescending(record => record.Date)
                    .ToArrayAsync(cancellationToken);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(records));

            return records;
        }
    }
}
