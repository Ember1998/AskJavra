using AskJavra.DataContext;
using AskJavra.Models.Contribution;
using AskJavra.Models.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace AskJavra.Repositories.Service
{
    public class AdminService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DbSet<ContributionRank> _rankDbSet;
        public AdminService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _rankDbSet = _dbContext.Set<ContributionRank>();
        }
        public async Task<bool> UpdateRankPoint(int id, int minPoint, int maxPoint)
        {
            try
            {
                var rank = await _rankDbSet.FindAsync(id);
                if(rank == null) return false;
                rank.RankMaxPoint = maxPoint;
                rank.RankMinPoint = minPoint;

                _rankDbSet.Attach(rank);
                _dbContext.Entry(rank).State = EntityState.Modified;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
