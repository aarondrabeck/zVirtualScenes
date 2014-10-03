using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
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

            var changed = false;

            if (existing_option == null)
            {
                context.ProgramOptions.Add(programOption);
                changed = true;
            }
            else
            {
                if (existing_option.Value != programOption.Value)
                {
                    changed = true;
                    existing_option.Value = programOption.Value;
                }
            }

            if (changed)
                return await context.TrySaveChangesAsync();

            return new Result();
        }

    }
}
