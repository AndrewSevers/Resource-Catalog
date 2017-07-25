namespace Extensions {

    public interface IFile {
        string FilePath { get; }
        string FileName { get; }
        bool FormatAsJson { get; }
    }

}
