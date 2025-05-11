using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
namespace DataAccess
{
    public class SingletonBase<T> where T : class, new()
    {
        private static T _instance;
        public static readonly object _lock = new object();
        public static ApplicationDbContext _context = new ApplicationDbContext();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }
    }
}
