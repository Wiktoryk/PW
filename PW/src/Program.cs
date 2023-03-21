class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("Podaj 2 liczby,które chcesz dodać");
        int x =Int32.Parse(Console.ReadLine());
        int y =Int32.Parse(Console.ReadLine());
        System.Console.WriteLine(add(x, y));
    }
    static int add(int x, int y) { return x + y; }
}
