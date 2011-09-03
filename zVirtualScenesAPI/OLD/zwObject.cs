//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using zVirtualScenesCommon.DatabaseCommon;
//using System.Xml.Serialization;

//namespace zVirtualScenesAPI.Structs
//{
//    public class zwObject
//    {
//        public int ID;
//        public string Name;
//        public int Node_ID;
//        public string Type;
//        public int Type_ID;
//        public string API_Name;
//        public int Level;
//        public decimal Temperature;
//        public bool On;

//        [XmlIgnore]
//        public string Groups;
//        [XmlIgnore]
//        public DateTime LastHeardFrom = new DateTime();

//        public string DeviceIcon()
//        {
//            if (Type.Equals("THERMOSTAT"))
//                return "20zwave-thermostat.png";
//            else if (Type.Equals("DIMMER"))
//                return "20bulb.png";
//            else if (Type.Equals("SWITCH"))
//                return "20switch.png";
//            else if (Type.Equals("CONTROLLER"))
//                return "controler320.png";
//            else if (Type.Equals("DOORLOCK"))
//                return "doorlock20";
//            else             
//                return "20radio2.png";
//        }

//        public override string ToString()
//        {
//            return Name;
//        }

//        public int GetLevelMeter()
//        {
//            if (Type.Equals("THERMOSTAT"))
//                return (int)this.Temperature;
//            else if (Type.Equals("SWITCH"))
//                return (On ? 100 : 0);
//            else
//                return this.Level;
//        }

//        public string GetLevelText()
//        {
//            if (Type.Equals("THERMOSTAT"))
//                return this.Temperature + " F";
//            else if (Type.Equals("SWITCH"))
//                return (On ? "ON" : "OFF");
//            else
//                return this.Level + "%";
//        }

//        public static implicit operator zwObject(DataRow dr)
//        {
//            zwObject zwObj = new zwObject();
//            int.TryParse(dr["id"].ToString(), out zwObj.ID);
//            int.TryParse(dr["node_id"].ToString(), out zwObj.Node_ID);
//            int.TryParse(dr["object_type_id"].ToString(), out zwObj.Type_ID);

//            zwObj.Name = dr["txt_object_name"].ToString();
//            zwObj.Type = dr["txt_object_type"].ToString();
//            zwObj.API_Name = dr["txt_api_name"].ToString();
//            DateTime.TryParse(dr["last_heard_from"].ToString(), out zwObj.LastHeardFrom);

//            //Add basic levels
//            switch (zwObj.Type)
//            {
//                case "SWITCH":
//                    int basicVal;
//                    int.TryParse(DatabaseControl.GetObjectValue(zwObj.ID, "Basic"), out basicVal);
//                    zwObj.On = (basicVal > 0);
//                    break;
//                case "DIMMER":
//                    int.TryParse(DatabaseControl.GetObjectValue(zwObj.ID, "Basic"), out zwObj.Level);
//                    break;
//                case "THERMOSTAT":
//                    decimal.TryParse(DatabaseControl.GetObjectValue(zwObj.ID, "Temperature"), out zwObj.Temperature);
//                    break;
//            }

//            //Add Groups
//            zwObj.Groups = String.Empty;
//            DataTable dt_groups = DatabaseControl.GetObjectGroups(zwObj.ID);

//            foreach (DataRow dr_group in dt_groups.Rows)
//            {
//                //if this group is the last one dont add a ,
//                string append = (dt_groups.Rows[dt_groups.Rows.Count - 1].Equals(dr_group) ? "" : ", ");
//                zwObj.Groups += dr_group["txt_group_name"].ToString() + append;
//            }

//            return zwObj;
//        }


//        public static List<zwObject> ConvertObjDataTabletoObjList(DataTable dt)
//        {
//            List<zwObject> objs = new List<zwObject>();

//            foreach (DataRow dr in dt.Rows)
//            {
//                zwObject zwObj = (zwObject)dr;
//                objs.Add(zwObj);
//            }

//            return objs;
//        }
//    }
//}
