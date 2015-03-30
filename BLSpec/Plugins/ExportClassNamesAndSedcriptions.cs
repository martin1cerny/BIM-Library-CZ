﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLSpec.Plugins
{
    class ExportClassNamesAndSedcriptions: IExternalCommand
    {
        private const int offset = 4;

        public void Execute(BLData.BLModel model, UIHelper ui)
        {

            var items = model.Get<BLData.Classification.BLClassificationItem>().OrderBy(c => c.LocalizedName);

            const string rootDir = @"c:\IFC4\schema";
            var resultDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
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

                        //get only names and descriptions for selected relevant classes
                        var clsName = Path.GetFileNameWithoutExtension(file);
                        if (!items.Any(i => i.Name.ToLower() == clsName.ToLower())) continue;

                        var data = System.IO.File.ReadAllText(file);

                        var title = (new Regex("(?<=<title>)(.*?)(?=</title>)", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Match(data).Groups[0].Value;
                        var prettyTitle = (new Regex("([a-z])([A-Z](?=[a-z]))")).Replace(title.Substring(3), "$1 $2");

                        w.WriteLine("<h1 id='{0}'>{1}</h1>", title, prettyTitle);
                        w.WriteLine();

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
                            if (!Directory.Exists(imgDir))
                                Directory.CreateDirectory(imgDir);
                            var actDir = Path.GetDirectoryName(file);

                            foreach (Match src in srcs)
                            {
                                var capture = src.Captures[0];
                                var imgPath = Path.Combine(actDir, capture.Value);
                                var imgName = Path.GetFileName(imgPath);
                                var newSrc = baseResultName + "/" +  imgName;
                                var newImgPath = Path.Combine(imgDir, imgName);
                                File.Copy(imgPath, newImgPath, true);
                                description = description.Replace(capture.Value, newSrc);
                            }

                        }

                        w.Write(description);
                    }
                    w.Indent -= offset;
                }

                w.Indent -= offset;
                w.WriteLine("</body>");
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
