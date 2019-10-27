using STCP_API.Models.Entities;
using STCP_API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCP_API.Models
{
    public static class Locate
    {
        public static LocatedLine LocateBusesFromLine(Line line)
        {
            CheckIfNoBuses(line);

            var locatedLine = new LocatedLine(line.Number, line.LineDirection);

            for (int i = 0; i < line.Stops.Count; i++)
            {
                if (line.Stops[i].IncomingBuses.Count != 0)
                {
                    foreach (var bus in line.Stops[i].IncomingBuses)
                    {
                        var probableBus = new LocatedBus(bus.Destination, line.Stops[i].BusStopId, line.Stops[i].BusStopName, line.Stops[i].Zone, bus.EstimatedTime, bus.WaitingTime);
                        if (locatedLine.LocatedBuses.Count == 0)
                        {
                            locatedLine.LocatedBuses.Add(probableBus);
                        }
                        else
                        {
                            foreach (var previouslyLocatedBus in locatedLine.LocatedBuses)
                            {
                                if (!AreSameBus(previouslyLocatedBus, probableBus, line))
                                {
                                    locatedLine.LocatedBuses.Add(probableBus);
                                }
                            }
                        }
                    }
                }
            }

            return locatedLine;
        }

        private static void CheckIfNoBuses(Line line)
        {
            bool busFound = false;
            foreach (var stop in line.Stops)
            {
                busFound = stop.IncomingBuses.Count > 0 ? true : false;
                if (busFound)
                {
                    break;
                }
            }
            if (!busFound) throw new NoBusesToLocateException(line.Number);
        }

        private static bool AreSameBus(LocatedBus bus1, LocatedBus bus2, Line line)
        {
            const int toleranceInMinutes = 2;

            var stopId1 = line.Stops.FindIndex(x => x.BusStopId == bus1.NextStopId);
            var stopId2 = line.Stops.FindIndex(x => x.BusStopId == bus2.NextStopId);
            var numberOfStopsBetweenBuses = Math.Abs(stopId2 - stopId1);

            bool sameDestination = bus1.Destination.Equals(bus2.Destination);

            bool nearWaitingTime = (bus1.WaitingTime <= (bus2.WaitingTime + (toleranceInMinutes * numberOfStopsBetweenBuses)))
                    || (bus1.WaitingTime >= (bus2.WaitingTime - (toleranceInMinutes * numberOfStopsBetweenBuses)));

            return sameDestination && nearWaitingTime;
        }
    }
}
