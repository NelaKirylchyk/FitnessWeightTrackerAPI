using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.CreateBodyWeightRecordCommand;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.UpdateBodyWeightRecordCommand
{

    public class UpdateBodyWeightRecordHandler : IRequestHandler<UpdateBodyWeightRecordCommand, bool>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<AddBodyWeightRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public UpdateBodyWeightRecordHandler(
            FitnessWeightTrackerDbContext context,
            ILogger<AddBodyWeightRecordHandler> logger,
            IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<bool> Handle(UpdateBodyWeightRecordCommand request, CancellationToken cancellationToken)
        {
            var record = await _context.BodyWeightRecords
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.UserId == request.UserId, cancellationToken);

            if (record == null)
                return false;

            // Update properties
            record.Weight = request.Record.Weight;
            record.Date = request.Record.Date;
            // ... any other fields ...

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
