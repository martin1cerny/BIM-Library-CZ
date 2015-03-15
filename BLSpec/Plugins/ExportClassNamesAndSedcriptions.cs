using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSpec.Plugins
{
    class ExportClassNamesAndSedcriptions: IExternalCommand
    {
        private const int offset = 4;

        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var fileName = System.IO.Path.Combine(dir, "Classes_and_descriptions.html");

            using (var w = new IndentedTextWriter(System.IO.File.CreateText(fileName)) { Indent = 0})
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

                var items = model.Get<BLData.Classification.BLClassificationItem>().OrderBy(c => c.LocalizedName);

                foreach (var item in items)
                {
                    w.WriteLine("<h1>{0}</h1>", item.LocalizedName);
                    w.WriteLine("<div>{0}</div>", (item.LocalizedDefinition ?? "").Replace("\n", "<br />"));

                }

                w.Indent -= offset;
                w.WriteLine("</body>");
                w.Close();
            }
            ui.ShowMessage("Done", "File created: " + fileName);
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
