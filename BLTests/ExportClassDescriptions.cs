using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLTests
{
    [TestClass]
    public class ExportClassDescriptions
    {
        [TestMethod]
        public void ExportDescriptions()
        {
            const string resultDir = @"d:\CODE\bimlibrary\IFC4Classes";
            const string rootDir = @"c:\IFC4\schema";
            var resultFile = Path.Combine(resultDir, "IFC4Definitions.htm");
            const int offset = 4;

            if (!System.IO.Directory.Exists(rootDir))
            {
                throw new Exception();
            }
            using (var w = new IndentedTextWriter(File.CreateText(resultFile)) { Indent = 0 })
            {

                w.WriteLine("<!DOCTYPE html>");
                w.WriteLine("<head>");
                w.Indent += offset;

                w.WriteLine("<title>IFC4 classes and definitions</title>");
                w.WriteLine("<meta charset='UTF-8'>");

                w.Indent -= offset;
                w.WriteLine("</head>");
                w.WriteLine("<body>");
                w.Indent += offset;

                foreach (var dir in Directory.GetDirectories(rootDir, "*", SearchOption.TopDirectoryOnly))
                {
                    var subdir = System.IO.Path.Combine(dir, "lexical");
                    if (!System.IO.Directory.Exists(subdir)) continue;

                    w.Indent += offset;
                    foreach (var file in Directory.GetFiles(subdir, "*.htm", SearchOption.TopDirectoryOnly))
                    {

                        var data = System.IO.File.ReadAllText(file);

                        var title = (new Regex("(?<=<title>)(.*?)(?=</title>)", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Match(data).Groups[0].Value;

                        w.WriteLine("<h1>{0}</h1>", title);
                        w.WriteLine();

                        //Use regex to get definition part as from first paragraph to horizontal line divider
                        var exp = new Regex("<p(\n|.)*?(?=<hr)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        var description = exp.Match(data).Groups[0].Value;

                        //remove Figure xxx - ...
                        description = (new Regex("Figure\\s*[0-9]+.*", RegexOptions.IgnoreCase)).Replace(
                            description, "");

                        //remove HISTORY and CHANGE
                        description = (new Regex("<blockquote class=\"history\">(.|\n)*?</blockquote>", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Replace(description, "");
                        description = (new Regex("<blockquote class=\"change(.|\n)*?(</blockquote>|</bloclquote>)", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Replace(description, "");

                        //remove links, change them to <strong></strong>
                        description = (new Regex("<a.*?>", RegexOptions.IgnoreCase)).Replace(description, "<strong>");
                        description = (new Regex("</a>", RegexOptions.IgnoreCase)).Replace(description, "</strong>");

                        w.Write(description);
                    }
                    w.Indent -= offset;
                }

                w.Indent -= offset;
                w.WriteLine("</body>");
                w.Close();
            }


        }
    }
}
