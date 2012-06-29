using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using zVirtualScenesModel;

namespace zVirtualScenes.Backup
{
    public partial class Backup
    {
        [Serializable]
        public class BackupScheduledTask
        {
            public int? Frequency;
            public string friendly_name;
            public string sceneName;
            public bool isEnabled;
            public bool? RecurDay01;
            public bool? RecurDay02;
            public bool? RecurDay03;
            public bool? RecurDay04;
            public bool? RecurDay05;
            public bool? RecurDay06;
            public bool? RecurDay07;
            public bool? RecurDay08;
            public bool? RecurDay09;
            public bool? RecurDay10;
            public bool? RecurDay11;
            public bool? RecurDay12;
            public bool? RecurDay13;
            public bool? RecurDay14;
            public bool? RecurDay15;
            public bool? RecurDay16;
            public bool? RecurDay17;
            public bool? RecurDay18;
            public bool? RecurDay19;
            public bool? RecurDay20;
            public bool? RecurDay21;
            public bool? RecurDay22;
            public bool? RecurDay23;
            public bool? RecurDay24;
            public bool? RecurDay25;
            public bool? RecurDay26;
            public bool? RecurDay27;
            public bool? RecurDay28;
            public bool? RecurDay29;
            public bool? RecurDay30;
            public bool? RecurDay31;
            public int? RecurDayofMonth;
            public int? RecurDays;
            public bool? RecurEven;
            public bool? RecurFriday;
            public bool? RecurMonday;
            public int? RecurMonth;
            public bool? RecurSaturday;
            public int? RecurSeconds;
            public bool? RecurSunday;
            public bool? RecurThursday;
            public bool? RecurTuesday;
            public bool? RecurWednesday;
            public int? RecurWeeks;
            public int? sortOrder;
            public DateTime? startTime;
        }

        public static void ExportScheduledTaskAsyc(string PathFileName, Action<string> Callback)
        {
            List<BackupScheduledTask> tasks = new List<BackupScheduledTask>();
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                foreach (scheduled_tasks t in context.scheduled_tasks)
                {
                    BackupScheduledTask task = new BackupScheduledTask();
                    task.sceneName = t.scene.friendly_name;
                    task.Frequency = t.Frequency;
                    task.friendly_name = t.friendly_name;
                    task.isEnabled = t.isEnabled;
                    task.RecurDay01 = t.RecurDay01;
                    task.RecurDay02 = t.RecurDay02;
                    task.RecurDay03 = t.RecurDay03;
                    task.RecurDay04 = t.RecurDay04;
                    task.RecurDay05 = t.RecurDay05;
                    task.RecurDay06 = t.RecurDay06;
                    task.RecurDay07 = t.RecurDay07;
                    task.RecurDay08 = t.RecurDay08;
                    task.RecurDay09 = t.RecurDay09;
                    task.RecurDay10 = t.RecurDay10;
                    task.RecurDay11 = t.RecurDay11;
                    task.RecurDay12 = t.RecurDay12;
                    task.RecurDay13 = t.RecurDay13;
                    task.RecurDay14 = t.RecurDay14;
                    task.RecurDay15 = t.RecurDay15;
                    task.RecurDay16 = t.RecurDay16;
                    task.RecurDay17 = t.RecurDay17;
                    task.RecurDay18 = t.RecurDay18;
                    task.RecurDay19 = t.RecurDay19;
                    task.RecurDay20 = t.RecurDay20;
                    task.RecurDay21 = t.RecurDay21;
                    task.RecurDay22 = t.RecurDay22;
                    task.RecurDay23 = t.RecurDay23;
                    task.RecurDay24 = t.RecurDay24;
                    task.RecurDay25 = t.RecurDay25;
                    task.RecurDay26 = t.RecurDay26;
                    task.RecurDay27 = t.RecurDay27;
                    task.RecurDay28 = t.RecurDay28;
                    task.RecurDay29 = t.RecurDay29;
                    task.RecurDay30 = t.RecurDay30;
                    task.RecurDay31 = t.RecurDay31;
                    task.RecurDayofMonth = t.RecurDayofMonth;
                    task.RecurDays = t.RecurDays;
                    task.RecurEven = t.RecurEven;
                    task.RecurFriday = t.RecurFriday;
                    task.RecurMonday = t.RecurMonday;
                    task.RecurSaturday = t.RecurSaturday;
                    task.RecurMonth = t.RecurMonth;
                    task.RecurSeconds = t.RecurSeconds;
                    task.RecurSunday = t.RecurSunday;
                    task.RecurThursday = t.RecurThursday;
                    task.RecurTuesday = t.RecurTuesday;
                    task.RecurWednesday = t.RecurWednesday;
                    task.RecurWeeks = t.RecurWeeks;
                    task.sortOrder = t.sortOrder;
                    task.startTime = t.startTime;
                    tasks.Add(task);
                }
            }

            Stream stream = null;
            try
            {
                stream = File.Open(PathFileName, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<BackupScheduledTask>));
                xmlSerializer.Serialize(stream, tasks);
                stream.Close();
                Callback(string.Format("Exported {0} scheduled tasks to '{1}'", tasks.Count, Path.GetFileName(PathFileName)));
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

        public static void ImportScheduledTaskAsyn(string PathFileName, Action<string> Callback)
        {
            List<BackupScheduledTask> tasks = new List<BackupScheduledTask>();
            int ImportedCount = 0;

            FileStream myFileStream = null;
            try
            {
                if (File.Exists(PathFileName))
                {
                    //Open the file written above and read values from it.       
                    XmlSerializer ScenesSerializer = new XmlSerializer(typeof(List<BackupScheduledTask>));
                    myFileStream = new FileStream(PathFileName, FileMode.Open);
                    tasks = (List<BackupScheduledTask>)ScenesSerializer.Deserialize(myFileStream);
                    
                    using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                    {
                        foreach (BackupScheduledTask t in tasks)
                        {
                            scene s = context.scenes.FirstOrDefault(o => o.friendly_name == t.sceneName);

                            if (s != null)
                            {
                                scheduled_tasks task = new scheduled_tasks();
                                task.SceneID = s.id;
                                task.Frequency = t.Frequency;
                                task.friendly_name = t.friendly_name;
                                task.isEnabled = t.isEnabled;
                                task.RecurDay01 = t.RecurDay01;
                                task.RecurDay02 = t.RecurDay02;
                                task.RecurDay03 = t.RecurDay03;
                                task.RecurDay04 = t.RecurDay04;
                                task.RecurDay05 = t.RecurDay05;
                                task.RecurDay06 = t.RecurDay06;
                                task.RecurDay07 = t.RecurDay07;
                                task.RecurDay08 = t.RecurDay08;
                                task.RecurDay09 = t.RecurDay09;
                                task.RecurDay10 = t.RecurDay10;
                                task.RecurDay11 = t.RecurDay11;
                                task.RecurDay12 = t.RecurDay12;
                                task.RecurDay13 = t.RecurDay13;
                                task.RecurDay14 = t.RecurDay14;
                                task.RecurDay15 = t.RecurDay15;
                                task.RecurDay16 = t.RecurDay16;
                                task.RecurDay17 = t.RecurDay17;
                                task.RecurDay18 = t.RecurDay18;
                                task.RecurDay19 = t.RecurDay19;
                                task.RecurDay20 = t.RecurDay20;
                                task.RecurDay21 = t.RecurDay21;
                                task.RecurDay22 = t.RecurDay22;
                                task.RecurDay23 = t.RecurDay23;
                                task.RecurDay24 = t.RecurDay24;
                                task.RecurDay25 = t.RecurDay25;
                                task.RecurDay26 = t.RecurDay26;
                                task.RecurDay27 = t.RecurDay27;
                                task.RecurDay28 = t.RecurDay28;
                                task.RecurDay29 = t.RecurDay29;
                                task.RecurDay30 = t.RecurDay30;
                                task.RecurDay31 = t.RecurDay31;
                                task.RecurDayofMonth = t.RecurDayofMonth;
                                task.RecurDays = t.RecurDays;
                                task.RecurEven = t.RecurEven;
                                task.RecurFriday = t.RecurFriday;
                                task.RecurMonday = t.RecurMonday;
                                task.RecurSaturday = t.RecurSaturday;
                                task.RecurMonth = t.RecurMonth;
                                task.RecurSeconds = t.RecurSeconds;
                                task.RecurSunday = t.RecurSunday;
                                task.RecurThursday = t.RecurThursday;
                                task.RecurTuesday = t.RecurTuesday;
                                task.RecurWednesday = t.RecurWednesday;
                                task.RecurWeeks = t.RecurWeeks;
                                task.sortOrder = t.sortOrder;
                                task.startTime = t.startTime;

                                context.scheduled_tasks.Add(task);
                                ImportedCount++;

                            }
                        }
                        context.SaveChanges();
                    }
                    Callback(string.Format("Imported {0} scheduled tasks from '{1}'", ImportedCount, Path.GetFileName(PathFileName)));
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