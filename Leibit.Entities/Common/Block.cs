using System.Collections.Generic;
using System.Diagnostics;

namespace Leibit.Entities.Common
{
    [DebuggerDisplay("{Name}")]
    public class Block
    {

        private string m_Name;
        private eBlockDirection m_Direction;
        private Track m_Track;

        public Block(string name, eBlockDirection direction, Track track)
        {
            m_Name = name;
            m_Direction = direction;
            m_Track = track;

            if (track != null)
                track.Blocks.Add(this);

            if (track != null && track.Station != null && track.Station.ESTW != null && track.Station.ESTW.Area != null)
            {
                if (!track.Station.ESTW.Blocks.ContainsKey(m_Name))
                    track.Station.ESTW.Blocks[m_Name] = new List<Block>();

                track.Station.ESTW.Blocks[m_Name].Add(this);

            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public eBlockDirection Direction
        {
            get
            {
                return this.m_Direction;
            }
        }

        public Track Track
        {
            get
            {
                return m_Track;
            }
        }

    }
}
