using UnityEngine;

public class Bus
{
    public string line;
    public string headsign;
    public int time;
    public int realtime;

    public Bus(string line, string headsign, int time, int realtime)
    {
        this.line = line;
        this.headsign = headsign;
        this.time = time;
        this.realtime = realtime;

    }

}
