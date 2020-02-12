using System.IO;

namespace PtahBuilder.BuildSystem.FileManagement
{
    public class FileTidier : DirectoryParser
    {
        protected override string FileFilter => "*.yaml";

        protected override void ParseFile(string file)
        {
            var text = File.ReadAllLines(file);

            var changed = false;

            for (int i = 0; i < text.Length; i++)
            {
                var newLine = text[i].TrimEnd();
                if (newLine != text[i])
                {
                    text[i] = newLine;
                    changed = true;
                }
            }

            if (changed)
            {
                File.WriteAllLines(file, text);
            }
        }
    }
}
