namespace ElAurelion_Sol
{
    using EloBuddy.SDK.Events;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += AurelionSol.OnGameLoad;
        }
    }
}