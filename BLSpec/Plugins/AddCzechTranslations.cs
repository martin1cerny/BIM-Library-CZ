using BLData.PropertySets;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSpec.Plugins
{
    public class AddCzechTranslations
#if DEBUG
 : IExternalCommand
#endif
    {
        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(@"c:\CODE\BIM-Library\BLSpec\Data\Terms_and_definitions.xls", FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            var sheet = workbook.GetSheetAt(0);
            var enumerator = sheet.GetRowEnumerator();

            using (var txn = model.BeginTansaction("Translation to Czech"))
            {
                try
                {
                    PropertySetDef pSet = null;
                    var rowNum = 0;
                    while (enumerator.MoveNext())
                    {
                        var row = enumerator.Current as HSSFRow;
                       
                        //english and czech values
                        var aName = row.Cells[0].StringCellValue.Trim();
                        var cName = row.Cells[1].StringCellValue.Trim();
                        var aDefinition = row.Cells[2].StringCellValue.Trim();
                        var cDefinition = row.Cells[3].StringCellValue.Trim();
                     
                        //if this is first after empty line or at the beginning of the file, it is property set nama and definition
                        if (row.RowNum != rowNum || pSet == null)
                        {
                            rowNum = row.RowNum;
                            model.Information.Lang = "en-US";
                            pSet = model.Get<PropertySetDef>().FirstOrDefault(ps => ps.LocalizedName.Replace(" ", "") == aName.Replace(" ", ""));
                            if (pSet == null)
                                throw new Exception("Property set not found for '" + aName + "'");

                            model.Information.Lang = "cs-CZ";
                            pSet.LocalizedName = cName;
                            pSet.LocalizedDefinition = cDefinition;
                            rowNum++;
                            continue;
                        }

                        model.Information.Lang = "en-US";
                        var prop = pSet.AllPropertyDefinitions.FirstOrDefault(p => p.LocalizedName.Replace(" ", "") == aName.Replace(" ", ""));
                        if (prop == null)
                            throw new Exception("Property '" + cName + "' was not found.");
                        model.Information.Lang = "cs-CZ";
                        prop.LocalizedName = cName;
                        prop.LocalizedDefinition = cDefinition;
                        rowNum++;
                    }

                    
                    //set model to cs-CZ at the end
                    model.Information.Lang = "cs-CZ";

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
            get { return "Add Czech translation"; }
        }

        public Guid ID
        {
            get { return new Guid("255C3981-3D44-4B1A-B3A4-F64668B05B66"); }
        }
    }
}
