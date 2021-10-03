using System.Collections.Generic;
using System.Diagnostics;

namespace Leibit.Entities.Common
{
    [DebuggerDisplay("{Name}")]
    public class Track
    {

        private string m_Name;
        private bool m_IsPlatform;
        private bool m_CalculateDelay;
        private Station m_Station;
        private Track m_Parent;
        private List<Block> m_Blocks;
        private List<Track> m_Alternatives;

        public Track(string name, bool isPlatform, bool calculateDelay, Station station, Track parent)
            : this(name, isPlatform, calculateDelay, station, parent, null, null)
        {

        }

        public Track(string name, bool isPlatform, bool calculateDelay, Station station, Track parent, string displayName, string displaySubName)
        {
            m_Name = name;
            m_IsPlatform = isPlatform;
            m_CalculateDelay = calculateDelay;
            m_Station = station;
            m_Parent = parent;

            DisplayName = displayName;
            DisplaySubName = displaySubName;

            m_Blocks = new List<Block>();
            m_Alternatives = new List<Track>();

            if (m_Station != null)
                m_Station.Tracks.Add(this);
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public string DisplayName { get; }
        public string DisplaySubName { get; }

        public bool IsPlatform
        {
            get
            {
                return m_IsPlatform;
            }
        }

        public bool CalculateDelay
        {
            get
            {
                return m_CalculateDelay;
            }
        }

        public Station Station
        {
            get
            {
                return m_Station;
            }
        }

        public Track Parent
        {
            get
            {
                if (m_Parent == null)
                    return this;

                return m_Parent;
            }
        }

        public List<Block> Blocks
        {
            get
            {
                return m_Blocks;
            }
        }

        public List<Track> Alternatives
        {
            get
            {
                return m_Alternatives;
            }
        }

    }
}
