using FluentResults;
using RecruitmentSystemApplication.Common.Interfaces;
using RecruitmentSystemApplication.Services.CandidateProjects.Create;
using RecruitmentSystemApplication.Services.CandidateProjects.Get;
using RecruitmentSystemApplication.Services.CandidateProjects.Modify;
using RecruitmentSystemApplication.Services.CandidateProjects.Tags;
using RecruitmentSystemDomain.Models;

namespace RecruitmentSystemApplication.Services.CandidateProjects
{
    public class CandidateProjectService(
        ICandidateProjectRepository candidateProjectRepository,
        IUserRepository userRepository,
        ITagRepository tagRepository,
        IBaseRepository baseRepository) : ICandidateProjectService
    {
        public async Task<Result<Guid>> Create(Guid userId, CandidateProjectCreateDTO candidateProjectCreateDTO)
        {
            var userProfile = await userRepository.FindUserProfile(userId);
            if (userProfile is null)
                return Result.Fail<Guid>(new Error("Candidate profile was not found").WithMetadata("StatusCode", 404));

            var candidateProject = new CandidateProject
            {
                Id = Guid.NewGuid(),
                Name = candidateProjectCreateDTO.Name,
                StartDate = candidateProjectCreateDTO.StartDate,
                EndDate = candidateProjectCreateDTO.EndDate,
                Description = candidateProjectCreateDTO.Description,
                UserProfile = userProfile,
                UserProfileId = userProfile.Id,
                Tags = []
            };

            await baseRepository.AddItem(candidateProject);
            await baseRepository.SaveChanges();
            return Result.Ok(candidateProject.Id);
        }

        public async Task<Result<List<CandidateProjectDTO>>> GetCandidateProjects(string? prefix)
        {
            var candidateProjects = await candidateProjectRepository.GetCandidateProjects(prefix);
            var result = candidateProjects.Select(project => new CandidateProjectDTO(
                project.Id,
                project.UserProfile.UserId,
                project.Name,
                project.StartDate,
                project.EndDate,
                project.Description,
                project.Tags)).ToList();

            return Result.Ok(result);
        }

        public async Task<Result> Modify(Guid userId, bool isAdmin, CandidateProjectModifyDTO candidateProjectModifyDTO)
        {
            var projectResult = await GetProjectForModification(candidateProjectModifyDTO.Id, userId, isAdmin);
            if (projectResult.IsFailed)
                return projectResult.ToResult();

            var candidateProject = projectResult.Value;
            candidateProject.Name = candidateProjectModifyDTO.Name;
            candidateProject.StartDate = candidateProjectModifyDTO.StartDate;
            candidateProject.EndDate = candidateProjectModifyDTO.EndDate;
            candidateProject.Description = candidateProjectModifyDTO.Description;

            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> Delete(Guid userId, bool isAdmin, Guid candidateProjectId)
        {
            var projectResult = await GetProjectForModification(candidateProjectId, userId, isAdmin);
            if (projectResult.IsFailed)
                return projectResult.ToResult();

            await baseRepository.DeleteItems<CandidateProject>(new List<Guid> { candidateProjectId });
            return Result.Ok();
        }

        public async Task<Result> AddTag(Guid userId, bool isAdmin, CandidateProjectTagDTO candidateProjectTagDTO)
        {
            var projectResult = await GetProjectForModification(candidateProjectTagDTO.CandidateProjectId, userId, isAdmin);
            if (projectResult.IsFailed)
                return projectResult.ToResult();

            var tag = await tagRepository.GetTag(candidateProjectTagDTO.TagId);
            if (tag is null)
                return Result.Fail(new Error($"Tag with id - {candidateProjectTagDTO.TagId} was not found").WithMetadata("StatusCode", 404));

            var candidateProject = projectResult.Value;
            if (candidateProject.Tags.Any(existingTag => existingTag.Id == tag.Id))
                return Result.Fail("Tag is already linked to this candidate project");

            candidateProject.Tags.Add(tag);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        public async Task<Result> DeleteTag(Guid userId, bool isAdmin, CandidateProjectTagDTO candidateProjectTagDTO)
        {
            var projectResult = await GetProjectForModification(candidateProjectTagDTO.CandidateProjectId, userId, isAdmin);
            if (projectResult.IsFailed)
                return projectResult.ToResult();

            var candidateProject = projectResult.Value;
            var tag = candidateProject.Tags.FirstOrDefault(existingTag => existingTag.Id == candidateProjectTagDTO.TagId);
            if (tag is null)
                return Result.Fail(new Error("Tag is not linked to this candidate project").WithMetadata("StatusCode", 404));

            candidateProject.Tags.Remove(tag);
            await baseRepository.SaveChanges();
            return Result.Ok();
        }

        private async Task<Result<CandidateProject>> GetProjectForModification(Guid candidateProjectId, Guid userId, bool isAdmin)
        {
            var candidateProject = await candidateProjectRepository.GetCandidateProject(candidateProjectId);
            if (candidateProject is null)
                return Result.Fail<CandidateProject>(new Error($"Candidate project with id - {candidateProjectId} was not found").WithMetadata("StatusCode", 404));

            if (!isAdmin && candidateProject.UserProfile.UserId != userId)
                return Result.Fail<CandidateProject>(new Error("You do not have permission to modify this candidate project").WithMetadata("StatusCode", 403));

            return Result.Ok(candidateProject);
        }
    }
}
