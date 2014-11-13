using System;

namespace zvs.DataModel.Tasks
{
    [Flags]
    public enum DaysOfMonth
    {
        First = 1 << 0,  
        Second = 1 << 1,
        Third = 1 << 2, 
        Fourth = 1 << 3,
        Fifth = 1 << 4, 
        Sixth = 1 << 5,
        Seventh = 1 << 6,
        Eighth = 1 << 7,
        Ninth = 1 << 8,
        Tenth = 1 << 9,
        Eleventh = 1 << 10,
        Twelfth = 1 << 11,
        Thirteenth = 1 << 12,
        Fourteenth = 1 << 13,
        Fiftieth = 1 << 14,
        Sixteenth = 1 << 15,
        Seventeenth = 1 << 16,
        Eighteenth = 1 << 17,
        Nineteenth = 1 << 18,
        Twentieth = 1 << 19,
        Twentyfirst = 1 << 20,
        Twentysecond = 1 << 21,
        Twentythrid = 1 << 22,
        Twentyfourth = 1 << 23,
        Twentyfifth = 1 << 24,
        Twentysixth = 1 << 25,
        Twentyseventh = 1 << 26,
        Twentyeighth = 1 << 27,
        Twentyninth = 1 << 28,
        Thirtieth = 1 << 29,
        Thirtyfirst = 1 << 30,
        All = ~(-1 << 31),
        Odd = First | Third | Fifth | Seventh | Ninth | Eleventh | Thirteenth | Fiftieth | Seventeenth | Nineteenth | Twentyfirst | Twentythrid | Twentyfifth | Twentyseventh | Twentyninth | Thirtyfirst,
        Even = Second | Fourth | Sixth | Eighth | Tenth | Twelfth | Fourteenth | Sixteenth | Eighteenth | Twentieth | Twentysecond | Twentyfourth | Twentysixth | Twentyeighth | Thirtieth
             

    };
}
