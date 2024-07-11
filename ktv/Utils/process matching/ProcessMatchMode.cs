namespace d9.ktv;
public enum ProcessMatchMode
{
    [ImplementationMethod(nameof(ProcessMatchUtils.IsInFolder))]
    InFolder,
    [ImplementationMethod(nameof(ProcessMatchUtils.FileNameMatches))]
    FileNameMatches,
    [ImplementationMethod(nameof(ProcessMatchUtils.MainWindowTitleMatches))]
    MainWindowTitleMatches,
    [ImplementationMethod(nameof(ProcessMatchUtils.ProcessNameMatches))]
    ProcessNameMatches,
    [ImplementationMethod(nameof(ProcessMatchUtils.IsInCategory))]
    IsInCategory
}