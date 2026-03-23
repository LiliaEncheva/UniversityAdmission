namespace UniversityAdmission.Services.Interfaces
{
    public interface IAdmissionService
    {
        Task RunAdmissionAsync();
        Task PublishResultsAsync();
    }
}