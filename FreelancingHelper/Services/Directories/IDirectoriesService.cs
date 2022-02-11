namespace FreelancingHelper.Services.Directories
{
    public interface IDirectoriesService
    {
        string HistoryFilesDir { get; }
        string AppConfigurationsDir { get; }
    }
}