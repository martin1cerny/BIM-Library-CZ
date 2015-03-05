using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLSpec.Plugins
{
    internal class LoadIFC4ClassDescriptions
#if DEBUG
        : IExternalCommand
#endif
    {
        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            var origLang = model.Information.Lang;
            using (var txn = model.BeginTansaction("Classification item definitions"))
            {
                try
                {
                    model.Information.Lang = "en-US";

                    //get directory
                    var dlg = new System.Windows.Forms.FolderBrowserDialog
                    {
                        Description = @"Vyberte složku s dokumentací IFC4",
                        ShowNewFolderButton = false
                    };
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        txn.RollBack();
                        return;
                    }

                    var rootDir = System.IO.Path.Combine(dlg.SelectedPath, "schema");
                    if (!System.IO.Directory.Exists(rootDir))
                    {
                        ui.ShowMessage("Chyba","Tato složka neobsahuje dokumentaci IFC4.");
                        txn.RollBack();
                        return;
                    }
                    foreach (
                        var dir in
                            System.IO.Directory.GetDirectories(rootDir, "*", System.IO.SearchOption.TopDirectoryOnly))
                    {
                        var subdir = System.IO.Path.Combine(dir, "lexical");
                        if (!System.IO.Directory.Exists(subdir)) continue;

                        foreach (
                            var file in
                                System.IO.Directory.GetFiles(subdir, "*.htm", System.IO.SearchOption.TopDirectoryOnly))
                        {
                            //find classification item
                            var clsItem =
                                model.Get<BLData.Classification.BLClassificationItem>(
                                    ci => ci.Name.ToLower() == System.IO.Path.GetFileNameWithoutExtension(file))
                                    .FirstOrDefault();
                            if (clsItem == null) continue;


                            var data = System.IO.File.ReadAllText(file);

                            //remove all end lines
                            data = data.Replace("\r", " ").Replace("\n", " ");
                            data = (new Regex("\\s{2,50}", RegexOptions.IgnoreCase)).Replace(data, " ");


                            //Use regex to get definition part as from first paragraph to horizontal line divider
                            var exp = new Regex("<p.*?(?=<hr)", RegexOptions.IgnoreCase);
                            var description = exp.Match(data).Groups[0].Value;

                            //Insert new lines instead of block HTML elements (<ol.*?>|<ul.*?>)
                            description = (new Regex(
                                "(<p.*?>|<div.*?>|<blockquote.*?>|<h.*?>|<dd.*?>|<dt.*?>|<hr.*?>|<pre.*?>)",
                                RegexOptions.IgnoreCase))
                                .Replace(description, "\n\n");

                            //keep lists
                            var listExpr = new Regex("<li.*?>", RegexOptions.IgnoreCase);
                            description = listExpr.Replace(description, "\n• ");

                            //replace HTML tags to create simple pure text
                            var tagExp = new Regex("(<.*?>)", RegexOptions.IgnoreCase);
                            description = tagExp.Replace(description, "");

                            //remove Figure xxx - ...
                            description = (new Regex("Figure\\s*[0-9]+.*", RegexOptions.IgnoreCase)).Replace(
                                description, "");

                            //remove HISTORY 
                            description = (new Regex("HISTORY.*", RegexOptions.IgnoreCase)).Replace(description, "");
                            description = (new Regex("IFC.*?CHANGE.*")).Replace(description, "");

                            //replace HTML entities to create simple pure text
                            description = description
                                .Replace("&nbsp;", " ")
                                .Replace("&mdash;", "-")
                                .Replace("&gt;", ">")
                                .Replace("&lt;", "<")
                                .Replace("&lsquo;", "\"")
                                .Replace("&rsquo;", "\"")
                                .Replace("&deg;", "°")
                                .Replace("&pi;", "π")
                                .Replace("&le;", "≤")
                                .Replace("&ge;", "≥")
                                .Replace("&infin;", "∞")
                                .Replace("&times;", "×")
                                .Replace("&minus;", "−")
                                .Replace("&auml;", "ä")
                                .Replace("&ndash;", "–")
                                .Replace("&Xi;", "Ξ");

                            //replace any unexpected entities with blank space
                            var entExp = new Regex("&[a-z]{1,5};", RegexOptions.IgnoreCase);
                            description = entExp.Replace(description, " ");

                            //reduce empty space
                            description = (new Regex("(\\s*\\n){3,10}", RegexOptions.IgnoreCase)).Replace(description,
                                "\n\n");

                            clsItem.LocalizedDefinition = description.Trim();
                        }
                    }
                    model.Information.Lang = origLang;
                    txn.Commit();
                }
                catch (Exception)
                {
                    txn.RollBack();
                    throw;
                }
            }
        }

        public string Name
        {
            get { return "Load IFC4 class descriptions"; }
        }

        private Guid _id = new Guid("8F8D6C3B-6203-43B4-9045-95D927417B90");

        public Guid ID
        {
            get { return _id; }
        }
    }
}