using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("ProgramOptions", Schema = "ZVS")]
    public partial class ProgramOption : INotifyPropertyChanged, IIdentity
    {

        public int Id { get; set; }

        private string _UniqueIdentifier;
        [StringLength(50)]
        public string UniqueIdentifier
        {
            get
            {
                return _UniqueIdentifier;
            }
            set
            {
                if (value != _UniqueIdentifier)
                {
                    _UniqueIdentifier = value;
                    NotifyPropertyChanged("UniqueIdentifier");
                }
            }
        }

        private string _Value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public static bool TryAddOrEdit(zvsContext context, ProgramOption opt, out string error)
        {

            if (opt == null)
            {
                error = "ProgramOption is null";
                return false;
            }


            ProgramOption existing_option = context.ProgramOptions.FirstOrDefault(o => o.UniqueIdentifier == opt.UniqueIdentifier);

            if (existing_option == null)
                context.ProgramOptions.Add(opt);
            else
                existing_option.Value = opt.Value;

            if (!context.TrySaveChanges(out error))
                return false;

            return true;

        }

        public static string GetProgramOption(zvsContext context, string UniqueIdentifier)
        {
            ProgramOption option = context.ProgramOptions.FirstOrDefault(o => o.UniqueIdentifier == UniqueIdentifier);

            if (option != null)
                return option.Value;

            return null;
        }
    }
}
