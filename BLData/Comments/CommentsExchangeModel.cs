using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO.Compression;
using BLData.Actors;
using System.IO;

namespace BLData.Comments
{
    public class CommentsExchangeModel
    {
        [XmlArrayItem("Comment")]
        public List<BLComment> Comments { get; set; }
        [XmlArrayItem("Person")]
        public List<BLPerson> People { get; set; }

        /// <summary>
        /// Adds comments and people to the model. This is supposed to be used after comments and
        /// people are loaded from external file. If comment exists already it isn't imported and
        /// this fact is described in the message returned by the function.
        /// </summary>
        /// <param name="model">Target model</param>
        /// <returns>Message about any issues which might have ocured during import. This is empty string or null if everything is OK.</returns>
        public string AddToModel(BLModel model)
        {
            if (Comments == null || People == null) return null;
            var msg = "";


            //get comments resource dictionary
            var comResource = model.GetResource<BLComment>();
            foreach (var com in Comments)
            {
                //check if this comment doesn't exist already
                var exist = comResource.Items.Any(c => c.Id == com.Id);
                if (exist)
                {
                    var person = People.FirstOrDefault(p => p.Id == com._issuePersonId);
                    msg += String.Format("Comment with ID {0}: {1} from {2} exists already. \n", com.Id, com.Issue, person.FullName);
                    continue;
                }

                com.SetModel(model);
                comResource.Items.Add(com);
            }


            var peopleResource = model.GetResource<BLPerson>();
            foreach (var per in People)
            {
                if (peopleResource.Items.Any(p => p.Id == per.Id))
                    continue;

                per.SetModel(model);
                peopleResource.Items.Add(per);
            }

            return msg;
        }

        /// <summary>
        /// This method loads comments from model into this exchange model. If you don't specify specific person
        /// all comments from all people are loaded. If you do specify one person only comments of this person
        /// will be loaded into this exchange model.
        /// </summary>
        /// <param name="model">Model to be used to load comments and people</param>
        /// <param name="issuePerson">Optional issue person</param>
        public void LoadFromModel(BLModel model, BLPerson issuePerson = null)
        {
            //init empty lists
            Comments = new List<BLComment>();
            People = new List<BLPerson>();

            //get comments from model
            IEnumerable<BLComment> comments;
            if (issuePerson == null)
                comments = model.Get<BLComment>();
            else
                comments = model.Get<BLComment>(c => c._issuePersonId == issuePerson.Id);

            Comments.AddRange(comments);
            foreach (var comment in comments)
            {
                if (!People.Any(p => p.Id == comment._issuePersonId))
                    People.Add(comment.IssuedBy);
            }
        }

        private const string innerName = "comments.xml";
        /// <summary>
        /// This will save zipped file containing only comments and related people.
        /// </summary>
        /// <param name="path"></param>
        public void SaveAs(string path)
        {
            using (var file = File.Create(path))
            {
                using (var archive = new ZipArchive(file, ZipArchiveMode.Create, false))
                {
                    var entry = archive.CreateEntry(innerName);
                    using (var data = entry.Open())
                    {
                        var serializer = new XmlSerializer(typeof(CommentsExchangeModel));
                        serializer.Serialize(data, this);
                        data.Close();
                    }
                }    
            }
        }

        public static CommentsExchangeModel LoadFromFile(string path)
        {
            using (var file = File.OpenRead(path))
            {
                using (var archive = new ZipArchive(file, ZipArchiveMode.Read, false))
                {
                    var entry = archive.GetEntry(innerName);
                    using (var data = entry.Open())
                    {
                        var serializer = new XmlSerializer(typeof(CommentsExchangeModel));
                        var result = (CommentsExchangeModel)serializer.Deserialize(data);
                        data.Close();
                        return result;
                    }
                }
            }
        }
    }
}
