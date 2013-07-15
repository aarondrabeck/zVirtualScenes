using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("QueuedCommands", Schema = "ZVS")]
    public class QueuedCommand : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
