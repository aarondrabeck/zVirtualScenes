using System;
using System.Collections.Generic;
using OpenZWaveDotNet;

namespace OpenZWavePlugin
{
    public class Node
    {
        private Byte m_id = 0;
        public Byte ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private UInt32 m_homeId = 0;
        public UInt32 HomeID
        {
            get { return m_homeId; }
            set { m_homeId = value; }
        }

        private String m_label = "";
        public String Label
        {
            get { return m_label; }
            set { m_label = value; }
        }

        private String m_manufacturer = "";
        public String Manufacturer
        {
            get { return m_manufacturer; }
            set { m_manufacturer = value; }
        }

        private String m_product = "";
        public String Product
        {
            get { return m_product; }
            set { m_product = value; }
        }

        private String m_location = "";
        public String Location
        {
            get { return m_location; }
            set { m_location = value; }
        }
        private String m_level;
        public String Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        private List<Value> m_values;
        public IEnumerable<Value> Values
        {
            get { return m_values; }
        }

        private String m_name;
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public Node()
        {
            m_values = new List<Value>();
        }

        public Value GetValue(ZWValueID zwv)
        {
            foreach (Value value in m_values)
            {
                if (value.CommandClassID == zwv.GetCommandClassId().ToString() && value.Index == zwv.GetIndex().ToString())
                {
                    return value;
                }
            }
            return new Value();
        }

        public void AddValue(Value val)
        {
            m_values.Add(val);
        }

        public void RemoveValue(Value val)
        {
            m_values.Remove(val);
        }
    }
}
