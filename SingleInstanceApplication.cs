using System.Threading;
namespace Exam
{
    public static class SingleInstanceApplication
    {
        private static Mutex mutex = null;
        private const string AppGuid = "2d0c26c1-a20d-4f05-8e1e-24f469950d87"; //random
        public static bool IsApplicationAlreadyRunning()
        {
            bool createdNew;
            mutex = new Mutex(true, AppGuid, out createdNew);
            return !createdNew;
        }
        public static void Release()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
                mutex = null;
            }
        }
    }
}
