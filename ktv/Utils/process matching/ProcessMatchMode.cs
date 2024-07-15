namespace d9.ktv;
public enum ProcessMatchMode
{
    [ImplementedBy(nameof(ProcessMatchModeImplementation.IsInFolder))]
    InFolder,
    [ImplementedBy(nameof(ProcessMatchModeImplementation.FileNameMatches))]
    FileNameMatches,
    [ImplementedBy(nameof(ProcessMatchModeImplementation.MainWindowTitleMatches))]
    MainWindowTitleMatches,
    [ImplementedBy(nameof(ProcessMatchModeImplementation.ProcessNameMatches))]
    ProcessNameMatches,
    [ImplementedBy(nameof(ProcessMatchModeImplementation.IsInCategory))]
    IsInCategory
}