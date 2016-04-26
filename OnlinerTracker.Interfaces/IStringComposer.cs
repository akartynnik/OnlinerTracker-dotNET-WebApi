using OnlinerTracker.Data;
using System.Collections.Generic;

namespace OnlinerTracker.Interfaces
{
    public interface IStringComposer
    {
        string GetUserNotificationHtmlString(IEnumerable<ProductForNotification> products);
    }
}
