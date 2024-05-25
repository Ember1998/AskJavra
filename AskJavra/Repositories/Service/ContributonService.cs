using AskJavra.DataContext;
using AskJavra.Models.Contribution;
using AskJavra.Models.Post;
using AskJavra.ViewModels.Dto;
using Microsoft.EntityFrameworkCore;
using static AskJavra.Constant.Constants;

namespace AskJavra.Repositories.Service
{
    public class ContributonService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DbSet<ContributionPointType> _dbSetPointType;
        private readonly DbSet<ContributionPoint> _dbSetPoint;
        private readonly DbSet<ContributionRank> _dbSetRank;
        public ContributonService(
            ApplicationDBContext dbContext
            )
        {
            _dbContext = dbContext;
            _dbSetPointType = _dbContext.Set<ContributionPointType>();
            _dbSetPoint = _dbContext.Set<ContributionPoint>();
            _dbSetRank = _dbContext.Set<ContributionRank>();
        }
        public async Task<bool> SetPoint(string userId, string pointType)
        {
            try
            {
                var pointTypeId = await _dbSetPointType.SingleOrDefaultAsync(x => x.Name == pointType);
                if (pointTypeId == null) return false;

                var point = new ContributionPoint
                {
                    ContributionPointTypeId = pointTypeId.Id,
                    Point = pointTypeId.Point,
                    UserId = userId
                };
                
                await _dbSetPoint.AddAsync(point);
                await _dbContext.SaveChangesAsync();
                
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> RevokePoint(string userId, string pointType)
        {
            try
            {
                var pointTypeId = await _dbSetPointType.SingleOrDefaultAsync(x => x.Name == pointType);
               
                if (pointTypeId != null) return false;

                var point = await _dbSetPoint.SingleOrDefaultAsync(x => x.UserId == userId && x.ContributionPointTypeId == pointTypeId.Id);
                
                if(point == null) return false;

                _dbSetPoint.Remove(point);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public UserRankDetails GetUserTotalPoints(string userId)
        {
            try
            {
                var result = _dbSetPoint.Where(x => x.UserId == userId).Select(x => x.Point).ToArray();
                int total_point = result.Sum();
                //var resultttt = await _dbSetRank.Where(x => x.RankMinPoint <= total_point && x.RankMaxPoint >= total_point).Select(y => new UserRankDetails
                //{
                //    RankName = y.RankName,
                //    TotalPoint = total_point
                //}).FirstOrDefaultAsync();
                return _dbSetRank.Where(x => x.RankMinPoint <= total_point && x.RankMaxPoint >= total_point).Select(y => new UserRankDetails
                {
                    RankName = y.RankName,
                    TotalPoint = total_point
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new UserRankDetails();
            }
        }
    }
}
