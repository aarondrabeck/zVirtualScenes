using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;
using zVirtualScenesCommon.Util;


namespace zVirtualScenesCommon.Entity
{   
    public partial class group : EntityObject
    {       
       public static void RemoveGroup(group g)
       {
           using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
           {
               db.groups.Attach(g);

               //Remove each device from the group and notify each
               foreach (group_devices gd in g.group_devices)
               {
                   db.group_devices.DeleteObject(gd);
                   db.SaveChanges();
                   zvsEntityControl.CallonSaveChanges(null, new List<zVirtualScenesCommon.Entity.zvsEntityControl.Tables>() { zvsEntityControl.Tables.group });
               }
               db.groups.DeleteObject(g);
               db.SaveChanges();
           }
       }

       public static void RenameGroup(group g, string NewName)
       {
           using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
           {
               db.groups.Attach(g);
               g.name = NewName;
               db.SaveChanges();
               zvsEntityControl.CallonSaveChanges(null, new List<zVirtualScenesCommon.Entity.zvsEntityControl.Tables>() { zvsEntityControl.Tables.group });
           }  
       }
    
    }
}