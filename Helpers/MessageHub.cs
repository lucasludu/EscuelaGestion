namespace EscuelaGestion.Helpers
{
    public static class MessageHub
    {
        public static event Action? ConfigurationChanged;
        
        public static void NotifyConfigurationChanged()
        {
            ConfigurationChanged?.Invoke();
        }
    }
}
