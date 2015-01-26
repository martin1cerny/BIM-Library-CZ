using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLSpec;

namespace Xbim.ExpressParser
{
    public class CreateClassificationFromSchema : IExternalCommand
    {
        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            using (var file = new MemoryStream(Schemas.SchemasDefinitions.IFC4))
            {
                using (var txn = model.BeginTansaction("Creation of the IFC structure"))
                {
                    try
                    {
                        var scanner = new Scanner(file);
                        var parser = new Parser(scanner, IfcVersionEnum.IFC4, model);
                        var result = parser.Parse();

                        if (parser.Output != null)
                            parser.Output.Close();

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
                            Console.WriteLine("Completed with no errors.");

                        if (!result)
                        {
                            ui.ShowMessage("Creation failed", "Creation of the structure failed. Operation canceled.");
                            var err = String.Concat(scanner.Errors);
                            txn.RollBack();
                        }
                        else
                            txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.RollBack();                        
                        throw;
                    }
                }

                
                

                file.Close();
            }
        }

        public string Name
        {
            get { return "Create IFC4 structure"; }
        }

        public Guid ID
        {
            get { return new Guid("7464EFB2-ADAD-4D98-AB00-6C341AD1DA91"); }
        }
    }
}
