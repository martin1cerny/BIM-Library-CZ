using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BLData.Actors;
using BLData.Exceptions;

namespace BLData.Comments
{
    public class Comment : BLModelEntity
    {
        private Guid __forEntityId;
        [XmlAttribute("ForEntity")]
        public Guid _forEntityId
        {
            get { return __forEntityId; }
            set { if (_model != null) __forEntityId = value; else throw new InvalidFieldOperationException(); }
        }
        [XmlIgnore]
        public BLEntity ForEntity
        {
            get { return _model.Get<BLEntity>(_forEntityId); }
            set { var old = _forEntityId; Set("ForEntity", () => _forEntityId = value != null? value.Id : Guid.Empty, () => _forEntityId = old); }
        }

        //issue date (DateTime)
        private DateTime _issueDate = DateTime.Now;
        public DateTime IssueDate {
            get { return _issueDate; }
            set { var old = _issueDate; Set("IssueDate", () => _issueDate = value, () => _issueDate = old); }
        }


        //state (enum)
        private CommentStateEnum _state = CommentStateEnum.ISSUE;
        public CommentStateEnum State
        {
            get { return _state; }
            set { var old = _state; Set("State", () => _state = value, () => _state = old); }
        }

        //issue person
        private Guid __issuePersonId;
        [XmlElement("IssuePersonId")]
        public Guid _issuePersonId { 
            get { return __issuePersonId; } 
            set { if (_model == null) __issuePersonId = value; else throw new InvalidFieldOperationException(); }
        }
        [XmlIgnore]
        public BLPerson IssuedBy
        {
            get { return _model.Get<BLPerson>(__issuePersonId); }
            set { var old = __issuePersonId; Set("IssuedBy", () => __issuePersonId = value != null ? value.Id : Guid.Empty, () => __issuePersonId = old); }
        }

        //solution date
        private DateTime _solutionDate = DateTime.Now;
        public DateTime SolutionDate
        {
            get { return _solutionDate; }
            set { var old = _solutionDate; Set("SolutionDate", () => _solutionDate = value, () => _solutionDate = old); }
        }

        //solution (string)
        private string _solution;
        public string Solution {
            get { return _solution; }
            set { var old = _solution; Set("Solution", () => _solution = value, () => _solution = old); }
        }

        //issue (string)
        private string _issue;
        public string Issue {
            get { return _issue; }
            set { var old = _issue; Set("Issue", () => _issue = value, () => _issue = old); }
        }

        //resolved by person
        private Guid __resolvedById;
        public Guid _resolvedById { 
            get { return __resolvedById; }
            set { if (_model == null) __resolvedById = value; else throw new InvalidFieldOperationException(); }
        }
        [XmlIgnore]
        public BLPerson SolvedBy {
            get { return _model.Get<BLPerson>(__resolvedById); }
            set { var old = __resolvedById; Set("SolvedBy", () => __resolvedById = value != null ? value.Id : Guid.Empty, () => __resolvedById = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            var result = "";

            return result;
        }


        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }

    public enum CommentStateEnum
    {
        ISSUE,
        REVIEW,
        RESOLVED
    }
}
