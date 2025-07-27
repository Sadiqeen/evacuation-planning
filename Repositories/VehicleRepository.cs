
using CourseService.Models.Database;
using EvacuationPlanning.Models;
using EvacuationPlanning.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvacuationPlanning.Repositories
{
    public class VehicleRepository : BaseRepository<TableVehicle>,  IVehicleRepository
    {
        public VehicleRepository(DatabaseContext dbContext) : base(dbContext)
        { }
    }
}