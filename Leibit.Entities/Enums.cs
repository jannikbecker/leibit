
using System;

namespace Leibit.Entities
{

    [Serializable]
    public enum eBlockDirection
    {
        Left,
        Right,
        Both
    }

    public enum eScheduleDirection
    {
        Unknown,
        LeftToRight,
        LeftToLeft,
        RightToLeft,
        RightToRight
    }

    public enum eHandling
    {
        Unknown,
        StopPassengerTrain,
        StopFreightTrain,
        Start,
        Destination,
        Transit,
        StaffChange
    }

    [Serializable]
    public enum eDelayType
    {
        NotSet = 0,
        Arrival = 1,
        Departure = 2
    }

    [Serializable]
    public enum eChildWindowType
    {
        NotSet = 0,
        DelayJustification = 1,
        ESTWSelection = 2,
        Settings = 3,
        TimeTable = 4,
        TrainProgressInformation = 5,
        TrainSchedule = 6,
        LocalOrders = 7,
        SystemState = 8,
        TrainComposition = 9,
        Display = 10,
    }

    public enum eTrainState
    {
        None,
        Composed,
        Prepared,
    }

    public enum eInfrastructureManager
    {
        DB,
        OEBB,
        SBB,
    }

    public enum eSkin
    {
        Light = 0,
        Dark = 1,
    }

    public enum eAutomaticReadyMessageBehaviour
    {
        Disabled = 0,
        Fix = 1,
        Random = 2,
    }
}
