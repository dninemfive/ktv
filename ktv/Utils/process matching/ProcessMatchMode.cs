namespace d9.ktv;
public enum ProcessMatchMode
{
    [ImplementedBy(nameof(ProcessMatchUtils.IsInFolder))]
    InFolder,
    [ImplementedBy(nameof(ProcessMatchUtils.FileNameMatches))]
    FileNameMatches,
    [ImplementedBy(nameof(ProcessMatchUtils.MainWindowTitleMatches))]
    MainWindowTitleMatches,
    [ImplementedBy(nameof(ProcessMatchUtils.ProcessNameMatches))]
    ProcessNameMatches,
    [ImplementedBy(nameof(ProcessMatchUtils.IsInCategory))]
    IsInCategory
}