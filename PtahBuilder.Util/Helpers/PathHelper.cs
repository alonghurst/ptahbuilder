using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtahBuilder.Util.Helpers
{
    public static class PathHelper
    {
        public static string GetRootPath(string lookingForDirectory, params string[] args)
        {
            string root = string.Empty;

            if (args.Length > 0)
            {
                root = args[0];
            }

            var path = string.IsNullOrWhiteSpace(root) ? ".." : root;

            while (Directory.GetDirectories(path).All(x => !x.EndsWith(lookingForDirectory)))
            {
                path = Path.GetFullPath(Path.Combine(path, "../"));

                Console.WriteLine($"Trying {path}");
            }

            root = Path.GetFullPath(path);

            return root;
        }
    }
}
