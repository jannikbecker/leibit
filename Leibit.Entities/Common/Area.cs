using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Leibit.Entities.Common
{
    [DebuggerDisplay("{Name}")]
    public class Area
    {

        private string m_Id;
        private string m_Name;
        private List<ESTW> m_Estws;
        private ConcurrentDictionary<int, Train> m_Trains;
        private ConcurrentDictionary<int, TrainInformation> m_LiveTrains;

        public Area(string id, string name)
        {
            m_Id = id;
            m_Name = name;
            m_Estws = new List<ESTW>();
            m_Trains = new ConcurrentDictionary<int, Train>();
            m_LiveTrains = new ConcurrentDictionary<int, TrainInformation>();
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

        public List<ESTW> ESTWs
        {
            get
            {
                return m_Estws;
            }
        }

        public ConcurrentDictionary<int, Train> Trains
        {
            get
            {
                return m_Trains;
            }
        }

        public ConcurrentDictionary<int, TrainInformation> LiveTrains
        {
            get
            {
                return m_LiveTrains;
            }
        }

    }
}
