using AskJavra.DataContext;
using AskJavra.Models.Contribution;
using AskJavra.Models.Post;
using AskJavra.ViewModels.Dto;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using static AskJavra.Constant.Constants;

namespace AskJavra.Repositories.Service
{
    public class PostThreadService
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<PostThread> _dbSet;
        private readonly DbSet<ThreadUpVote> _threadUpvotedbSet;
        private readonly DbSet<Post> _postDBSet;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DbSet<ContributionPointType> _dbSetPointType;
        private readonly DbSet<ContributionPoint> _dbSetPoint;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;


        public PostThreadService(
            ApplicationDBContext context
            , UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
             IConfiguration configuration
            )
        {
            _context = context;
            _dbSet = _context.Set<PostThread>();
            _postDBSet = _context.Set<Post>();
            _threadUpvotedbSet = _context.Set<ThreadUpVote>();
            _userManager = userManager;
            _dbSetPointType = _context.Set<ContributionPointType>();
            _dbSetPoint = _context.Set<ContributionPoint>();
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task<List<PostThreadViewDto>> GetAllAsync()
        {
            var result = await _dbSet.Include(x => x.Post).ThenInclude(x => x.Tags).ThenInclude(x => x.Tag).ToListAsync();
            return result.Select(x => new PostThreadViewDto
            {
                PostId = x.PostId,
                ThreadTitle = x.ThreadTitle,
                ThreadDescription = x.ThreadDescription,
                Post = new PostViewDto
                {
                    Description = x.Post.Description,
                    PostType = (Enums.PostType)x.Post.PostType,
                    Title = x.Post.Title,
                    Tags = x.Post.Tags.Select(t => new PostTagDto
                    {
                        PostId = t.PostId,
                        TagId = t.TagId,
                        TagDescription = t.Tag.TagDescription,
                        TagName = t.Tag.Name

                    }).ToList(),
                }
            }).ToList();
        }

        public async Task<ResponseDto<PostThread>> GetByIdAsync(Guid id)
        {
            try
            {
                var postThread = await _dbSet.FindAsync(id);
                if (postThread == null) return new ResponseDto<PostThread>(false, "not found", postThread);
                else
                    return new ResponseDto<PostThread>(true, "Success", postThread);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThread>(false, ex.Message, new PostThread());
            }
        }
        public async Task<ResponseDto<PostThreadViewDto>> AddAsync(PostThreadCreateDto entity)
        {
            try
            {
                Post post = new Post();
                var postFetched = await _postDBSet.FindAsync(entity.PostId);

                if (postFetched != null)
                    post = postFetched;
                else
                    return new ResponseDto<PostThreadViewDto>(false, "Invalid post id", new PostThreadViewDto());

                var postThread = new PostThread(entity.ThreadTitle, entity.ThreadDescription, entity.PostId, post);
                postThread.CreatedBy = entity.CreatedBy;
                await _dbSet.AddAsync(postThread);
                await _context.SaveChangesAsync();
                
                var postCreaterFullName = _context.Users.Where(z => z.Id == post.CreatedBy)
                 .Select(user => new { user.FullName , user.Email })
                 .FirstOrDefault();
                var threadCreaterFullName = _context.Users.Where(z => z.Id == entity.CreatedBy)
                .Select(user => user.FullName)
                .FirstOrDefault();

                var feedbasepath = _configuration.GetValue<string>("FEBaseUrl");
                var path = feedbasepath + post.Id;

                if (entity.CreatedBy != null)
                    await SetPoint(entity.CreatedBy, ContributionPointTypes.ThreadCreate);
                await SendEmail(postCreaterFullName.FullName, path, threadCreaterFullName, post.Title, postCreaterFullName.Email);
                var result = new PostThreadViewDto
                {
                    PostId = postThread.PostId,
                    ThreadTitle = postThread.ThreadTitle,
                    ThreadDescription = postThread.ThreadDescription,
                    ThreadId = postThread.Id,
                    Post = new PostViewDto
                    {
                        Description = postThread.Post.Description,
                        PostType = (Enums.PostType)postThread.Post.PostType,
                        Title = postThread.Post.Title,
                        Tags = postThread.Post.Tags.Select(t => new PostTagDto
                        {
                            PostId = t.PostId,
                            TagId = t.TagId,
                            TagDescription = t.Tag.TagDescription,
                            TagName = t.Tag.Name

                        }).ToList(),
                    }
                };
                //await ConfigureSyncEmail(await _userManager.FindByIdAsync(entity.CreatedBy), result);
                return new ResponseDto<PostThreadViewDto>(true, "Record added successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThreadViewDto>(false, ex.Message, new PostThreadViewDto());
            }
        }

        public async Task<ResponseDto<PostThreadViewDto>> UpdateAsync(PostThread entity)
        {
            try
            {
                var post = await _postDBSet.FindAsync(entity.PostId);
                if (post == null)
                    return new ResponseDto<PostThreadViewDto>(false, "Invalid post id", new PostThreadViewDto());
                entity.Post = post;

                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                var result = new PostThreadViewDto
                {
                    PostId = entity.PostId,
                    ThreadTitle = entity.ThreadTitle,
                    ThreadDescription = entity.ThreadDescription,
                    Post = new PostViewDto
                    {
                        Description = entity.Post.Description,
                        PostType = (Enums.PostType)entity.Post.PostType,
                        Title = entity.Post.Title,
                        Tags = entity.Post.Tags.Select(t => new PostTagDto
                        {
                            PostId = t.PostId,
                            TagId = t.TagId,
                            TagDescription = t.Tag.TagDescription,
                            TagName = t.Tag.Name

                        }).ToList(),
                    }
                };
                return new ResponseDto<PostThreadViewDto>(true, "Record updated successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThreadViewDto>(false, ex.Message, new PostThreadViewDto());
            }
        }

        public async Task<ResponseDto<PostThreadDto>> DeleteAsync(Guid id)
        {
            try
            {
                string userId = string.Empty;
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    userId = entity.CreatedBy;
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    if (!userId.IsNullOrEmpty())
                        await RevokePoint(userId, ContributionPointTypes.ThreadCreate);
                    return new ResponseDto<PostThreadDto>(true, "Record deleted successfully", new PostThreadDto(entity.ThreadTitle, entity.ThreadDescription, entity.PostId));
                }
                else
                    return new ResponseDto<PostThreadDto>(false, "not found", new PostThreadDto());

            }
            catch (Exception ex)
            {
                return new ResponseDto<PostThreadDto>(false, ex.Message, new PostThreadDto());
            }
        }
        public async Task<ResponseDto<PostUpvoteResponseDto>> UpvoteThread(Guid threadId, string upvoteBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(upvoteBy);
                if (user == null)
                    return new ResponseDto<PostUpvoteResponseDto>(false, "User not found", new PostUpvoteResponseDto());
                var thread = await _dbSet.FindAsync(threadId);
                if (thread == null)
                    return new ResponseDto<PostUpvoteResponseDto>(false, "Invalid Post Id", new PostUpvoteResponseDto());
                var checkIfUpVoteAlreadyExist = await _threadUpvotedbSet.Where(x => x.UserId == upvoteBy && x.ThreadId == threadId).FirstOrDefaultAsync();
                if (checkIfUpVoteAlreadyExist == null)
                {
                    ThreadUpVote postUpVote = new ThreadUpVote
                    {
                        UserId = user.Id,
                        User = user,
                        ThreadId = threadId,
                        Thread = thread
                    };
                    await _threadUpvotedbSet.AddAsync(postUpVote);
                    await _context.SaveChangesAsync();

                    if (!user.Id.IsNullOrEmpty())
                        await SetPoint(user.Id, ContributionPointTypes.ThreadUpvote);
                    var response = new PostUpvoteResponseDto
                    {
                        PostDescription = thread.ThreadDescription,
                        PostTitle = thread.ThreadTitle,
                        UpvoteBy = user.FullName
                    };
                    return new ResponseDto<PostUpvoteResponseDto>(true, "Feed upvote raised succesfully.", response);
                }
                else
                {
                    return await RevokeUpvoteThread(checkIfUpVoteAlreadyExist, user.Id);
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ResponseDto<PostUpvoteResponseDto>> RevokeUpvoteThread(ThreadUpVote postUpVote, string userId)
        {
            try
            {
                if (postUpVote != null)
                {
                    _threadUpvotedbSet.Remove(postUpVote);
                    await _context.SaveChangesAsync();

                    if (userId.IsNullOrEmpty()) await SetPoint(userId, ContributionPointTypes.ThreadUpvote);

                    return new ResponseDto<PostUpvoteResponseDto>(true, "Upvote revoked successfully", new PostUpvoteResponseDto());
                }
                else
                    return new ResponseDto<PostUpvoteResponseDto>(false, "not found", new PostUpvoteResponseDto());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ResponseDto<PostThreadViewDto>> MarkThreadAsSolution(Guid threadId, string markedBy)
        {
            try
            {
                var postThread = await _dbSet.Include(x => x.Post).FirstOrDefaultAsync(x => x.Id == threadId);

                if (postThread == null) return new ResponseDto<PostThreadViewDto> { Message = "Invalid threadId.", Success = false, Data = new PostThreadViewDto() };

                if (postThread.Post.CreatedBy != markedBy) return new ResponseDto<PostThreadViewDto> { Message = "Only Post creater can set this flag.", Success = false, Data = new PostThreadViewDto() };

                postThread.IsSolution = postThread.IsSolution == false ? true : false;

                _dbSet.Attach(postThread);
                _context.Entry(postThread).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                if (postThread.IsSolution)
                    await SetPoint(postThread.CreatedBy, ContributionPointTypes.Resolver);
                else
                    await RevokePoint(postThread.CreatedBy, ContributionPointTypes.Resolver);

                var result = new PostThreadViewDto
                {
                    PostId = postThread.PostId,
                    ThreadTitle = postThread.ThreadTitle,
                    ThreadDescription = postThread.ThreadDescription,
                    ThreadId = postThread.Id,
                    Post = new PostViewDto
                    {
                        Description = postThread.Post.Description,
                        PostType = (Enums.PostType)postThread.Post.PostType,
                        Title = postThread.Post.Title,
                        Tags = postThread.Post.Tags.Select(t => new PostTagDto
                        {
                            PostId = t.PostId,
                            TagId = t.TagId,
                            TagDescription = t.Tag.TagDescription,
                            TagName = t.Tag.Name

                        }).ToList(),
                    }
                };

                return new ResponseDto<PostThreadViewDto> { Data = result, Message = "Marked succesfully", Success = true };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        private async Task ConfigureSyncEmail(ApplicationUser user, PostThreadViewDto dto)
        {
            var subject = "Notification";
            var htmlMessage = $"<html>\r\n<head>\r\n<title>New Comment Notification</title>\r\n<style>\r\n        body {{\r\n            font-family: Arial, sans-serif;\r\n            line-height: 1.6;\r\n        }}\r\n        .container {{\r\n            width: 100%;\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            border: 1px solid #ccc;\r\n            border-radius: 10px;\r\n            background-color: #f9f9f9;\r\n        }}\r\n        .header {{\r\n            font-size: 18px;\r\n            font-weight: bold;\r\n            margin-bottom: 10px;\r\n        }}\r\n        .content {{\r\n            font-size: 16px;\r\n            margin-bottom: 20px;\r\n        }}\r\n        .link {{\r\n            display: inline-block;\r\n            padding: 10px 20px;\r\n            font-size: 16px;\r\n            color: #fff;\r\n            background-color: #007bff;\r\n            text-decoration: none;\r\n            border-radius: 5px;\r\n        }}\r\n        .footer {{\r\n            font-size: 14px;\r\n            color: #555;\r\n            margin-top: 20px;\r\n        }}\r\n</style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n<div class=\"header\">Hey [User's Name], There's a New Comment on Your Thread!</div>\r\n<div class=\"content\">\r\n            Just a quick heads-up! Someone just added a new comment to the thread you started about [thread topic].\r\n</div>\r\n<div>\r\n<a href=\"[Link to the Thread]\" class=\"link\">Check it out here</a>\r\n</div>\r\n<div class=\"footer\">\r\n            If you have any questions or need help, just hit reply or reach out to our support team.<br>\r\n            Thanks for being part of our community!<br><br>\r\n            Cheers,<br>\r\n            [Your Name]<br>\r\n            [Your Position]<br>\r\n            [Your Contact Information]<br>\r\n            [Company/Website Name]\r\n</div>\r\n</div>\r\n</body>\r\n</html>";

            await _emailSender.SendEmailAsync(user.Email, subject, htmlMessage);
        }
        public async Task<bool> SendEmail(string creatorName, string feedlink, string feedCreatorname, string postTitle, string email)
        {
            try
            {
                var body = getEmailTemplateOnThreadCreate();
                var title = "There's a New Comment on Your Feed!";

                var replacements = new Dictionary<string, string>
                {
                    { "createName", creatorName },
                    { "feedLink", feedlink },
                    { "POST_TITLE", postTitle },
                    { "FeedCreatorName", feedCreatorname }
                };

                // Replace the placeholders
                string emailBody = ReplacePlaceholders(body, replacements);
                var sendEmailService = new SendEmailService();
                await sendEmailService.SendEmailAsync(email, title, emailBody);
                //await _emailSender.SendEmailAsync(email, title, emailBody);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
        public string ReplacePlaceholders(string template, Dictionary<string, string> replacements)
        {
            string pattern = @"\{\{(\w+)\}\}";
            return Regex.Replace(template, pattern, match =>
            {
                string placeholder = match.Groups[1].Value;
                return replacements.TryGetValue(placeholder, out string replacement) ? replacement : match.Value;
            });
        }
        private string getEmailTemplateOnThreadCreate()
        {
            return @"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            .card {
                                max-width: 600px;
                                margin: auto;
                                padding: 20px;
                                border: 1px solid #dcdcdc;
                                border-radius: 10px;
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                font-family: Arial, sans-serif;
                                background-color: #f9f9f9;
                            }
                            .card h2 {
                                color: #333;
                            }
                            .card p {
                                color: #666;
                                line-height: 1.6;
                            }
                            .card a {
                                display: inline-block;
                                margin-top: 20px;
                                padding: 10px 20px;
                                color: #fff;
                                background-color: #007bff;
                                text-decoration: none;
                                border-radius: 5px;
                            }
                            .card a:hover {
                                background-color: #0056b3;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='card'>
                            <h2>Hey {{createName}}, There's a New Comment on Your Feed!</h2>
                            <p>Just a quick heads-up! <b>{{FeedCreatorName}}</b>, just added a new comment to the thread you started about [{{POST_TITLE}}].</p> <br>If you have any questions or need help, just hit 'View Feed':<br>
                            <a href='{{feedLink}}'>View Feed</a>
                            <p> or reach out to our support team.<br>
                            Thanks for being part of our community!<br><br>
                            Cheers,<br></p>
                            <p>Best regards,<br>Ask.Javra (Admin),<br>ask.javra@notifications</p>
                        </div>
                    </body>
                    </html>";

        }
    }
}
