using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xbim.Ifc2x3.ExternalReferenceResource;

namespace BimLibrary
{
    public class ClassificationImporter
    {
        public void ImportCSV(string file)
        {
            if (!File.Exists(file))
                throw new ArgumentException("File '"+file+"' doesn't exist.");

            var lines = File.ReadAllLines(file);
            
            //nothing to do realy
            if (lines.Length <= 1)
                return;

            var header = ParseLine( lines[0]);
            //check format
            if (header.Length < 3)
                throw new Exception("Unexpected format of CSV. Header information is missing.");
            if (header[0] != "Code" || header[1] != "Description" || header[2] != "Parent")
                throw new Exception("Wrong header format of CSV file. See examples in the install directory.");

            var name = Path.GetFileNameWithoutExtension(file);
            var model = App.Library.Model;
            var classification = model.Instances.Where<IfcClassification>(c => c.Name == name).FirstOrDefault();
            if (classification != null)
                throw new Exception("Classification '"+name+"' exists already. There can be only one classification with the same name.");


            var creation = new Action(() => {
                classification = model.Instances.New<IfcClassification>(c => c.Name = name);
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = ParseLine(lines[i]);
                    var code = fields[0];
                    var description = fields[1];
                    var parent = fields.Length >= 3 ? fields[2] : null;

                    var item = model.Instances.Where<IfcClassificationItem>(c => 
                        c.ItemOf == classification && 
                        c.Notation != null && 
                        c.Notation.NotationValue == code
                        ).FirstOrDefault();
                    if (item == null)
                    {
                        item = model.Instances.New<IfcClassificationItem>(c => {
                            c.ItemOf = classification;
                            c.Notation = model.Instances.New<IfcClassificationNotationFacet>(f => f.NotationValue = code);
                        });
                    }

                    item.Title = description; //set description
                    if (!String.IsNullOrEmpty(parent)) //set parent node
                    {
                        var parentNode = model.Instances.Where<IfcClassificationItem>(c => c.ItemOf == classification && c.Notation.NotationValue == parent).FirstOrDefault();
                        if (parentNode == null)
                            parentNode = model.Instances.New<IfcClassificationItem>(c => {
                                c.ItemOf = classification;
                                c.Notation = model.Instances.New<IfcClassificationNotationFacet>(f => f.NotationValue = parent);
                            });
                        var rel = model.Instances.Where<IfcClassificationItemRelationship>(r => r.RelatingItem == parentNode).FirstOrDefault();
                        if (rel == null)
                            rel = model.Instances.New<IfcClassificationItemRelationship>(r => r.RelatingItem = parentNode);
                        rel.RelatedItems.Add_Reversible(item);
                    }
                }
            });

            //perform creation of the classification system within transaction
            if (model.IsTransacting)
            {
                creation();
            }
            else
            {
                using (var txn = model.BeginTransaction("Creation of classification '" + name + "'"))
                {
                    creation();
                    txn.Commit();
                }
            }

            App.Library.Classifications.Add(new ViewModel.ClassificationViewModel(classification));
        }

        public void ExportToCSV(IfcClassification classification, string file)
        {
            var model = App.Library.Model;
            var format = "\"{0}\",\"{1}\",\"{2}\"";
            using (var writer = File.CreateText(file))
            {
                //write header
                writer.WriteLine(format, "Code", "Description", "Parent");

                //write all classification items
                foreach (var item in model.Instances.Where<IfcClassificationItem>(c => c.ItemOf == classification).OrderBy(c => c.Notation.NotationValue))
                {
                    var parent = "";
                    var rel = model.Instances.Where<IfcClassificationItemRelationship>(r => r.RelatedItems.Contains(item)).FirstOrDefault();
                    if (rel != null)
                        parent = rel.RelatingItem.Notation.NotationValue;

                    writer.WriteLine(format, item.Notation.NotationValue, item.Title, parent);
                }
                writer.Close();
            }
        }

        private string[] ParseLine(string line)
        {
            var results = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = results[i].Trim('"');
            }
            return results;
        }
    }
}
