using System.Drawing;
namespace MultiThreadProject;

public class PiCalculator
{
    public static string Calculate(int digits=100)
    {
           double pi = 0;
            for (int i = 0; i < digits; i++)
            {
                pi += 4 * (Math.Pow(-1, i) / (2 * i + 1));
            }
            return pi.ToString();
    }
    
    public static string Calculate(int digits,CancellationToken token)
    {
        double pi = 0;
        for (int i = 0; i < digits; i++)
        {
            pi += 4 * (Math.Pow(-1, i) / (2 * i + 1));
            if (token.IsCancellationRequested)
            {
                break;
            }
            Console.WriteLine(i);
        }
        return pi.ToString();
    }
}
