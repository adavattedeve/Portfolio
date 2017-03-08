using UnityEngine;
using System.Collections.Generic;

public class StartEndRoomDecider
{
    //How many times starting room should be tried to choose randomly before iterating over all rooms.
    private int randomTries=3;

    public bool DetermineStartAndEndRooms(List<RoomMetaData> allRooms, int minDistance)
    {
        RoomMetaData startingRoom = null;
        RoomMetaData endingRoom = null;
        List<RoomMetaData> invalidEndingRooms = null;

        int counter = 0;
        //Try choosing starting room randomly
        while (randomTries > counter)
        {
            ++counter;
            startingRoom = allRooms[Random.Range(0, allRooms.Count)];
            invalidEndingRooms = GetInvalidEndRooms(startingRoom, minDistance);
            if (invalidEndingRooms != null)
                break;
        }
        //if no valid room found, iterate through all rooms to find valid start/end rooms.
        if (invalidEndingRooms == null)
        {
            for (int i = 0; i < allRooms.Count; ++i)
            {
                startingRoom = allRooms[i];
                invalidEndingRooms = GetInvalidEndRooms(startingRoom, minDistance);
                if (invalidEndingRooms != null)
                    break;
            }
        }
        //if valid start/end rooms not still found return false;
        if (invalidEndingRooms == null)
            return false;

        counter = 0;
        //Try choosing ending room randomly
        while (randomTries > counter)
        {
            ++counter;
            endingRoom = allRooms[Random.Range(0, allRooms.Count)];
            if (!invalidEndingRooms.Contains(endingRoom))
            {
                startingRoom.Type = RoomType.STARTINGROOM;
                endingRoom.Type = RoomType.ENDINGROOM;
                return true;
            }
        }

        for (int i = 0; i < allRooms.Count; ++i)
        {
            endingRoom = allRooms[i];
            if (!invalidEndingRooms.Contains(endingRoom))
            {
                startingRoom.Type = RoomType.STARTINGROOM;
                endingRoom.Type = RoomType.ENDINGROOM;
                return true;
            }
        }
        
        return false;
    }

    //Takes start room as parameter and return invalid end rooms if starting room is valid. Otherwise returns null.
    private List<RoomMetaData> GetInvalidEndRooms(RoomMetaData startingRoom, int minDistance)
    {
        List<RoomMetaData> invalidRooms = new List<RoomMetaData>();
        List<RoomMetaData> roomList = new List<RoomMetaData>();
        invalidRooms.Add(startingRoom);
        roomList.Add(startingRoom);

        int counter = 0;
        while (minDistance > counter)
        {
            List<RoomMetaData> newRoomList = new List<RoomMetaData>();

            for (int i = 0; i < roomList.Count; ++i)
            {
                for (int i2 = 0; i2 < roomList[i].Neighbours.Count; ++i2)
                {
                    RoomMetaData current = roomList[i].Neighbours[i2];
                    if (!invalidRooms.Contains(current))
                    {
                        newRoomList.Add(current);
                        invalidRooms.Add(current);
                    }
                }
            }
            if (newRoomList.Count == 0)
            {
                return null;
            }
            roomList = newRoomList;
            ++counter;
        }
        return invalidRooms;
    
    }
}
