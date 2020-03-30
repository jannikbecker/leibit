using Leibit.Core.Scheduling;
using System.Collections.Generic;
using System.Diagnostics;

namespace Leibit.Entities.Common
{
    [DebuggerDisplay("{Name}")]
    public class ESTW
    {

        private string m_Id;
        private string m_Name;
        private string m_DataFile;
        private Area m_Area;
        private List<Station> m_Stations;
        private Dictionary<string, List<Block>> m_Blocks;

        public ESTW(string id, string name, string dataFile, Area area)
        {
            m_Id = id;
            m_Name = name;
            m_DataFile = dataFile;
            m_Area = area;

            m_Stations = new List<Station>();
            m_Blocks = new Dictionary<string, List<Block>>();

            if (area != null)
                area.ESTWs.Add(this);
        }

        public string Id
        {
            get
            {
                return m_Id;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public string DataFile
        {
            get
            {
                return m_DataFile;
            }
        }

        public Area Area
        {
            get
            {
                return m_Area;
            }
        }

        public List<Station> Stations
        {
            get
            {
                return m_Stations;
            }
        }

        public Dictionary<string, List<Block>> Blocks
        {
            get
            {
                return m_Blocks;
            }
        }

        public bool IsLoaded { get; set; }

        public LeibitTime Time { get; set; }

        public int IOExceptionCount { get; set; }

    }
}
