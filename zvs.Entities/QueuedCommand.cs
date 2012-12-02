using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("QueuedCommands", Schema = "ZVS")]
    public class QueuedCommand : INotifyPropertyChanged, IIdentity
    {
        public int Id { get; set; }

        public int? CommandId { get; set; }
        public virtual Command Command { get; set; }

        private string _Argument;
        [StringLength(512)]
        public string Argument
        {
            get
            {
                return _Argument;
            }
            set
            {
                if (value != _Argument)
                {
                    _Argument = value;
                    NotifyPropertyChanged("Argument");
                }
            }
        }

        private string _Argument2;
        [StringLength(512)]
        public string Argument2
        {
            get
            {
                return _Argument2;
            }
            set
            {
                if (value != _Argument2)
                {
                    _Argument2 = value;
                    NotifyPropertyChanged("Argument2");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        

    }
}
