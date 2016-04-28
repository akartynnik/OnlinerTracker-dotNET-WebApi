using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OnlinerTracker.Services.BaseServices
{
    public class StringComposer : IStringComposer
    {
        public string GetUserNotificationHtmlString(IEnumerable<ProductForNotification> products)
        {
            var body = products.Aggregate(string.Empty,
                (current, product) =>
                    current +
                    (string.Format("{0}<b>{1}</b> ( old:{2})<br/>", product.Name, product.CurrentCost, product.DayAgoCost)));
            return string.Format("<html><body>{0}</body></html>", body);
        }
    }
}
