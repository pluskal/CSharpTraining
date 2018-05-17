using System.Data.Entity;

namespace ChatService.Persistence
{
    public static class Extensions
    {
        public static void DeleteAll<T>(this DbContext context)
            where T : class
        {
            foreach (T p in context.Set<T>())
            {
                context.Entry(p).State = EntityState.Deleted;
            }
        }
    }
}