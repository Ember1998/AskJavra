using AskJavra.DataContext;
using AskJavra.Models.Contribution;
using AskJavra.Models.Post;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Reflection;
using static AskJavra.Constant.Constants;

namespace AskJavra.Repositories.Service
{
    public class PostService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<Post> _dbSet;
        private readonly DbSet<PostUpVote> _voteSet;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DbSet<ContributionPointType> _dbSetPointType;
        private readonly DbSet<ContributionPoint> _dbSetPoint;
        private readonly IConfiguration _configuration;

        public PostService(
            ApplicationDBContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration
            )
        {
            _context = context;
            _dbSet = _context.Set<Post>();
            _userManager = userManager;
            _voteSet = _context.Set<PostUpVote>();
            _dbSetPointType = _context.Set<ContributionPointType>();
            _dbSetPoint = _context.Set<ContributionPoint>();
            _configuration = configuration;
        }

        public async Task<ResponseFeedDto> GetAllAsync(FeedRequestDto request)
        {
            var responseResult = new ResponseFeedDto();
            var post =  _dbSet.Include(t => t.Tags).ThenInclude(x=>x.Tag).Include(p => p.Threads).ThenInclude(x=>x.ThreadUpVotes).Include(X=>X.UpVotes).ThenInclude(x=>x.User).AsQueryable();
            if (post == null)
                return responseResult;            

            if (request.SearchTerm != null && request.SearchTerm.Length > 0)
                post = post.Where(x => x.Title.Contains(request.SearchTerm) || x.Description.Contains(request.SearchTerm));
            
            if (request.Filters != null)
                post = post.Where(x => x.FeedStatus.Equals(request.Filters));
            if (request.TagIds != null)
                post = post.Where(x => x.Tags.Any(y => request.TagIds.Contains(y.TagId.Value)));
            //post = post.Where(x => request.TagIds.Contains(x.Tags.Select(y => y.TagId)));

            if (!request.UserId.IsNullOrEmpty())
                post = post.Where(x => x.CreatedBy == request.UserId);

            bool isSorted = false;
            do
            {
                switch (request.SortBy)
                {
                    case "title":
                        post = request.SortOrder == "asc" ? post.OrderBy(x => x.Title) : post.OrderByDescending(x => x.Title);
                        isSorted = true;
                        break;
                    case "creation":
                        post = request.SortOrder == "asc" ? post.OrderBy(x => x.CreatedAt) : post.OrderByDescending(x => x.CreatedAt);
                        isSorted = true;
                        break;
                    case "upvote":
                        post = request.SortOrder == "asc" ? post.OrderBy(x => x.UpVotes.Count) : post.OrderByDescending(x => x.UpVotes.Count);
                        isSorted = true;
                        break;
                    default:
                        // Default sort by CreatedAt descending if SortBy is not specified or invalid
                        post = post.OrderByDescending(x => x.CreatedAt);
                        isSorted = true;
                        break;
                }
            } while (!isSorted);

            int totalRecord = await post.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecord / request.PageSize);
            var skip = (request.PageNumber - 1) * request.PageSize;
            post = post.Skip(skip).Take(request.PageSize).AsQueryable();

            //var check = post.ToList();
            var result =await post.Select(x => new PostViewDto
            {
                Title = x.Title,
                Description = x.Description,
                PostType = x.PostType,
                PostId = x.Id,
                CreatedBy = x.CreatedBy,
                CreationAt = x.CreatedAt,
                FeedStatus = x.FeedStatus,
                PostTypeName = GetEnumDescription(x.PostType),
                FeedStatusName = GetEnumDescription(x.FeedStatus),
                IsAnonymous = x.IsAnonymous,
                CreatedByUser =x.CreatedBy != null? _context.Users.Where(z => z.Id == x.CreatedBy)
                .Select(user => new ApplicationUserViewDtocs
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName

                })
                .FirstOrDefault(): new ApplicationUserViewDtocs(),
                //CreatedByUser = await GetUsersAsync(x.CreatedBy).Result,
                Tags = x.Tags.Select(t => new PostTagDto
                {
                    PostId = t.PostId,
                    TagId = t.TagId,
                    TagDescription = t.Tag.TagDescription,
                    TagName = t.Tag.Name,
                    CreationAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy

                }).ToList(),
                postThreads = x.Threads.Select(t => new PostThreadViewDto
                {
                    PostId = t.PostId,
                    ThreadDescription = t.ThreadDescription,
                    ThreadId = t.Id,
                    ThreadTitle = t.ThreadTitle,
                    ThreadUpVoteCount = t.ThreadUpVotes.Count,
                    CreatedByUser = t.CreatedBy != null ? _context.Users.Where(z => z.Id == t.CreatedBy)
                        .Select(user => new ApplicationUserViewDtocs
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            FullName = user.FullName

                        }).FirstOrDefault() : new ApplicationUserViewDtocs(),
                    ThreadUpVotes = t.ThreadUpVotes.Select(upThread=>new ThreadUpvoteResponseDto
                    {
                         ThreadDescription = upThread.Thread.ThreadDescription,
                         ThreadTitle = upThread.Thread.ThreadTitle,
                         UpvoteBy = upThread.UserId
                    }).ToList()

                }).ToList(),
                UpVotes = x.UpVotes.Select(y=> new UpvoteCountViewMode
                {
                    Id =y.Id,
                    PostId = y.PostId,
                    UserId = y.UserId,
                    UserName = y.User.UserName
                }).ToList(),
                TotalUpvoteCount = x.UpVotes.Count

            }).ToListAsync();

            responseResult.Feeds = result;
            responseResult.PageNumber = request.PageNumber;
            responseResult.PageSize = request.PageSize;
            responseResult.TotalRecords = totalRecord;
            responseResult.TotalPages = totalPages;

            return responseResult;
        }
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
        public async Task<ResponseDto<PostViewDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var post = await _dbSet.Include(t => t.Tags).ThenInclude(x => x.Tag).Include(p => p.Threads).ThenInclude(x => x.ThreadUpVotes).Include(X => X.UpVotes).Where(x=>x.Id == id).FirstOrDefaultAsync();
                if (post != null)
                {
                    var result = new PostViewDto
                    {
                        Title = post.Title,
                        Description = post.Description,
                        PostType = post.PostType,
                        PostId = post.Id,
                        CreatedBy = post.CreatedBy,
                        CreationAt = post.CreatedAt,
                        FeedStatus = post.FeedStatus,
                        PostTypeName = GetEnumDescription(post.PostType),
                        FeedStatusName = GetEnumDescription(post.FeedStatus),
                        Screenshot = post.ScreenshotPath,
                        CreatedByUser = _context.Users.Where(x=>x.Id == post.CreatedBy).Select(user => new ApplicationUserViewDtocs
                                {
                                    Id = user.Id,
                                    UserName = user.UserName,
                                    Email = user.Email,
                                    FullName = user.FullName

                                }).FirstOrDefault(),
                        IsAnonymous = post.IsAnonymous,
                        Tags = post.Tags.Select(t => new PostTagDto
                        {
                            PostId = t.PostId,
                            TagId = t.TagId,
                            TagDescription = t.Tag.TagDescription,
                            TagName = t.Tag.Name,
                            CreationAt = t.CreatedAt,
                            CreatedBy = t.CreatedBy

                        }).ToList(),
                        postThreads = post.Threads.Select(t => new PostThreadViewDto
                        {
                            PostId = t.PostId,
                            ThreadDescription = t.ThreadDescription,
                            ThreadId = t.Id,
                            ThreadTitle = t.ThreadTitle,
                            CreatedByUser = t.CreatedBy != null ? _context.Users.Where(z => z.Id == t.CreatedBy)
                                        .Select(user => new ApplicationUserViewDtocs
                                        {
                                            Id = user.Id,
                                            UserName = user.UserName,
                                            Email = user.Email,
                                            FullName = user.FullName

                                        }).FirstOrDefault() : new ApplicationUserViewDtocs(),
                            ThreadUpVotes = t.ThreadUpVotes.Select(upThread => new ThreadUpvoteResponseDto
                            {
                                ThreadDescription = upThread.Thread.ThreadDescription,
                                ThreadTitle = upThread.Thread.ThreadTitle,
                                UpvoteBy = upThread.UserId
                            }).ToList()
                        }).ToList(),
                        UpVotes = post.UpVotes.Select(y => new UpvoteCountViewMode
                        {
                            Id = y.Id,
                            PostId = y.PostId,
                            UserId = y.UserId,
                            UserName = y.User.UserName
                        }).ToList(),
                        TotalUpvoteCount = post.UpVotes.Count

                    };

                    return new ResponseDto<PostViewDto>(true, "Success", result);

                }
                else
                    return new ResponseDto<PostViewDto>(false, "not found", new PostViewDto());
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostViewDto>(false, ex.Message, new PostViewDto());
            }
        }

        public async  Task<ResponseDto<PostViewDto>> AddAsync(PostDto entity, IFormFile file)
        {
            try
            {
                if (!IsValidEnumValue(entity.FeedStatus))
                    return new ResponseDto<PostViewDto>(false, "not found", new PostViewDto());
                if (!IsValidEnumValue(entity.PostType))
                    return new ResponseDto<PostViewDto>(false, "not found", new PostViewDto());
                var post = new Post(entity.Title, entity.Description, entity.PostType, entity.FeedStatus, new List<PostThread>(), new List<PostTag>(), entity.CreatedBy, entity.IsAnonymous);
                string imagePath = string.Empty;
                if (file != null)
                  imagePath = await UploadFile(file);
                if(!imagePath.IsNullOrEmpty())
                    post.ScreenshotPath = imagePath;
                await _dbSet.AddAsync(post);

                await _context.SaveChangesAsync();
               
                var result = new PostViewDto
                {
                    Title = post.Title,
                    Description = post.Description,
                    PostType = post.PostType,
                    PostId = post.Id,
                    CreatedBy = post.CreatedBy,
                    CreationAt = post.CreatedAt,
                    FeedStatus = post.FeedStatus,
                    PostTypeName = GetEnumDescription(post.PostType),
                    FeedStatusName = GetEnumDescription(post.FeedStatus),
                    IsAnonymous = post.IsAnonymous,
                    CreatedByUser = _context.Users.Where(x => x.Id == post.CreatedBy).Select(user => new ApplicationUserViewDtocs
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName

                    }).FirstOrDefault(),
                    Tags = post.Tags.Select(t => new PostTagDto
                    {
                        PostId = t.PostId,
                        TagId = t.TagId,
                        TagDescription = t.Tag.TagDescription,
                        TagName = t.Tag.Name,
                        CreationAt = t.CreatedAt,
                        CreatedBy = t.CreatedBy

                    }).ToList(),
                    postThreads = post.Threads.Select(t => new PostThreadViewDto
                    {
                        PostId = t.PostId,
                        ThreadDescription = t.ThreadDescription,
                        ThreadId = t.Id,
                        ThreadTitle = t.ThreadTitle,
                        CreatedByUser = t.CreatedBy != null ? _context.Users.Where(z => z.Id == t.CreatedBy)
                        .Select(user => new ApplicationUserViewDtocs
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            FullName = user.FullName

                        }).FirstOrDefault() : new ApplicationUserViewDtocs(),

                    }).ToList(),
                      UpVotes = post.UpVotes.Select(y => new UpvoteCountViewMode
                      {
                          Id = y.Id,
                          PostId = y.PostId,
                          UserId = y.UserId,
                          UserName = y.User.UserName
                      }).ToList(),
                    TotalUpvoteCount = post.UpVotes.Count

                };
                await SetPoint(entity.CreatedBy, ContributionPointTypes.PostCreate);
                return new ResponseDto<PostViewDto>(true, "Record added successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostViewDto>(false, ex.Message, new PostViewDto());
            }
        }
        private async Task<string> UploadFile(IFormFile file)
        {
            try
            {
                var uploadPath = _configuration.GetValue<string>("ImageUploadPath");
                if (!string.IsNullOrEmpty(uploadPath))
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                if (file == null || file.Length == 0)
                {
                    return string.Empty;
                }

                var relativePath = Path.Combine(uploadPath, file.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return relativePath;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
            //}
            public static bool IsValidEnumValue<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }
        public async Task<ResponseDto<PostViewDto>> UpdateAsync(Post post,IFormFile file)
        {
            try
            {
                if (!IsValidEnumValue(post.FeedStatus))
                    return new ResponseDto<PostViewDto>(false, "not found", new PostViewDto());
                if (!IsValidEnumValue(post.PostType))
                    return new ResponseDto<PostViewDto>(false, "not found", new PostViewDto());
                post.LastModifiedAt = DateTime.UtcNow;

                if(await _dbSet.FindAsync(post.Id) == null)
                    return new ResponseDto<PostViewDto>(false, "not found", new PostViewDto());

                string imagePath = string.Empty;
                if (file != null)
                    imagePath = await UploadFile(file);

                _dbSet.Attach(post);
                _context.Entry(post).State = EntityState.Modified;
               
                await _context.SaveChangesAsync();
                var result = new PostViewDto
                {
                    Title = post.Title,
                    Description = post.Description,
                    PostType = post.PostType,
                    PostId = post.Id,
                    CreatedBy = post.CreatedBy,
                    CreationAt = post.CreatedAt,
                    FeedStatus = post.FeedStatus,
                    PostTypeName = GetEnumDescription(post.PostType),
                    FeedStatusName = GetEnumDescription(post.FeedStatus),
                    IsAnonymous = post.IsAnonymous,
                    Tags = post.Tags.Select(t => new PostTagDto
                    {
                        PostId = t.PostId,
                        TagId = t.TagId,
                        TagDescription = t.Tag.TagDescription,
                        TagName = t.Tag.Name,
                        CreationAt = t.CreatedAt,
                        CreatedBy = t.CreatedBy

                    }).ToList(),
                    postThreads = post.Threads.Select(t => new PostThreadViewDto
                    {
                        PostId = t.PostId,
                        ThreadDescription = t.ThreadDescription,
                        ThreadId = t.Id,
                        ThreadTitle = t.ThreadTitle
                    }).ToList()

                };
                return new ResponseDto<PostViewDto>(true, "Record updated successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostViewDto>(false, ex.Message, new PostViewDto());
            }
        }

        public async Task<ResponseDto<string>> DeleteAsync(Guid id)
        {
            try
            {
                string userId = string.Empty;
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    userId = entity.CreatedBy;
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();

                    await RevokePoint(userId, ContributionPointTypes.PostCreate);

                    return new ResponseDto<string>(true, "Record deleted successfully", userId);
                }
                else
                    return new ResponseDto<string>(false, "not found", userId);
            }
            catch (Exception ex)
            {
                return new ResponseDto<string>(false, ex.Message, string.Empty);
            }

        }
        public async Task<ResponseDto<PostUpvoteResponseDto>> UpvoteFeed(Guid postId, string upvoteBy)
        {
            try
            {
                string creatorId = string.Empty;

                var user  = await _userManager.FindByIdAsync(upvoteBy);
                if (user == null)
                    return new ResponseDto<PostUpvoteResponseDto>(false, "User not found", new PostUpvoteResponseDto());
                var post = await _dbSet.FindAsync(postId);
                if(post == null) 
                    return new ResponseDto<PostUpvoteResponseDto>(false, "Invalid Post Id", new PostUpvoteResponseDto());
                
                creatorId = post.CreatedBy;
                var checkIfUpVoteAlreadyExist = await _voteSet.Where(x => x.UserId == upvoteBy && x.PostId == postId).FirstOrDefaultAsync();
                
                if(checkIfUpVoteAlreadyExist == null)
                {
                    PostUpVote postUpVote = new PostUpVote
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = upvoteBy,
                        UserId = user.Id,
                        User = user,
                        PostId = postId,
                        Post = post
                    };
                    await _voteSet.AddAsync(postUpVote);
                    await _context.SaveChangesAsync();                   

                    
                    var response = new PostUpvoteResponseDto
                    {
                        PostDescription = post.Description,
                        PostTitle = post.Title,
                        UpvoteBy = user.FullName,
                        NeedPointRevoke = false,
                        PointUserId = user.Id
                    };

                   await SetPoint(user.Id, ContributionPointTypes.PostUpvote);

                    return new ResponseDto<PostUpvoteResponseDto>(true, "Feed upvote raised succesfully.", response);
                }
                else
                {
                    return await RevokeUpvoteFeed(checkIfUpVoteAlreadyExist,post, user.Id, user.FullName);
                }
               

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ResponseDto<PostUpvoteResponseDto>> RevokeUpvoteFeed(PostUpVote postUpVote,Post post, string userId, string userFullName)
        {
            try
            {
                var response = new PostUpvoteResponseDto
                {
                    PostDescription = post.Description,
                    PostTitle = post.Title,
                    UpvoteBy = userFullName,
                    NeedPointRevoke = true,
                    PointUserId = userId
                };
                if (postUpVote != null)
                {
                    _voteSet.Remove(postUpVote);
                    await _context.SaveChangesAsync();

                    await RevokePoint(userId, ContributionPointTypes.PostUpvote);

                    return new ResponseDto<PostUpvoteResponseDto>(true, "Upvote revoked successfully", response);
                }
                else
                    return new ResponseDto<PostUpvoteResponseDto>(false, "not found", new PostUpvoteResponseDto());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async Task<ApplicationUserViewDtocs> GetApplicationUserById(string userId)
        {
            var result = await  _userManager.FindByIdAsync(userId);
                if (result == null)
                    return new ApplicationUserViewDtocs();
                else
                    return new ApplicationUserViewDtocs { Email = result.Email, FullName = result.FullName, Id = result.Id, UserName = result.UserName};
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
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> RevokePoint(string userId, string pointType)
        {
            try
            {
                var pointTypeId = await _dbSetPointType.SingleOrDefaultAsync(x => x.Name == pointType);

                if (pointTypeId == null) return false;

                var point = await _dbSetPoint.SingleOrDefaultAsync(x => x.UserId == userId && x.ContributionPointTypeId == pointTypeId.Id);

                if (point == null) return false;

                _dbSetPoint.Remove(point);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
