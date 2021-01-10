namespace System.IO
{
    public partial class FileDirectoryProcessing
    {
        public class ResultInfo
        {
            internal int filesProcessed;
            internal int directoriesProcessed;

            public int FilesProcessed
            {
                get { return filesProcessed; }
            }

            public int DirectoriesProcessed
            {
                get { return directoriesProcessed; }
            }

            public override string ToString()
            {
                return "Total Files Processed : " + filesProcessed
                       + "\nTotal Directories Processed : " + directoriesProcessed;
            }

            internal ResultInfo(int directoriesProcessed, int filesProcessed)
            {
                this.filesProcessed = filesProcessed;
                this.directoriesProcessed = directoriesProcessed;
            }
        }
    }
}
