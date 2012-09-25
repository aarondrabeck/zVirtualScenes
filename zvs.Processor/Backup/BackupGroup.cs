﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zvs.Entities;


namespace zvs.Processor.Backup
{
    public partial class Backup
    {
        [Serializable]
        public class GroupBackup
        {
            public string Name;
            public List<int> NodeNumbers = new List<int>();
        }       

        public static void ExportGroupsAsync(string PathFileName, Action<string> Callback)
        {
            List<GroupBackup> groups = new List<GroupBackup>();
            using (zvsContext context = new zvsContext())
            {
                foreach (Group group in context.Groups)
                {
                    GroupBackup backupGroup = new GroupBackup();
                    backupGroup.Name = group.Name;

                    foreach (Device d in group.Devices)
                        backupGroup.NodeNumbers.Add(d.NodeNumber);

                    groups.Add(backupGroup);
                }               
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GroupBackup>));
                xmlSerializer.Serialize(stream, groups);
                stream.Close();

                Callback(string.Format("Exported {0} groups to '{1}'", groups.Count, Path.GetFileName(PathFileName)));
            }
            catch (Exception e)
            {
                Callback("Error saving " + PathFileName + ": (" + e.Message + ")");
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public static void ImportGroupsAsync(string PathFileName, Action<string> Callback)
        {
            List<GroupBackup> groups = new List<GroupBackup>();
            int ImportedCount = 0;

            FileStream myFileStream = null;
            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<GroupBackup>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    groups = (List<GroupBackup>)ScenesSerializer.Deserialize(myFileStream);                    

                    using (zvsContext context = new zvsContext())
                    {
                        foreach (GroupBackup backupGroup in groups)
                        {
                            Group g = new Group();
                            g.Name = backupGroup.Name;

                            foreach (int NodeID in backupGroup.NodeNumbers) 
                            {
                                Device d = context.Devices.FirstOrDefault(o => o.NodeNumber == NodeID);
                                if (d != null)
                                    g.Devices.Add(d);
                            }

                            context.Groups.Add(g);
                            ImportedCount++;
                        }
                        context.SaveChanges();
                    }

                    Callback(string.Format("Imported {0} groups from '{1}'",ImportedCount, Path.GetFileName(PathFileName)));
                }
                else
                    Callback(string.Format("File '{0}' not found.", PathFileName));

            }
            catch (Exception e)
            {
                Callback("Error importing " + PathFileName + ": (" + e.Message + ")");
            }
            finally
            {

                if (myFileStream != null)
                    myFileStream.Close();
            }
        }
    }


}