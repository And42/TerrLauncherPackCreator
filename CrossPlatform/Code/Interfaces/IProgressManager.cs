namespace CrossPlatform.Code.Interfaces;

public interface IProgressManager
{
    string Text { get; set; }

    int CurrentProgress { get; set; }

    int MaximumProgress { get; set; }

    int RemainingFilesCount { get; set; }

    bool IsIndeterminate { get; set; }
}