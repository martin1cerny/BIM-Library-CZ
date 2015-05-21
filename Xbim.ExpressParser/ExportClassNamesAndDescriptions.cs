using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLSpec;

namespace Xbim.ExpressParser
{
    class ExportClassNamesAndDescriptions
#if DEBUG
        : IExternalCommand
#endif
    {
        private const int offset = 4;

        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            Parser parser = null;
            using (var file = new MemoryStream(Schemas.SchemasDefinitions.IFC4))
            {
                var scanner = new Scanner(file);
                parser = new Parser(scanner, IfcVersionEnum.IFC4);
                var result = parser.Parse();

                if (parser.Output != null)
                    parser.Output.Close();
                file.Close();


                if (scanner.Errors.Any())
                {
                    if (!result)
                    {
                        Debug.WriteLine("Errors occured during the processing. Output might be incomplete or eroneous.");
                        foreach (var err in scanner.Errors)
                        {
                            Debug.WriteLine(err);
                        }
                    }
                    else
                        Debug.WriteLine("Errors occured during the processing but all of them had been catched.");
                }
                else
                    Debug.WriteLine("Completed with no errors.");
            }

            //items are used to filter ot the content
            var items = model.Get<BLData.Classification.BLClassificationItem>().OrderBy(c => c.LocalizedName);

            const string rootDir = @"c:\IFC4\schema";
            var resultDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var resultFile = Path.Combine(resultDir, "IFC4Definitions.htm");
            var resultSingleFilesDir = Path.Combine(resultDir, "IFC4DefsSingleFiles");
            if (!Directory.Exists(resultSingleFilesDir))
                Directory.CreateDirectory(resultSingleFilesDir);

            const int offset = 4;

            if (!System.IO.Directory.Exists(rootDir))
            {
                throw new Exception();
            }
            using (var w = new IndentedTextWriter(File.CreateText(resultFile)) { Indent = 0 })
            {

                w.WriteLine("<!DOCTYPE html>");
                w.WriteLine("<html>");
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
                    w.Indent += offset;
                    var subdir = System.IO.Path.Combine(dir, "lexical");
                    if (!System.IO.Directory.Exists(subdir)) continue;

                    var domain = parser.Schema.Domains.FirstOrDefault(d => d.Name.ToLower() == Path.GetFileNameWithoutExtension(dir).ToLower().Substring(3));
                    if (domain == null) throw new Exception("Domain not found: " + dir);
                    var domainName = (new Regex("([a-z])([A-Z](?=[a-z]))")).Replace(domain.Name, "$1 $2");
                    var domainWritten = false;

                    w.Indent += offset;
                    foreach (var file in Directory.GetFiles(subdir, "*.htm", SearchOption.TopDirectoryOnly))
                    {

                        //get only names and descriptions for selected relevant classes
                        var clsName = Path.GetFileNameWithoutExtension(file);
                        if (!items.Any(i => i.Name.ToLower() == clsName.ToLower())) continue;

                        using (var single = File.CreateText(Path.Combine(resultSingleFilesDir, clsName + ".htm")))
                        {
                            //single file header
                            single.WriteLine("<!DOCTYPE html>");
                            single.WriteLine("<html>");
                            single.WriteLine("<head>");
                            single.WriteLine("<title>{0}</title>", clsName);
                            single.WriteLine("<meta charset='UTF-8'>");
                            single.WriteLine("</head>");
                            single.WriteLine("<body>");



                            var node = parser.Tree.FirstOrDefault(n => n.Name.ToLower() == Path.GetFileNameWithoutExtension(file).ToLower());
                            if (node == null) throw new Exception("Node not found for " + file);
                            if (!domainWritten)
                            {
                                w.WriteLine(String.Format("<h1>{0}</h1>", domainName));
                                domainWritten = true;
                            }

                            var data = System.IO.File.ReadAllText(file);

                            var title = (new Regex("(?<=<title>)(.*?)(?=</title>)", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Match(data).Groups[0].Value;
                            var prettyTitle = (new Regex("([a-z])([A-Z](?=[a-z]))")).Replace(title.Substring(3), "$1 $2");

                            w.WriteLine("<h2 id='{0}'>{1}</h2>", title, prettyTitle);
                            w.WriteLine();
                            single.WriteLine("<h2 id='{0}'>{1}</h2>", title, prettyTitle);
                            single.WriteLine("<p><strong>Domain: {0}</strong></p>", domainName);
                            single.WriteLine();

                            //Use regex to get definition part as from first paragraph to horizontal line divider
                            var exp = new Regex("<p(\n|.)*?(?=<hr)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            var description = exp.Match(data).Groups[0].Value;

                            //remove Figure xxx - ...
                            description = (new Regex("Figure\\s*[0-9]+.*", RegexOptions.IgnoreCase)).Replace(
                                description, "");

                            //remove HISTORY, NOTE and CHANGE
                            description = (new Regex("<blockquote class=\"history\">(.|\n)*?</blockquote>", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Replace(description, "");
                            description = (new Regex("<blockquote class=\"note\">(.|\n)*?</blockquote>", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Replace(description, "");
                            description = (new Regex("<blockquote class=\"change(.|\n)*?(</blockquote>|</bloclquote>)", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Replace(description, "");

                            //remove links, change them to <strong></strong>
                            description = (new Regex("<a\\s.*?>", RegexOptions.IgnoreCase)).Replace(description, "<strong>");
                            description = (new Regex("</a>", RegexOptions.IgnoreCase)).Replace(description, "</strong>");

                            //copy and fix images
                            var srcs = new Regex("(?<=src=[\r|\n]*\")(.*?)(?=\")", RegexOptions.IgnoreCase | RegexOptions.Multiline).Matches(description);
                            if (srcs.Count > 0)
                            {
                                var baseResultName = Path.GetFileNameWithoutExtension(resultFile);
                                var imgDir = Path.Combine(resultDir, baseResultName);
                                if (!Directory.Exists(imgDir)) Directory.CreateDirectory(imgDir);
                                var imgDirSingle = Path.Combine(resultSingleFilesDir, baseResultName);
                                if (!Directory.Exists(imgDirSingle)) Directory.CreateDirectory(imgDirSingle);

                                var actDir = Path.GetDirectoryName(file);

                                foreach (Match src in srcs)
                                {
                                    var capture = src.Captures[0];
                                    var imgPath = Path.Combine(actDir, capture.Value);
                                    var imgName = Path.GetFileName(imgPath);
                                    var newSrc = baseResultName + "/" + imgName;
                                    var newImgPath = Path.Combine(imgDir, imgName);
                                    var newImgPathSingle = Path.Combine(imgDirSingle, imgName);
                                    File.Copy(imgPath, newImgPath, true);
                                    File.Copy(imgPath, newImgPathSingle, true);
                                    description = description.Replace(capture.Value, newSrc);
                                }

                            }

                            w.Write(description);
                            single.WriteLine(description);

                            //write out any predefined types if defined
                            if (node.PredefinedTypes != null && node.PredefinedTypes.Any())
                            {
                                w.WriteLine("<h3>Predefined types</h3>");
                                single.WriteLine("<h3>Predefined types</h3>");
                                w.WriteLine("<ul>");
                                single.WriteLine("<ul>");
                                w.Indent += offset;
                                foreach (var pt in node.PredefinedTypes)
                                {
                                    w.WriteLine(string.Format("<li>{0}</li>", pt));
                                    single.WriteLine(string.Format("<li>{0}</li>", pt));
                                }
                                w.Indent -= offset;
                                w.WriteLine("</ul>");
                                single.WriteLine("</ul>");
                            }

                            //finish single file
                            single.WriteLine("</body> \n </html>");
                            single.Close();
                        }
                    }
                    w.Indent -= offset;
                    w.Indent -= offset;
                }

                w.Indent -= offset;
                w.WriteLine("</body>");
                w.WriteLine("</html>");
                w.Close();
            }
            
            ui.ShowMessage("Done", "File created: " + resultFile);
        }

        public string Name
        {
            get {return "Export class names and descriptions"; }
        }

        public Guid ID
        {
            get { return new Guid("EC95042C-5F14-45AF-B03E-31666042D27D"); }
        }
    }
}
