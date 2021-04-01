using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSMSApp.Model;

namespace WpfSMSApp.Logic
{
    public class DataAccess
    {
        public static List<User> GetUsers()
        {
            List<User> users;

            using (var ctx = new SMSEntities())
            {
                users = ctx.User.ToList(); // = SELECT * FROM user
            }

            return users;
        }

        // SetUser : 값 입력 및 수정 동시작업
        //<return> 0 또는 1이상의 값 </return>
        internal static int SetUser(User user)
        {
            using (var ctx = new SMSEntities())
            {
                ctx.User.AddOrUpdate(user);
                return ctx.SaveChanges(); // 커밋 
            }
        }

        internal static List<Stock> GetStocks()
        {
            List<Stock> stocks;
            using (var ctx = new SMSEntities())
            {
                stocks = ctx.Stock.ToList();
            }
            return stocks;
        }

        internal static List<Store> GetStores()
        {
            List<Store> stores;
            using (var ctx = new SMSEntities())
            {
                stores = ctx.Store.ToList();
            }
            return stores;
        }

        public static int SetStore(Store store)
        {
            using (var ctx = new SMSEntities())
            {
                ctx.Store.AddOrUpdate(store);
                return ctx.SaveChanges();
            }
        }
    }
}
