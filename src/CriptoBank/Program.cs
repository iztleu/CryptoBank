// See https://aka.ms/new-console-template for more information
unsafe
{
    Point point = new Point(0, 0);
    point.Value = new("Point1", 1);
    Console.WriteLine(point);   // X: 0  Y: 0
    Point* p = &point;
 
    p->X = 30;
    Console.WriteLine(p->X);    // 30

    p->Value = new("Point2", 2);
    Console.WriteLine(p->Value.ToString());
 
    // разыменовывание указателя
    (*p).Y = 180;
    Console.WriteLine((*p).Y);  // 180
 
    Console.WriteLine(point);   // X: 30  Y: 180
}


struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public Value Value { get; set; }
    public Point(int x, int y)
    {
        X = x; Y = y;
    }
    public override string ToString() => $"X: {X}  Y: {Y}";
}

class Value
{
    string Name;
    int Age;

    public Value(string name, int age)
    {
        Name = name;
        Age = age;
    }
    
    public override string ToString() => $"Name: {Name}  Age: {Age}";
}