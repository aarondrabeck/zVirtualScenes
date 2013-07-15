using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace zvs.Entities
{
    [Table("ProgramOptions", Schema = "ZVS")]
    public partial class ProgramOption : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private string _UniqueIdentifier;
        [StringLength(255)]
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //TODO: MOVE TO EXTENSION METHOD
        public static async Task<Result> TryAddOrEditAsync(zvsContext context, ProgramOption programOption)
        {
            if (programOption == null)
                throw new ArgumentNullException("programOption");

            var existing_option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == programOption.UniqueIdentifier);

            if (existing_option == null)
                context.ProgramOptions.Add(programOption);
            else
                existing_option.Value = programOption.Value;

            return await context.TrySaveChangesAsync();
        }

        //TODO: MOVE TO EXTENSION METHOD
        public static string GetProgramOption(zvsContext context, string UniqueIdentifier)
        {
            ProgramOption option = context.ProgramOptions.FirstOrDefault(o => o.UniqueIdentifier == UniqueIdentifier);

            if (option != null)
                return option.Value;

            return null;
        }
    }
}
