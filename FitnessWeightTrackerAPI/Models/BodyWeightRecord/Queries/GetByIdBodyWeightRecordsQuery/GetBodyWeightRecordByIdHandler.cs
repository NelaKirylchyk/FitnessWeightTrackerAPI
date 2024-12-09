using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Queries.GetByIdBodyWeightRecordsQuery
{
    public class GetBodyWeightRecordByIdHandler : IRequestHandler<GetBodyWeightRecordByIdQuery, BodyWeightRecord>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetBodyWeightRecordByIdHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<BodyWeightRecord> Handle(GetBodyWeightRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"BodyWeightRecord_{request.UserId}_{request.Id}";
            var cachedRecord = await _cache.GetStringAsync(cacheKey);
            if (cachedRecord != null)
            {
                return JsonSerializer.Deserialize<BodyWeightRecord>(cachedRecord);
            }

            var record = await _context.BodyWeightRecords.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.UserId == request.UserId, cancellationToken);

            if (record != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(record));
            }

            return record;
        }
    }
}
