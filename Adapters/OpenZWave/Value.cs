using System;
using OpenZWaveDotNet;

namespace OpenZWavePlugin
{
    public class Value
    {
        private ZWValueID m_valueID;
        public ZWValueID ValueID
        {
            get { return m_valueID; }
            set { m_valueID = value; }
        }

        private String m_genre;
        public String Genre
        {
            get { return m_genre; }
            set { m_genre = value; }
        }

        private String m_cmdClassID;
        public String CommandClassID
        {
            get { return m_cmdClassID; }
            set { m_cmdClassID = value; }
        }

        private String m_index;
        public String Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        private String m_type;
        public String Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private String m_val;
        public String Val
        {
            get { return m_val; }
            set { m_val = value; }
        }

        private String m_label;
        public String Label
        {
            get { return m_label; }
            set { m_label = value; }
        }

        private String m_help = "";
        public String Help
        {
            get { return m_help; }
            set { m_help = value; }
        }
    }
}
