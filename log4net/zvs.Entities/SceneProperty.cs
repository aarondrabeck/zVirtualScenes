﻿using System;
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
    [Table("SceneProperties", Schema = "ZVS")]
    public partial class SceneProperty : BaseValue
    {
        public int ScenePropertyId { get; set; }

        public SceneProperty()
        {
            this.Options = new ObservableCollection<ScenePropertyOption>();
        }

        public virtual ObservableCollection<ScenePropertyOption> Options { get; set; }


        public static void AddOrEdit(SceneProperty p, zvsContext context)
        {
            if (p != null)
            {
                SceneProperty existing_property = context.SceneProperties.FirstOrDefault(ep => ep.UniqueIdentifier == p.UniqueIdentifier);

                if (existing_property == null)
                {
                    context.SceneProperties.Add(p);
                }
                else
                {
                    //Update
                    existing_property.Name = p.Name;
                    existing_property.Description = p.Description;
                    existing_property.ValueType = p.ValueType;
                    existing_property.Value = p.Value;

                    existing_property.Options.ToList().ForEach(o =>
                    {
                        context.ScenePropertyOptions.Remove(o);                        
                    });
                    existing_property.Options.Clear();
                    p.Options.ToList().ForEach(o => existing_property.Options.Add(o));
                }
                context.SaveChanges();
            }
        }
    }
}
