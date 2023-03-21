namespace PW
{
     public class Program
    {
        public Program() { }
        static void Main(string[] args)
        {
            Program program = new Program();
            System.Console.WriteLine("Podaj 2 liczby,które chcesz dodać");
            int x = Int32.Parse(Console.ReadLine());
            int y = Int32.Parse(Console.ReadLine());
            System.Console.WriteLine(program.add(x, y));
        }
        public int add(int x, int y) { return x + y; }
    }
}
