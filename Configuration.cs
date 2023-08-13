namespace Oxide.Ext.CustomNpc
{
    public class Configuration
    {
        private static Configuration m_current;
        public static Configuration Current => m_current != null ? m_current : Load();

        public static Configuration Load()
        {
            if (ConfigurationFileExist() == false)
            {
                return Create();
            }

            // else Load

            return null;
        }

        private static bool ConfigurationFileExist()
        {
            return false;
        }

        private static Configuration Create()
        {
            return null;
        }
    }
}
