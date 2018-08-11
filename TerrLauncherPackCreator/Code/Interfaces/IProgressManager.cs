namespace TerrLauncherPackCreator.Code.Interfaces
{
    public interface IProgressManager
    {
        string Text { get; set; }

        int CurrentStep { get; set; }

        int MaximumStep { get; set; }

        int CurrentProcessedFile { get; set; }

        int TotalFilesToProcess { get; set; }

        void SetNormalState();

        void SetIndeterminateState();
    }
}