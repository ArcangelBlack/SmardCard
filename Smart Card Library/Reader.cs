using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartCard.Library
{
    public class Reader
    {
        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public Reader(string name)
        {
            m_name = name;
        }
    }
}
