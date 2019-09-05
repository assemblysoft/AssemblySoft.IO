
using System.IO;
using System.Threading.Tasks;

namespace AssemblySoft.IO
{
    /// <summary>
    /// Client for file search file tasks
    /// </summary>
    public partial class FileClient
    {
 private string CheckModifiedFilesRecursive(string rootFolder, DateTime sinceDateTime)
        {
            Console.WriteLine("  Checking " + rootFolder);

            var result = new StringBuilder();

            try
            {
                foreach (string d in Directory.GetDirectories(rootFolder))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        var lastModified = File.GetLastWriteTime(f);

                        if (lastModified > sinceDateTime)
                        {
                            result.AppendFormat(@"<tr><td>{0:dd/MM/yyyy HH:mm:ss}</td><td>{1}</td></tr>",
                                lastModified, f).AppendLine(); ;
                        }
                    }

                    var subFolderResult = RecursivelyCheckModifiedFiles(d, sinceDateTime);

                    if (!string.IsNullOrWhiteSpace(subFolderResult))
                    {
                        result.Append(subFolderResult);
                    }
                }
            }
            catch (Exception)
            {
            }

            return result.ToString();
        }

        public string SearchModifiedFiles(string rootFolder, int sinceDays)
        {
            var result = new StringBuilder();

            result.AppendFormat(@"<h1>Files modified since {0} days under {1} at {2:dd/MM/yyyy HH:mm:ss}</h1>",
                sinceDays, rootFolder, DateTime.Now).AppendLine();

            var sinceDateTime = DateTime.Today.AddDays(-Math.Abs(sinceDays));
            var subFolderResult = CheckModifiedFilesRecursive(rootFolder, sinceDateTime);

            if (!string.IsNullOrWhiteSpace(subFolderResult))
            {
                result.AppendLine(@"<table>");
                result.AppendLine(subFolderResult);
                result.AppendLine(@"</table>");
            }
            else
            {
                result.AppendLine("No files found");
            }

            return result.ToString();
        }

        public string SearchModifiedFiles(string rootFolder, DateTime sinceDateTime)
        {
            var result = new StringBuilder();

            result.AppendFormat(@"<h1>Files modified since {0:d} under {1} at {2:dd/MM/yyyy HH:mm:ss}</h1>",
                sinceDateTime, rootFolder, DateTime.Now).AppendLine();

            var subFolderResult = CheckModifiedFilesRecursive(rootFolder, sinceDateTime);

            if (!string.IsNullOrWhiteSpace(subFolderResult))
            {
                result.AppendLine(@"<table>");
                result.AppendLine(subFolderResult);
                result.AppendLine(@"</table>");
            }
            else
            {
                result.AppendLine("No files found");
            }

            return result.ToString();
        }

        public string SearchForFilesContainingPhrases(string rootFolder, string phraseSearchStrings, 
            bool ignoreCase, params string[] fileWildcards)
        {
            var result = new StringBuilder();

            result.AppendFormat(@"<h1>Files containing phrase from ""{0}"" under {1} at {2:dd/MM/yyyy HH:mm:ss}</h1>",
                phraseSearchStrings, rootFolder, DateTime.Now).AppendLine();

            var phrases = phraseSearchStrings.Split(',');

            var searched = false;

            foreach (var phrase in phrases)
            {
                if (!string.IsNullOrWhiteSpace(phrase))
                {
                    Console.WriteLine("  Checking " + phrase);

                    if (fileWildcards.Length > 0)
                    {
                        var fileList = fileWildcards.Length > 0
                            ? FileSystem.FindInFiles(rootFolder, phrase, ignoreCase,
                                Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories, fileWildcards)
                            : FileSystem.FindInFiles(rootFolder, phrase, ignoreCase,
                                Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories);

                        searched = true;

                        result.AppendFormat(@"<h2>Phrase = {0}</h2>", phrase).AppendLine();

                        if (fileList.Count > 0)
                        {
                            foreach (var fileName in fileList)
                            {
                                result.AppendFormat(@"<p>{0}</p>", fileName).AppendLine();
                            }
                        }
                        else
                        {
                            result.AppendLine(@"<p>No matches found</p>");
                        }
                    }
                }
            }

            if (!searched)
            {
                result.AppendLine(@"<p>No phrases found to search for</p>");
            }

            return result.ToString();
        }
        }
        }
