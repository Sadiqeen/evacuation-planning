using CourseService.Models.Database;
using EvacuationPlanning.Models;
using EvacuationPlanning.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvacuationPlanning.Repositories
{
    public class EvacuationZoneRepository : BaseRepository<TableEvacuationZone>, IEvacuationZoneRepository
    {
        public EvacuationZoneRepository(DatabaseContext dbContext) : base(dbContext)
        {}
    }
}