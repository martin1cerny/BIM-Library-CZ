using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSpec.Plugins
{
    public class AddCzechTranslations : IExternalCommand
    {
        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            using (var txn = model.BeginTansaction("Translation to Czech"))
            {
                try
                {
                    var data = Data.TermsAndDefinitions.terminy_a_definice_CS.Split('\n');
                    foreach (var line in data)
                    {
                        var values = line.Split(new[]{ '\t'}, StringSplitOptions.RemoveEmptyEntries);


                    }


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
