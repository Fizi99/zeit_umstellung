using UnityEngine;

public class Bus
{
    public string line;
    public string headsign;
    public long time;
    public long realtime;

    public Bus(string line, string headsign, long time, long realtime)
    {
        this.line = line;
        this.headsign = headsign;
        this.time = time;
        this.realtime = realtime;

    }

}
