namespace EscuelaGestion.Helpers
{
    public static class MessageHub
    {
        public static event Action? ConfigurationChanged;
        public static event Action? StudentsChanged;
        
        public static void NotifyConfigurationChanged()
        {
            ConfigurationChanged?.Invoke();
        }

        public static void NotifyStudentsChanged()
        {
            StudentsChanged?.Invoke();
        }
    }
}
