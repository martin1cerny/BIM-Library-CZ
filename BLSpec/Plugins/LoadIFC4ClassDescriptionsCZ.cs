using BLData.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLSpec.Plugins
{
    internal class LoadIFC4ClassDescriptionsCZ
#if DEBUG
 : IExternalCommand
#endif
    {
        protected string Lang = "en-US";

        public LoadIFC4ClassDescriptionsCZ()
        {
            Lang = "cs-CZ";
        }

        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            var origLang = model.Information.Lang;
            using (var txn = model.BeginTansaction("Classification item definitions"))
            {
                try
                {
                    model.Information.Lang = Lang;

                    //get directory
                    var dlg = new System.Windows.Forms.FolderBrowserDialog
                    {
                        Description = @"Vyberte složku s dokumentací IFC4 v češtině",
                        ShowNewFolderButton = false
                    };
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        txn.RollBack();
                        return;
                    }

                    var rootDir = dlg.SelectedPath;
                    if (!System.IO.Directory.Exists(rootDir))
                    {
                        ui.ShowMessage("Chyba", "Tato složka neexistuje.");
                        txn.RollBack();
                        return;
                    }
                    foreach (
                        var file in
                            System.IO.Directory.GetFiles(rootDir, "*.htm", System.IO.SearchOption.TopDirectoryOnly))
                    {
                        //find classification item
                        var clsName = System.IO.Path.GetFileNameWithoutExtension(file);
                        var clsItem =
                            model.Get<BLData.Classification.BLClassificationItem>(
                                ci => ci.Name.ToLower() == clsName)
                                .FirstOrDefault();
                        if (clsItem == null) continue;


                        var data = System.IO.File.ReadAllText(file);

                        //remove all end lines
                        data = data.Replace("\r", " ").Replace("\n", " ");
                        data = (new Regex("\\s{2,50}", RegexOptions.IgnoreCase)).Replace(data, " ");

                        var ptExp = new Regex("</h3>.*?<ul>(?<data>.*?)</ul>", RegexOptions.IgnoreCase);
                        var predefinedTypes = ptExp.Match(data).Groups["data"];
                        if (predefinedTypes != null && predefinedTypes.Length != 0)
                        {
                            ProcessPredefinedTypes(predefinedTypes.Value, clsItem);
                        }

                        var nameExp = new Regex("<h2.*?>(?<data>.*?)</h2>", RegexOptions.IgnoreCase);
                        var name = nameExp.Match(data).Groups["data"].Value;
                        //replace HTML entities to create simple pure text
                        name = System.Web.HttpUtility.HtmlDecode(name);


                        //Use regex to get definition part as from first paragraph to horizontal line divider
                        var exp = new Regex("</h2>(?<data>.*?)</body>", RegexOptions.IgnoreCase);
                        var description = exp.Match(data).Groups["data"].Value;

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
                        description = System.Web.HttpUtility.HtmlDecode(description);

                        //reduce empty space
                        description = (new Regex("(\\s*\\n){3,10}", RegexOptions.IgnoreCase)).Replace(description,
                            "\n\n");

                        clsItem.LocalizedDefinition = description.Trim();
                        clsItem.LocalizedName = name.Trim();
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

        private void ProcessPredefinedTypes(string data, BLClassificationItem parent)
        {
            var exp = new Regex("<li.*?>(?<data>.*?)</li>", RegexOptions.IgnoreCase);
            var nameMatches = exp.Matches(data);
            var names = new List<string>();
            foreach (Match m in nameMatches)
            {
                var name = m.Groups["data"].Value;
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                name = System.Web.HttpUtility.HtmlDecode(name);
                names.Add(name);
            }

            var predefItems = parent.Children.Where(c => !c.Name.StartsWith("Ifc")).ToList();
            if (predefItems.Count != names.Count)
                throw new Exception("Wrong count of predefined type names");

            for (var i = 0; i < names.Count; i++)
                predefItems[i].LocalizedName = names[i];
        }


        public virtual string Name
        {
            get { return "Load IFC4 Czech class descriptions"; }
        }

        private Guid _id = new Guid("893D318A-3962-4BBC-8740-AC02B3F203A1");

        public virtual Guid ID
        {
            get { return _id; }
        }
    }
}